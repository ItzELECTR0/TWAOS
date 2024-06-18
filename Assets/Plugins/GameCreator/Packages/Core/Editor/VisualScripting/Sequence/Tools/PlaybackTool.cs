using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class PlaybackTool : VisualElement
    {
        private struct Metric
        {
            public float time;
            public VisualElement element;
        }
        
        // CONSTANTS: -----------------------------------------------------------------------------
        
        private const string NAME_PLAYBACK_TRACK = "GC-Sequence-Playback-Track";
        private const string NAME_PLAYBACK_TOOLTIP = "GC-Sequence-Playback-Tooltip";
        private const string NAME_PLAYBACK_HEAD = "GC-Sequence-Playback-Head";

        private const string NAME_PLAYBACK_METRIC_UNIT = "GC-Sequence-Playback-Metric-Unit";
        private const string NAME_PLAYBACK_METRIC_FRAC = "GC-Sequence-Playback-Metric-Fraction";

        private const string KEY_PLAYBACK_VALUE = "gc:sequence:playback-head-value";
        private static readonly IIcon ICON_HEAD_ON = new IconSequenceHead(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_HEAD_OFF = new IconSequenceHead(ColorTheme.Type.TextLight);

        private const float TOOLTIP_WIDTH = 35f;
        private const float TOOLTIP_PADDING = 10f;

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly SequenceTool m_SequenceTool;
        private readonly HandleDragManipulator m_Manipulator;
        
        private readonly VisualElement m_PlaybackTrack;

        private readonly Label m_PlaybackTooltip;
        private readonly Image m_PlaybackHead;

        [NonSerialized] private float m_MaxFrame;
        [NonSerialized] private Metric[] m_Metrics = Array.Empty<Metric>();

        // PROPERTIES: ----------------------------------------------------------------------------

        public float Value
        {
            get => EditorPrefs.GetFloat(KEY_PLAYBACK_VALUE, 0.25f);
            set
            {
                if (Math.Abs(this.Value - value) < float.Epsilon) return;
                EditorPrefs.SetFloat(KEY_PLAYBACK_VALUE, value);
                
                this.RefreshPlaybackHead();
                this.RefreshPlaybackTooltip();
                this.EventChange?.Invoke();
            }
        }

        public float MaxFrame
        {
            get => this.m_MaxFrame;
            set
            {
                this.m_MaxFrame = value;
                
                this.RefreshPlaybackHead();
                this.RefreshPlaybackTooltip();
                
                this.RecalculatePlaybackMetrics();
                this.RefreshPlaybackMetrics();
            }
        }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventChange;
        
        public event Action EventDragStart;
        public event Action EventDragFinish;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PlaybackTool(SequenceTool sequenceTool)
        {
            this.m_SequenceTool = sequenceTool;

            this.m_PlaybackTrack = new VisualElement { name = NAME_PLAYBACK_TRACK };
            
            this.m_PlaybackTooltip = new Label("0")
            {
                name = NAME_PLAYBACK_TOOLTIP,
                pickingMode = PickingMode.Ignore,
                style =
                {
                    width = new Length(TOOLTIP_WIDTH, LengthUnit.Pixel),
                    display = DisplayStyle.None
                }
            };
            
            this.m_PlaybackHead = new Image
            {
                image = ICON_HEAD_OFF.Texture,
                name = NAME_PLAYBACK_HEAD
            };

            this.m_PlaybackTrack.RegisterCallback<GeometryChangedEvent>(this.OnChangeGeometry);

            this.m_SequenceTool.SetupControlL(this);

            this.m_Manipulator = new HandleDragManipulator(
                this.OnChangeFromStart,
                this.OnChangeFromFinish,
                this.OnChangeFromMove
            );
            
            this.m_PlaybackTrack.AddManipulator(this.m_Manipulator);

            this.RecalculatePlaybackMetrics();

            this.m_PlaybackTrack.Add(this.m_PlaybackTooltip);
            this.m_PlaybackTrack.Add(this.m_PlaybackHead);
            this.Add(this.m_PlaybackTrack);
            
            this.m_SequenceTool.SetupControlR(this);
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnChangeFromStart()
        {
            float x = this.m_Manipulator.StartPosition.x;
            this.m_PlaybackHead.image = ICON_HEAD_ON.Texture;
            this.m_PlaybackTooltip.style.display = DisplayStyle.Flex;
            
            this.OnChange(x);
            this.EventDragStart?.Invoke();
        }

        private void OnChangeFromFinish()
        {
            this.m_PlaybackHead.image = ICON_HEAD_OFF.Texture;
            this.m_PlaybackTooltip.style.display = DisplayStyle.None;
            this.EventDragFinish?.Invoke();
        }

        private void OnChangeFromMove()
        {
            float x = this.m_Manipulator.FinishPosition.x;
            this.OnChange(x);
        }

        private void OnChange(float x)
        {
            float contentLength = this.m_PlaybackTrack.resolvedStyle.width;
            if (contentLength <= 0f) return;

            float normalized = x / contentLength;

            float value = normalized;
            if (this.m_SequenceTool.RoundTimelineHead && this.MaxFrame != 0f)
            {
                float frame = Mathf.Round(Mathf.Clamp01(normalized) * this.MaxFrame);
                value = frame / this.MaxFrame;
            }

            this.Value = Mathf.Clamp01(value);
        }

        private void OnChangeGeometry(GeometryChangedEvent eventGeometry)
        {
            if (eventGeometry.oldRect == eventGeometry.newRect) return;
            
            this.RefreshPlaybackHead();
            this.RefreshPlaybackTooltip();
            this.RefreshPlaybackMetrics();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshPlaybackHead()
        {
            float playheadOffset = this.m_PlaybackHead.resolvedStyle.width * 0.5f;
            float x = Mathf.Lerp(0f, this.m_PlaybackTrack.resolvedStyle.width, this.Value);
            float y = this.m_PlaybackHead.transform.position.y;
            
            Vector3 position = new Vector3(x - playheadOffset, y, 0);
            this.m_PlaybackHead.transform.position = position;
        }

        private void RefreshPlaybackTooltip()
        {
            float time = this.Value;
            float offset = time < 0.75f 
                ? TOOLTIP_PADDING
                : -1f * (TOOLTIP_WIDTH + TOOLTIP_PADDING);
            
            float x = Mathf.Lerp(0f, this.m_PlaybackTrack.resolvedStyle.width, time);
            float y = this.m_PlaybackTooltip.transform.position.y;
            
            Vector3 position = new Vector3(x + offset, y, 0);
            this.m_PlaybackTooltip.transform.position = position;
            this.m_PlaybackTooltip.text = (time * this.MaxFrame).ToString("0000");
        }

        private void RecalculatePlaybackMetrics()
        {
            if (this.m_MaxFrame == 0)
            {
                this.m_Metrics = new Metric[1];
                this.m_Metrics[0] = new Metric { time = 0f };
            }
            else
            {
                this.m_Metrics = new Metric[(int) this.m_MaxFrame + 1];
                for (int i = 0; i <= this.m_MaxFrame; ++i)
                {
                    this.m_Metrics[i] = new Metric { time = i / this.m_MaxFrame };
                }
            }

            for (int i = this.m_PlaybackTrack.childCount - 3; i >= 0; --i)
            {
                this.m_PlaybackTrack.RemoveAt(i);   
            }

            for (int i = 0; i < this.m_Metrics.Length; i++)
            {
                this.m_Metrics[i].element = new VisualElement
                {
                    style = { backgroundColor = ColorTheme.Get(ColorTheme.Type.TextNormal) }
                };
                
                this.m_PlaybackTrack.Insert(0, this.m_Metrics[i].element);
            }
        }

        private void RefreshPlaybackMetrics()
        {
            float size = this.m_PlaybackTrack.resolvedStyle.width - 1;
            for (int i = 0; i < this.m_Metrics.Length; i++)
            {
                VisualElement element = this.m_Metrics[i].element;
                
                float position = size * this.m_Metrics[i].time;
                element.transform.position = new Vector3(position, 0);

                element.name = i % 30 == 0
                    ? NAME_PLAYBACK_METRIC_UNIT
                    : NAME_PLAYBACK_METRIC_FRAC;
            }

            if (this.m_Metrics.Length == 0) return;

            if (this.m_Metrics.Length >= 1)
            {
                this.m_Metrics[0].element.style.display = this.m_SequenceTool.ShowMetric0
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }

            if (this.m_Metrics.Length >= 2)
            {
                this.m_Metrics[^1].element.style.display = this.m_SequenceTool.ShowMetric1
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }
    }
}