using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave
{
    public class ReIconedInstaller : AssetPostprocessor
    {

        #region Unity Methods

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.Contains("ReIconedInstaller.cs"))
                {
                    string[] lines = File.ReadAllLines(Application.dataPath + "/../ProjectSettings/ProjectVersion.txt");
                    for(int i=0; i < lines.Length; i++)
                    {
                        if(lines[i].IndexOf("m_EditorVersion", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            string version = lines[i].Substring(lines[i].IndexOf(" ")).Trim();
                            version = version.Substring(0, version.IndexOf('.'));
                            int vm;
                            if(int.TryParse(version, out vm) && vm >= 2019)
                            {
                                AssetDatabase.ImportPackage(Application.dataPath + "/NullSave/ReIconed 2/2019_Or_Newer_ReIconed.unitypackage", false);
                            }
                            break;
                        }
                    }

                    return;
                }
            }
        }

        #endregion

    }
}