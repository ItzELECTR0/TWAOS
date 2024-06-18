using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Installs
{
    [CustomPropertyDrawer(typeof(Version))]
    public class VersionDrawer : PropertyDrawer
    {
        private const string M_X = "major";
        private const string M_Y = "minor";
        private const string M_Z = "patch";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Vector3IntField fieldVector3 = new Vector3IntField("Version");
            fieldVector3.value = new Vector3Int(
                property.FindPropertyRelative(M_X).intValue,
                property.FindPropertyRelative(M_Y).intValue,
                property.FindPropertyRelative(M_Z).intValue
            );

            IntegerField fieldX = fieldVector3.Q<IntegerField>("unity-x-input");
            IntegerField fieldY = fieldVector3.Q<IntegerField>("unity-y-input");
            IntegerField fieldZ = fieldVector3.Q<IntegerField>("unity-z-input");

            fieldX.label = "M";
            fieldY.label = "M";
            fieldZ.label = "P";

            fieldVector3.RegisterValueChangedCallback(changeEvent =>
            {
                property.serializedObject.Update();
                
                property.FindPropertyRelative(M_X).intValue = changeEvent.newValue.x;
                property.FindPropertyRelative(M_Y).intValue = changeEvent.newValue.y;
                property.FindPropertyRelative(M_Z).intValue = changeEvent.newValue.z;

                property.serializedObject.ApplyModifiedProperties();
            });

            return fieldVector3;
        }
    }
}