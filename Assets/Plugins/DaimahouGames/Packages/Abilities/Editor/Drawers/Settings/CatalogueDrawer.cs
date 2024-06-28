using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Runtime.Abilities
{
    [CustomPropertyDrawer(typeof(AbilitiesCatalogue))]
    public class CatalogueDrawer : TTitleDrawer
    {
        protected override string Title => "Abilities";

        protected override void CreateContent(VisualElement body, SerializedProperty property)
        {
            body.Add(new SpaceSmall());
            SerializedProperty items = property.FindPropertyRelative("m_Abilities");

            int itemsCount = items.arraySize;
            for (int i = 0; i < itemsCount; ++i)
            {
                SerializedProperty item = items.GetArrayElementAtIndex(i);
                PropertyField itemField = new PropertyField(item, string.Empty);

                itemField.SetEnabled(false);
                body.Add(itemField);
                body.Add(new SpaceSmaller());
            }
        }
    }
}