using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomEditor(typeof(GlobalListVariables))]
    public class GlobalListVariablesEditor : TGlobalVariablesEditor
    {
        private const string USS_PATH = EditorPaths.VARIABLES + "StyleSheets/RuntimeGlobalList";

        private const string NAME_LIST = "GC-RuntimeGlobal-List-Head";
        private const string CLASS_LIST_ELEMENT = "gc-runtime-global-list-element";

        // PAINT EDITOR: --------------------------------------------------------------------------

        protected override void PaintEditor()
        {
            SerializedProperty indexList = this.serializedObject.FindProperty("m_IndexList");
            SerializedProperty saveUniqueID = this.serializedObject.FindProperty(PROP_SAVE_UNIQUE_ID);

            PropertyField indexListRuntime = new PropertyField(indexList);
            PropertyField fieldSaveUniqueID = new PropertyField(saveUniqueID);

            this.m_Body.Add(indexListRuntime);
            this.m_Body.Add(this.m_MessageID);
            this.m_Body.Add(fieldSaveUniqueID);
            
            this.RefreshErrorID();
            fieldSaveUniqueID.RegisterValueChangeCallback(_ => this.RefreshErrorID());
        }
        
        // PAINT RUNTIME: -------------------------------------------------------------------------

        protected override void PaintRuntime()
        {
            GlobalListVariables variables = this.target as GlobalListVariables;
            if (variables == null) return;
            
            variables.Unregister(this.RuntimeOnChange);
            variables.Register(this.RuntimeOnChange);
            
            this.RuntimeOnChange(ListVariableRuntime.Change.Set, 0);
        }

        private void RuntimeOnChange(ListVariableRuntime.Change changeType, int index)
        {
            this.m_Body.Clear();
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) this.m_Body.styleSheets.Add(styleSheet);

            VisualElement content = new VisualElement
            {
                name = NAME_LIST
            };

            GlobalListVariables variables = this.target as GlobalListVariables;
            if (variables == null) return;

            int variablesCount = variables.Count;
            for (int i = 0; i < variablesCount; ++i)
            {
                Image image = new Image
                {
                    image = variables.Icon(i)
                };
            
                Label title = new Label(variables.Title(i));
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
