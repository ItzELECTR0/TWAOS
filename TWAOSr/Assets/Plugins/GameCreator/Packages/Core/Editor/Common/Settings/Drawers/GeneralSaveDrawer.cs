using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(GeneralSave))]
    public class GeneralSaveDrawer : TTitleDrawer
    {
        protected override string Title => "Saving";

        protected override void CreateContent(VisualElement body, SerializedProperty property)
        {
            SerializedProperty encryption = property.FindPropertyRelative("m_Encryption");
            SerializedProperty storage = property.FindPropertyRelative("m_Storage");
            
            PropertyElement fieldEncryption = new PropertyElement(encryption, "Encryption", false);
            PropertyElement fieldStorage = new PropertyElement(storage, "Storage", false);
            
            body.Add(fieldEncryption);
            body.Add(new SpaceSmaller());
            body.Add(fieldStorage);

            SerializedProperty load = property.FindPropertyRelative("m_Load");
            SerializedProperty scene = property.FindPropertyRelative("m_Scene");

            PropertyField fieldLoad = new PropertyField(load);
            PropertyField fieldScene = new PropertyField(scene);
            
            fieldLoad.RegisterValueChangeCallback(changeEvent =>
            {
                const int index = (int) LoadSceneMode.Scene;
                fieldScene.style.display = changeEvent.changedProperty.enumValueIndex == index
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            fieldScene.style.display = load.enumValueIndex == (int) LoadSceneMode.Scene
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            
            body.Add(new SpaceSmall());
            body.Add(fieldLoad);
            body.Add(fieldScene);
        }
    }
}