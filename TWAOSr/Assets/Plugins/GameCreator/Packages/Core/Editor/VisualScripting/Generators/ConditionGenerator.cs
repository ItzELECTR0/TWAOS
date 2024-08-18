using UnityEditor;

namespace GameCreator.Editor.VisualScripting
{
    public static class ConditionGenerator
    {
        [MenuItem(TScriptGenerator.MENU_PATH + "C# Condition", false, 151)]
        internal static void CreateTemplateCondition()
        {
            TScriptGenerator.CreateScript(
                "Template_Condition.txt",
                "Package_Condition.txt",
                "MyCondition.cs"
            );
        }        
    }
}