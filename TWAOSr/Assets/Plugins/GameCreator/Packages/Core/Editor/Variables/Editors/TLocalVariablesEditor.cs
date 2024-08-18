using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public abstract class TLocalVariablesEditor : UnityEditor.Editor
    {
        private static readonly Length ERROR_MARGIN = new Length(10, LengthUnit.Pixel);
        private const string ERR_DUPLICATE_ID = "Another Variable component has the same ID";

        // MEMBERS: -------------------------------------------------------------------------------
        
        private ErrorMessage m_Error;
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement
            {
                style =
                {
                    marginTop = new StyleLength(5)
                }
            };

            SerializedProperty runtime = this.serializedObject.FindProperty("m_Runtime");
            SerializedProperty saveUniqueID = this.serializedObject.FindProperty("m_SaveUniqueID");

            PropertyField fieldRuntime = new PropertyField(runtime);
            PropertyField fieldSaveUniqueID = new PropertyField(saveUniqueID);
            
            this.m_Error = new ErrorMessage(ERR_DUPLICATE_ID)
            {
                style = { marginTop = ERROR_MARGIN }
            };

            root.Add(fieldRuntime);
            root.Add(this.m_Error);
            root.Add(fieldSaveUniqueID);
            
            this.RefreshErrorID();
            fieldSaveUniqueID.RegisterValueChangeCallback(_ =>
            {
                this.RefreshErrorID();
            });

            return root;
        }
        
        private void RefreshErrorID()
        {
            this.serializedObject.Update();
            this.m_Error.style.display = DisplayStyle.None;

            if (PrefabUtility.IsPartOfPrefabAsset(this.target)) return;
            
            TLocalVariables instance = this.target as TLocalVariables;
            if (instance != null)
            {
                PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null && stage.IsPartOfPrefabContents(instance.gameObject)) return;
            }
            
            SerializedProperty saveUniqueID = this.serializedObject.FindProperty("m_SaveUniqueID");
            
            string itemID = saveUniqueID
                .FindPropertyRelative(SaveUniqueIDDrawer.PROP_UNIQUE_ID)
                .FindPropertyRelative(UniqueIDDrawer.SERIALIZED_ID)
                .FindPropertyRelative(IdStringDrawer.NAME_STRING)
                .stringValue;

            TLocalVariables[] variables = FindObjectsOfType<TLocalVariables>(true);
            foreach (TLocalVariables variable in variables)
            {
                if (variable.SaveID != itemID || variable == this.target) continue;
                this.m_Error.style.display = DisplayStyle.Flex;
                
                return;
            }
        }
    }
}
