using GameCreator.Runtime.Common;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class DetailsTool : VisualElement
    {
        private const string NAME_TRACK = "GC-Sequence-Details-Head";
        private const string NAME_BODY = "GC-Sequence-Details-Body";
        private const string NAME_HEAD = "GC-Sequence-Details-Head-Track";
        private const string NAME_SIGN = "GC-Sequence-Details-Head-Sign";

        private static readonly IIcon ICON_SIGN = new IconSequenceArrow(ColorTheme.Type.White);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly VisualElement m_Head;
        private readonly VisualElement m_Body;
        
        private readonly VisualElement m_Track;
        private readonly VisualElement m_Sign;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public SequenceTool SequenceTool { get; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public DetailsTool(SequenceTool sequenceTool)
        {
            this.m_Head = new VisualElement { name = NAME_HEAD };
            this.m_Body = new VisualElement { name = NAME_BODY };

            this.m_Head.style.paddingLeft = sequenceTool.TracksOffsetL;
            this.m_Head.style.paddingRight = sequenceTool.TracksOffsetR;

            this.m_Track = new VisualElement { name = NAME_TRACK };
            
            this.m_Sign = new VisualElement
            {
                name = NAME_SIGN,
                style = { backgroundImage = ICON_SIGN.Texture }
            };

            this.m_Track.Add(this.m_Sign);
            this.m_Head.Add(this.m_Track);

            this.Add(this.m_Head);
            this.Add(this.m_Body);
            
            this.SequenceTool = sequenceTool;
            
            this.RegisterCallback<GeometryChangedEvent>(OnChangeGeometry);
            this.Refresh();
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------
        
        private void OnChangeGeometry(GeometryChangedEvent eventGeometry)
        {
            this.RefreshSign();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Refresh()
        {
            TrackTool selectionTrack = this.SelectedTrackTool();
            if (selectionTrack == null || !selectionTrack.Track.HasInspector)
            {
                this.style.display = DisplayStyle.None;
                return;
            }

            ClipTool selectionClip = this.SelectedClipTool();
            if (selectionClip == null)
            {
                this.style.display = DisplayStyle.None;
                return;
            }
            
            this.style.display = DisplayStyle.Flex;
            this.RefreshSign(selectionClip);
            this.RefreshBindings(selectionClip);
        }

        public void RefreshSign()
        {
            TrackTool selectionTrack = this.SelectedTrackTool();
            if (selectionTrack == null || !selectionTrack.Track.HasInspector)
            {
                this.style.display = DisplayStyle.None;
                return;
            }
            
            ClipTool selectionClip = this.SelectedClipTool();
            if (selectionClip == null)
            {
                this.style.display = DisplayStyle.None;
                return;
            }
            
            this.style.display = DisplayStyle.Flex;
            this.RefreshSign(selectionClip);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshSign(ClipTool clipTool)
        {
            if (clipTool == null) return;
            float value = clipTool.PropertyTime.floatValue;
            
            float offset = this.m_Sign.resolvedStyle.width * 0.5f;
            float x = Mathf.Lerp(0f, this.m_Track.resolvedStyle.width, value);
            float y = this.m_Sign.transform.position.y;
            
            Vector3 position = new Vector3(x - offset, y, 0);
            this.m_Sign.transform.position = position;
        }

        private void RefreshBindings(ClipTool clipTool)
        {
            if (clipTool == null) return;

            PropertyField field = new PropertyField(clipTool.Property);
            
            this.m_Body.Clear();
            this.m_Body.Add(field);

            clipTool.Property.serializedObject.Update();
            field.Bind(this.SequenceTool.SerializedObject);
        }

        private ClipTool SelectedClipTool()
        {
            TrackTool trackTool = this.SelectedTrackTool();
            if (trackTool == null) return null;
            
            return trackTool.SelectedClip != -1 
                ? trackTool.ClipTools[trackTool.SelectedClip]
                : null;
        }

        private TrackTool SelectedTrackTool()
        {
            return this.SequenceTool.SelectedTrack >= 0 
                ? this.SequenceTool.TrackTools[this.SequenceTool.SelectedTrack] 
                : null;
        }
    }
}