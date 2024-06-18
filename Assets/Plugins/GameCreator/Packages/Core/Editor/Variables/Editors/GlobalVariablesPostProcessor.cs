using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;

namespace GameCreator.Editor.Variables
{
    public class GlobalVariablesPostProcessor : AssetPostprocessor
    {
        public static event Action EventRefresh;
        
        // PROCESSORS: ----------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SettingsWindow.InitRunners.Add(new InitRunner(
                SettingsWindow.INIT_PRIORITY_LOW,
                CanRefreshVariables,
                RefreshVariables
            ));
        }
        
        private static void OnPostprocessAllAssets(
            string[] importedAssets, 
            string[] deletedAssets, 
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0 && deletedAssets.Length == 0) return;
            RefreshVariables();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static bool CanRefreshVariables()
        {
            return true;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static void RefreshVariables()
        {
            string[] varSettingsGuids = AssetDatabase.FindAssets($"t:{nameof(VariablesSettings)}");
            if (varSettingsGuids.Length == 0) return;

            string varSettingsPath = AssetDatabase.GUIDToAssetPath(varSettingsGuids[0]);
            
            VariablesSettings varSettings = AssetDatabase.LoadAssetAtPath<VariablesSettings>(varSettingsPath);
            if (varSettings == null) return;

            string[] nameVariablesGuids = AssetDatabase.FindAssets($"t:{nameof(GlobalNameVariables)}");
            GlobalNameVariables[] nameVariables = new GlobalNameVariables[nameVariablesGuids.Length];
            
            for (int i = 0; i < nameVariablesGuids.Length; i++)
            {
                string nameVariablesGuid = nameVariablesGuids[i];
                string nameVariablesPath = AssetDatabase.GUIDToAssetPath(nameVariablesGuid);
                nameVariables[i] = AssetDatabase.LoadAssetAtPath<GlobalNameVariables>(nameVariablesPath);
            }
            
            string[] listVariablesGuids = AssetDatabase.FindAssets($"t:{nameof(GlobalListVariables)}");
            GlobalListVariables[] listVariables = new GlobalListVariables[listVariablesGuids.Length];

            for (int i = 0; i < listVariablesGuids.Length; i++)
            {
                string listVariablesGuid = listVariablesGuids[i];
                string listVariablesPath = AssetDatabase.GUIDToAssetPath(listVariablesGuid);
                listVariables[i] = AssetDatabase.LoadAssetAtPath<GlobalListVariables>(listVariablesPath);
            }

            SerializedObject varSettingsSerializedObject = new SerializedObject(varSettings);
            SerializedProperty globalVariablesProperty = varSettingsSerializedObject
                .FindProperty(TAssetRepositoryEditor.NAMEOF_MEMBER)
                .FindPropertyRelative("m_Variables");

            SerializedProperty nameVariablesProperty = globalVariablesProperty.FindPropertyRelative("m_NameVariables");
            SerializedProperty listVariablesProperty = globalVariablesProperty.FindPropertyRelative("m_ListVariables");
                
            nameVariablesProperty.arraySize = nameVariables.Length;
            for (int i = 0; i < nameVariables.Length; ++i)
            {
                nameVariablesProperty.GetArrayElementAtIndex(i).objectReferenceValue = nameVariables[i];
            }
            
            listVariablesProperty.arraySize = listVariables.Length;
            for (int i = 0; i < listVariables.Length; ++i)
            {
                listVariablesProperty.GetArrayElementAtIndex(i).objectReferenceValue = listVariables[i];
            }

            varSettingsSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            EventRefresh?.Invoke();
        }
    }
}
