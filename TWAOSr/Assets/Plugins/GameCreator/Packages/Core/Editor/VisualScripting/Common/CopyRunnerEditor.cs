using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomEditor(typeof(TCopyRunner), true)]
    public class CopyRunnerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            SerializedProperty runner = this.serializedObject.FindProperty("m_Runner");
            return new PropertyField(runner);
        }
    }
}