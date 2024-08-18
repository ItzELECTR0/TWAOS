using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(UseRaycast))]
    public class UseRaycastDrawer : PropertyDrawer
    {
        private const string CLASS_INLINE_FIELD = "gc-inline-toggle-field";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            root.AddToClassList(CLASS_INLINE_FIELD);
            
            StyleSheet[] sheets = StyleSheetUtils.Load();
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);
            
            SerializedProperty useRaycast = property.FindPropertyRelative("m_UseRaycast");
            SerializedProperty layerMask = property.FindPropertyRelative("m_LayerMask");
            
            Toggle fieldUseRaycast = new Toggle
            {
                label = useRaycast.displayName,
                bindingPath = useRaycast.propertyPath
            };
            
            PropertyField fieldLayerMask = new PropertyField(layerMask, string.Empty);
            
            fieldUseRaycast.RegisterValueChangedCallback(changeEvent =>
            {
                fieldLayerMask.SetEnabled(changeEvent.newValue);
            });
            
            fieldLayerMask.SetEnabled(useRaycast.boolValue);

            root.Add(fieldUseRaycast);
            root.Add(fieldLayerMask);
            
            AlignLabel.On(root);
            
            return root;
        }
    }
}