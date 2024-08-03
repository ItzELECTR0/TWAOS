#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace NullSave
{
    public static class EditorHelper
    {

        #region Variables

        private static Color proColor = new Color(0.9f, 0.9f, 0.9f, 1);
        private static Color freeColor = new Color(0.1f, 0.1f, 0.1f, 1);

        #endregion

        #region Properties

        public static Color EditorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin) return proColor;
                return freeColor;
            }
        }

        #endregion

        #region Public Methods

        public static Texture2D GetIcon(string name, string path)
        {
            return (Texture2D)Resources.Load(path, typeof(Texture2D));
        }

        public static int SimpleInt(SerializedObject serializedObject, string propertyName)
        {
            return serializedObject.FindProperty(propertyName).intValue;
        }

        public static void SimpleInt(SerializedObject serializedObject, string propertyName, int value)
        {
            serializedObject.FindProperty(propertyName).intValue = value;
        }

        public static int SimpleInt(SerializedProperty property, string propertyName)
        {
            return property.FindPropertyRelative(propertyName).intValue;
        }

        public static void SimpleInt(SerializedProperty property, string propertyName, int value)
        {
            property.FindPropertyRelative(propertyName).intValue = value;
        }

        #endregion

    }
}

#endif