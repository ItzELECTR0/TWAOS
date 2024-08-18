using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class SequenceTool : VisualElement
    {
        private static readonly string[] USS_PATH = 
        {
            EditorPaths.VISUAL_SCRIPTING + "Sequence/StyleSheets/Sequence",
            EditorPaths.VISUAL_SCRIPTING + "Sequence/StyleSheets/Playback",
            EditorPaths.VISUAL_SCRIPTING + "Sequence/StyleSheets/Track",
            EditorPaths.VISUAL_SCRIPTING + "Sequence/StyleSheets/Clip",
            EditorPaths.VISUAL_SCRIPTING + "Sequence/StyleSheets/Details"
        };
        
        private const string NAME_HEAD = "GC-Sequence-Head";
        private const string NAME_BODY = "GC-Sequence-Body";
        private const string NAME_FOOT = "GC-Sequence-Foot";

        public const int TRACKS_OFFSET_L = 20;
        public const int TRACKS_OFFSET_R = 20;

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly VisualElement m_Head;
        private readonly VisualElement m_Body;
        private readonly VisualElement m_Foot;

        // PROPERTIES: ----------------------------------------------------------------------------

        public virtual int TracksOffsetL => TRACKS_OFFSET_L;
        public virtual int TracksOffsetR => TRACKS_OFFSET_R;

        public virtual bool ShowMetric0 => true;
        public virtual bool ShowMetric1 => true;

        public virtual bool RoundTimelineHead => false;

        public SerializedObject SerializedObject => this.Property.serializedObject;
        private SerializedProperty Property { get; }
        
        public PlaybackTool PlaybackTool { get; private set; }
        public TrackTool[] TrackTools { get; private set; }
        public DetailsTool DetailsTool { get; private set; }
        
        public SerializedProperty Tracks
        {
            get
            {
                SerializationUtils.ApplyUnregisteredSerialization(this.SerializedObject);
                return this.Property.FindPropertyRelative("m_Tracks");
            }
        }

        public int SelectedTrack { get; private set; } = -1;

        public bool IsEnabled
        {
            get => this.PlaybackTool.enabledSelf;
            set => this.PlaybackTool.SetEnabled(value);
        }
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int, int> EventSelect;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        protected SequenceTool(SerializedProperty property)
        {
            this.Property = property;
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);
            
            this.m_Head = new VisualElement { name = NAME_HEAD };
            this.m_Body = new VisualElement { name = NAME_BODY };
            this.m_Foot = new VisualElement { name = NAME_FOOT };
            
            this.CreateElements();
            
            this.Add(this.m_Head);
            this.Add(this.m_Body);
            this.Add(this.m_Foot);

            this.SetupHead();
            this.SetupBody();
            this.SetupFoot();
        }

        private void SetupHead()
        {
            this.PlaybackTool = new PlaybackTool(this);
            this.m_Head.Add(this.PlaybackTool);
        }
        
        private void SetupBody()
        {
            SerializedProperty tracks = this.Tracks;
            this.TrackTools = new TrackTool[tracks.arraySize];

            for (int i = 0; i < TrackTools.Length; ++i)
            {
                TrackTool trackTool = this.CreateTrackTool(i);
                trackTool.EventSelectClip += this.SelectClip;

                this.TrackTools[i] = trackTool;

                this.m_Body.Add(trackTool);
            }
        }
        
        private void SetupFoot()
        {
            this.DetailsTool = new DetailsTool(this);
            this.m_Foot.Add(this.DetailsTool);
        }

        protected virtual void CreateElements()
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void RefreshSelection()
        {
            if (this.SelectedTrack == -1)
            {
                this.DetailsTool.Refresh();
                return;
            }
            
            TrackTool trackTool = TrackTools[this.SelectedTrack];
            if (trackTool.SelectedClip == -1)
            {
                this.DetailsTool.Refresh();
                return;
            }

            trackTool.ClipTools[trackTool.SelectedClip].Refresh();
            this.DetailsTool.Refresh();
        }
        
        public void Refresh()
        {
            foreach (TrackTool trackTool in this.TrackTools)
            {
                trackTool.Refresh();
            }
            
            this.DetailsTool.Refresh();
        }
        
        public void SelectClip(int trackIndex, int clipIndex)
        {
            if (this.SelectedTrack != -1)
            {
                TrackTool trackTool = this.TrackTools[this.SelectedTrack];
                if (this.SelectedTrack == trackIndex && trackTool.SelectedClip == clipIndex)
                {
                    return;
                }

                this.TrackTools[this.SelectedTrack].Refresh();
            }

            this.SelectedTrack = trackIndex;
            this.TrackTools[this.SelectedTrack].Select(clipIndex);
            
            this.DetailsTool.Refresh();
            this.EventSelect?.Invoke(trackIndex, clipIndex);
        }

        public void ShowTrack(int trackIndex, bool show)
        {
            if (trackIndex < 0 || trackIndex >= this.TrackTools.Length) return;
            this.TrackTools[trackIndex].style.display = show 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected internal virtual void SetupControlL(PlaybackTool playbackTool)
        {
            playbackTool.style.paddingLeft = this.TracksOffsetL + 1;
        }
        
        protected internal virtual void SetupControlR(PlaybackTool playbackTool)
        {
            playbackTool.style.paddingRight = this.TracksOffsetR + 1;
        }
        
        protected virtual TrackTool CreateTrackTool(int trackIndex)
        {
            SerializedProperty track = this.Tracks.GetArrayElementAtIndex(trackIndex);
            return track != null ? new TrackToolDefault(this, trackIndex) : null;
        }
    }
}