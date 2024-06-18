using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomEditor(typeof(GlobalNameVariables))]
    public class GlobalNameVariablesEditor : TGlobalVariablesEditor
    {
        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/RuntimeGlobalList";
        
        private const string NAME_LIST = "GC-RuntimeGlobal-List-Head";
        private const string CLASS_LIST_ELEMENT = "gc-runtime-global-list-element";

        // PAINT EDITOR: --------------------------------------------------------------------------

        protected override void PaintEditor()
        {
            SerializedProperty nameList = this.serializedObject.FindProperty("m_NameList");
            SerializedProperty saveUniqueID = this.serializedObject.FindProperty(PROP_SAVE_UNIQUE_ID);

            PropertyField fieldNameList = new PropertyField(nameList);
            PropertyField fieldSaveUniqueID = new PropertyField(saveUniqueID);

            this.m_Body.Add(fieldNameList);
            this.m_Body.Add(this.m_MessageID);
            this.m_Body.Add(fieldSaveUniqueID);
            
            this.RefreshErrorID();
            fieldSaveUniqueID.RegisterValueChangeCallback(_ => this.RefreshErrorID());
        }
        
        // PAINT RUNTIME: -------------------------------------------------------------------------

        protected override void PaintRuntime()
        {
            GlobalNameVariables variables = this.target as GlobalNameVariables;
            if (variables == null) return;
            
            variables.Unregister(this.RuntimeOnChange);
            variables.Register(this.RuntimeOnChange);
            
            this.RuntimeOnChange(string.Empty);
        }

        private void RuntimeOnChange(string variableName)
        {
            this.m_Body.Clear();
            this.m_Body.styleSheets.Clear();
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) this.m_Body.styleSheets.Add(styleSheet);

            VisualElement content = new VisualElement
            {
                name = NAME_LIST
            };

            GlobalNameVariables variables = this.target as GlobalNameVariables;
            if (variables == null) return;

            string[] names = variables.Names;
            foreach (string id in names)
            {
                Image image = new Image
                {
                    image = variables.Icon(id)
                };
            
                Label title = new Label(variables.Title(id));
                title.style.color = ColorTheme.Get(ColorTheme.Type.TextNormal);

                VisualElement element = new VisualElement();
                element.AddToClassList(CLASS_LIST_ELEMENT);

                element.Add(image);
                element.Add(title);
            
                content.Add(element);
            }
            
            this.m_Body.Add(content);
        }
    }
}
