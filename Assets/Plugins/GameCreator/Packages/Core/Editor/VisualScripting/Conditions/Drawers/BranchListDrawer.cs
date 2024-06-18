using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(BranchList))]
    public class BranchListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            BranchListTool branchListTool = new BranchListTool(
                property
            );
            
            return branchListTool;
        }
    }
}