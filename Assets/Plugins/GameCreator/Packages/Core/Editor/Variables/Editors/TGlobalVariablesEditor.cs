using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TGlobalVariablesEditor : UnityEditor.Editor
    {
        private const string ERR_DUPLICATE_ID = "Another Variable has the same ID";

        protected const string PROP_SAVE_UNIQUE_ID = "m_SaveUniqueID";
        
        // MEMBERS: -------------------------------------------------------------------------------

        protected VisualElement m_Root;
        protected VisualElement m_Head;
        protected VisualElement m_Body;

        protected ErrorMessage m_MessageID;

        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            this.m_Head = new VisualElement();
            this.m_Body = new VisualElement();
            
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);

            this.m_Root.style.marginTop = new StyleLength(5);

            this.m_MessageID = new ErrorMessage(ERR_DUPLICATE_ID)
            {
                style = { marginTop = new StyleLength(10) }
            };

            switch (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                case true: this.PaintRuntime(); break;
                case false: this.PaintEditor(); break;
            }

            return this.m_Root;
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract void PaintRuntime();
        protected abstract void PaintEditor();
        
        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected void RefreshErrorID()
        {
            this.serializedObject.Update();
            this.m_MessageID.style.display = DisplayStyle.None;

            SerializedProperty id = this.serializedObject.FindProperty(PROP_SAVE_UNIQUE_ID);
            
            string itemID = id
                .FindPropertyRelative(SaveUniqueIDDrawer.PROP_UNIQUE_ID)
                .FindPropertyRelative(UniqueIDDrawer.SERIALIZED_ID)
                .FindPropertyRelative(IdStringDrawer.NAME_STRING)
                .stringValue;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(TGlobalVariables)}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TGlobalVariables variables = AssetDatabase.LoadAssetAtPath<TGlobalVariables>(path);

                if (variables.UniqueID.String != itemID || variables == this.target) continue;
                
                this.m_MessageID.style.display = DisplayStyle.Flex;
                return;
            }
        }
    }
}