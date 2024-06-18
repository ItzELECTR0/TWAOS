using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Cameras
{
    public abstract class TShotSystemDrawer : TBoxDrawer
    {
        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        protected override void CreatePropertyContent(VisualElement root, SerializedProperty property)
        {
            SerializationUtils.CreateChildProperties(
                root,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                true
            );
        }
    }
}