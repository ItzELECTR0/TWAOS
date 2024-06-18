using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    [CustomPropertyDrawer(typeof(GlobalVariables))]
    public class GlobalVariablesDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Button buttonRefresh = new Button(GlobalVariablesPostProcessor.RefreshVariables)
            {
                text = "Refresh",
                style = { height = 25 }
            };

            root.Add(new SpaceSmall());
            root.Add(buttonRefresh);
            root.Add(new SpaceSmall());
            
            SerializedProperty nameVariables = property.FindPropertyRelative("m_NameVariables");
            SerializedProperty listVariables = property.FindPropertyRelative("m_ListVariables");

            ContentBox boxNameVariables = new ContentBox("Global Name Variables", true);
            ContentBox boxListVariables = new ContentBox("Global List Variables", true);
            
            this.PaintVariables(nameVariables, boxNameVariables);
            this.PaintVariables(listVariables, boxListVariables);

            root.Add(boxNameVariables);
            root.Add(new SpaceSmallest());
            root.Add(boxListVariables);

            GlobalVariablesPostProcessor.EventRefresh += () =>
            {
                this.PaintVariables(nameVariables, boxNameVariables);
                this.PaintVariables(listVariables, boxListVariables);
            };

            return root;
        }

        private void PaintVariables(SerializedProperty property, ContentBox box)
        {
            box.Content.Clear();

            property.serializedObject.Update();
            int itemsCount = property.arraySize;

            for (int i = 0; i < itemsCount; ++i)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                ObjectField itemField = new ObjectField
                {
                    label = string.Empty,
                    value = item.objectReferenceValue
                };

                itemField.SetEnabled(false);
                box.Content.Add(itemField);
                
                if (i < itemsCount - 1)
                {
                    box.Content.Add(new SpaceSmaller());
                }
            }
        }
    }
}