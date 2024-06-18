using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomEditor(typeof(Remember))]
    public class RememberEditor : UnityEditor.Editor
    {
        private static readonly Length DEFAULT_MARGIN_TOP = new Length(5, LengthUnit.Pixel);
        private static readonly Length ERROR_MARGIN = new Length(10, LengthUnit.Pixel); 
        
        private const string ERR_DUPLICATE_ID = "Another Remember component has the same ID";

        private VisualElement m_Root;
        private ErrorMessage m_Error;
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement
            {
                style =
                {
                    marginTop = DEFAULT_MARGIN_TOP
                }
            };

            SerializedProperty memories = this.serializedObject.FindProperty("m_Memories");
            SerializedProperty saveUniqueID = this.serializedObject.FindProperty("m_SaveUniqueID");

            PropertyField fieldMemories = new PropertyField(memories);
            PropertyField fieldUniqueID = new PropertyField(saveUniqueID);
            
            this.m_Error = new ErrorMessage(ERR_DUPLICATE_ID)
            {
                style = { marginTop = ERROR_MARGIN }
            };
            
            this.m_Root.Add(fieldMemories);
            this.m_Root.Add(this.m_Error);
            this.m_Root.Add(fieldUniqueID);

            this.RefreshErrorID();
            fieldUniqueID.RegisterValueChangeCallback(_ =>
            {
                this.RefreshErrorID();
            });

            return this.m_Root;
        }

        private void RefreshErrorID()
        {
            this.serializedObject.Update();
            this.m_Error.style.display = DisplayStyle.None;

            if (PrefabUtility.IsPartOfPrefabAsset(this.target)) return;

            SerializedProperty saveUniqueID = this.serializedObject.FindProperty("m_SaveUniqueID");
            
            string itemID = saveUniqueID
                .FindPropertyRelative(SaveUniqueIDDrawer.PROP_UNIQUE_ID)
                .FindPropertyRelative(UniqueIDDrawer.SERIALIZED_ID)
                .FindPropertyRelative(IdStringDrawer.NAME_STRING)
                .stringValue;

            Remember[] remembers = FindObjectsOfType<Remember>(true);
            foreach (Remember remember in remembers)
            {
                if (remember.SaveID != itemID || remember == this.target) continue;
                this.m_Error.style.display = DisplayStyle.Flex;
                
                return;
            }
        }
    }
}
