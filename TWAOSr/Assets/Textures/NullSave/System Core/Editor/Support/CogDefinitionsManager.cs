#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace NullSave
{
    [InitializeOnLoad]
    public class CogDefinitionsManager
    {

        #region Constructor

        static CogDefinitionsManager()
        {
            CreateDefinitions();
        }

        #endregion

        #region Public Methods

        public static void CreateDefinitions()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            List<string> allNullSaveDefines = new List<string>();

            var denitionsType = GetAllDefinitions();
            foreach (var t in denitionsType)
            {
                var value = t.InvokeMember(null, BindingFlags.DeclaredOnly |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);

                List<string> list = null;
                try
                {
                    list = (List<string>)t.InvokeMember("GetSymbols", BindingFlags.DeclaredOnly |
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, value, null);
                    if (list != null)
                    {
                        allNullSaveDefines.AddRange(list.Except(allNullSaveDefines));
                    }
                }
                catch { }
            }

            var allDefiniesToAdd = allNullSaveDefines.FindAll(s => !allDefines.Contains(s));
            if (allDefiniesToAdd.Count > 0)
            {
                AddDefinitionSymbols(allDefiniesToAdd, allDefines);
            }

        }

        #endregion

        #region Private Methods

        static void AddDefinitionSymbols(List<string> targetDefineSymbols, List<string> currentDefineSymbols)
        {
            bool needUpdate = false;
            for (int i = 0; i < targetDefineSymbols.Count; i++)
            {
                if (!currentDefineSymbols.Contains(targetDefineSymbols[i]))
                {
                    needUpdate = true; break;
                }
            }
            currentDefineSymbols.AddRange(targetDefineSymbols.Except(currentDefineSymbols));
            if (needUpdate)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", currentDefineSymbols.ToArray()));
        }

        static List<Type> GetAllDefinitions()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(CogDefinitionSymbol).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
        }

        #endregion

    }
}
#endif