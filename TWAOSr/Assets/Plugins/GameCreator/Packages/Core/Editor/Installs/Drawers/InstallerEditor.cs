using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    [CustomEditor(typeof(Installer))]
    internal class InstallerEditor : UnityEditor.Editor
    {
        private const string USS_PATH = EditorPaths.INSTALLS + "Windows/StyleSheets/Installer";

        private const string NAME_BUTTON = "GC-Installer-Button";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Installer m_Installer;
        
        private VisualElement m_Root;
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Installer = this.target as Installer;
            this.m_Root = new VisualElement();
            
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.m_Root.styleSheets.Add(styleSheet);
            }

            this.m_Root.Add(new Button(this.Open)
            {
                text = $"Open in {InstallManager.NAME}",
                name = NAME_BUTTON
            });

            SerializedProperty install = this.serializedObject.FindProperty("m_Install");
            this.m_Root.Add(new PropertyField(install));
            
            return m_Root;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Open()
        {
            InstallerManagerWindow.Open(this.m_Installer);
        }
    }
}