using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(Memories))]
    public class MemoriesDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            MemoriesTool memoriesTool = new MemoriesTool(property);
            return memoriesTool;
        }
    }
}