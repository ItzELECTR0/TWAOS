using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomEditor(typeof(MaterialSoundsAsset))]
    public class MaterialSoundsAssetEditor : UnityEditor.Editor
    {
        // PAINT METHODS: -------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            SerializedProperty materialSounds = this.serializedObject.FindProperty("m_MaterialSounds");
            root.Add(new PropertyField(materialSounds));
            
            return root;
        }
    }
}