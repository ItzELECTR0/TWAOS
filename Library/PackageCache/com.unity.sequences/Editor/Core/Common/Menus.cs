using UnityEditor.SceneManagement;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    internal class Menus
    {
        const string k_ParentMenuName = "Window/Sequencing";

        [MenuItem(k_ParentMenuName + "/Sequences", priority = 3004)]
        static void OpenSequencesWindow()
        {
            var win = EditorWindow.GetWindow<SequencesWindow>();
            win.Show();
        }

        [MenuItem(k_ParentMenuName + "/Sequence Assets", priority = 3004)]
        static void OpenSequenceAssetsWindow()
        {
            var win = EditorWindow.GetWindow<SequenceAssetsWindow>(typeof(SequencesWindow));
            win.Show();
        }

        [MenuItem(k_ParentMenuName + "/Sequence Assembly", priority = 3004)]
        static void OpenSequenceAssemblyWindow()
        {
            var win = EditorWindow.GetWindow<SequenceAssemblyWindow>(typeof(SequencesWindow));
            win.Show();
        }
    }
}
