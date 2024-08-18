using UnityEditor;

namespace GameCreator.Editor.VisualScripting
{
    public static class EventGenerator
    {
        [MenuItem(TScriptGenerator.MENU_PATH + "C# Event", false, 152)]
        internal static void CreateTemplateEvent()
        {
            TScriptGenerator.CreateScript(
                "Template_Event.txt",
                "Package_Event.txt",
                "MyEvent.cs"
            );
        }
    }
}