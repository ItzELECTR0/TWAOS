using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;

namespace GameCreator.Editor.VisualScripting
{
    public class TrackToolDefault : TrackTool
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public TrackToolDefault(SequenceTool sequenceTool, int trackIndex) 
            : base(sequenceTool, trackIndex)
        { }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected override void OnCreateClip(SerializedProperty clip, float time, float duration)
        {
            clip.SetValue(new ClipDefault());
            clip.FindPropertyRelative("m_Time").floatValue = time;
            clip.FindPropertyRelative("m_Duration").floatValue = 0f;
        }
    }
}