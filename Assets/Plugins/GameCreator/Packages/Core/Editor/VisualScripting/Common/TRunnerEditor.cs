using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomEditor(typeof(Runner), true)]
    public class TRunnerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.Add(new SpaceSmaller());
            
            SerializedProperty value = this.serializedObject.FindProperty("m_Value");
            root.Add(new PropertyField(value));
            
            return root;
        }
    }
}