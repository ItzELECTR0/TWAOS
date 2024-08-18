using System;
using UnityEditor;

namespace GameCreator.Editor.Search
{
    internal static class Favorites
    {
        public const int BOOST = 2;
        
        // BOOST METHODS: -------------------------------------------------------------------------

        public static bool IsFavorite(Type type)
        {
            string key = GetKey(type);
            return EditorPrefs.GetBool(key, false);
        }

        public static void ToggleFavorite(Type type)
        {
            string key = GetKey(type);
            bool value = EditorPrefs.GetBool(key, false);
            EditorPrefs.SetBool(key, !value);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static string GetKey(Type type)
        {
            return $"gc-favorite:{type}";
        }
    }
}