using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(Volumes))]
    public class VolumesDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VolumesTool volumesTool = new VolumesTool(property);
            return volumesTool;
        }
    }
}