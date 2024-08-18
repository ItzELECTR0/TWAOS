using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(SaveUniqueID))]
    public class SaveUniqueIDDrawer : PropertyDrawer
    {
        private const string PATH_STYLES = EditorPaths.COMMON + "Structures/Save/StyleSheets/SaveUniqueID";

        public const string PROP_SAVE = "m_Save";
        public const string PROP_UNIQUE_ID = "m_UniqueID";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(PATH_STYLES);
            foreach (var styleSheet in styleSheets) root.styleSheets.Add(styleSheet);

            SerializedProperty propertySave = property.FindPropertyRelative(PROP_SAVE);
            SerializedProperty propertyUniqueID = property.FindPropertyRelative(PROP_UNIQUE_ID);

            PropertyField fieldSave = new PropertyField(propertySave);
            PropertyField fieldUniqueID = new PropertyField(propertyUniqueID);
            
            root.AddToClassList("gc-saveuniqueid-root");
            fieldSave.AddToClassList("gc-saveuniqueid-save");
            fieldUniqueID.AddToClassList("gc-saveuniqueid-uniqueid");
            
            root.Add(fieldSave);
            root.Add(fieldUniqueID);

            return root;
        }
    }
}