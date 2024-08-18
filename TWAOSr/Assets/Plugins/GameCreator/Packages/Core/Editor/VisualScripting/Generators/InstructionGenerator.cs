using UnityEditor;

namespace GameCreator.Editor.VisualScripting
{
    internal static class InstructionGenerator
    {
        [MenuItem(TScriptGenerator.MENU_PATH + "C# Instruction", false, 150)]
        public static void CreateTemplateInstruction()
        {
            TScriptGenerator.CreateScript(
                "Template_Instruction.txt",
                "Package_Instruction.txt",
                "MyInstruction.cs"
            );
        }
    }
}