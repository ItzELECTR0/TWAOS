using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using GameCreator.Editor.Common;
using System.Text.RegularExpressions;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Hub
{
    internal static class Download
    {
        [Serializable]
        private struct SendDownload
        {
            public string username;
            public string passcode;
            public string packageId;
        }

        // CONSTANTS: -----------------------------------------------------------------------------

        private const string PATH_INSTRUCTIONS = "Instructions";
        private const string PATH_CONDITIONS = "Conditions";
        private const string PATH_EVENTS = "Events";
        private const string PATH_OTHERS = "Others";

        private const string CF_DOWNLOAD = "editorDownload";

        private static readonly TextInfo TEXT_INFO = new CultureInfo("en-US", false).TextInfo;
        private static readonly Regex REGEX_ALPHANUMERIC = new Regex("[^a-zA-Z0-9/]");

        private const string KEY_OPT_OUT_UPDATE_OLD_VERSION_DIALOG = "gchub:optout-auto-update";

        // EVENTS: --------------------------------------------------------------------------------

        public static event Action<bool> EventDownload;

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool Downloading { get; private set; }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static async Task<Package> Get(string packageID)
        {
            if (Downloading) return null;

            Downloading = true;
            EventDownload?.Invoke(true);

            SendDownload sendPost = new SendDownload
            {
                username = Auth.Username,
                passcode = Auth.Passcode,
                packageId = packageID
            };
            
            Http.ReceiveData response = await Http.Send(CF_DOWNLOAD, sendPost);
            
            Downloading = false;
            EventDownload?.Invoke(false);

            if (response.error)
            {
                EditorUtility.DisplayDialog(
                    "Error while downloading package",
                    response.data,
                    "Accept"
                );

                return null;
            }

            return JsonUtility.FromJson<Package>(response.data);
        }

        public static async Task<string> Install(string packageID, bool skipUpdateDialog = false)
        {
            Package package = await Get(packageID);
            if (package == null) return string.Empty;
            
            string path = GetAssetPath(package.type, package.category);
            string installPath = PathUtils.Combine("Assets", path);
            string absoluteFilepath = PathUtils.Combine(
                Application.dataPath,
                path,
                package.filename
            );

            if (File.Exists(PathUtils.PathForOS(absoluteFilepath)))
            {
                bool replace = skipUpdateDialog || EditorUtility.DisplayDialog(
                    "Another version of this package already exists",
                    "Do you want to update it?",
                    "Yes, update", "Cancel", 
                    DialogOptOutDecisionType.ForThisMachine, 
                    KEY_OPT_OUT_UPDATE_OLD_VERSION_DIALOG
                );

                if (!replace) return string.Empty;
            }
            else
            {
                string className = Path.GetFileNameWithoutExtension(package.filename);
                string[] candidatesGuids = AssetDatabase.FindAssets(className);

                foreach (string candidateGuid in candidatesGuids)
                {
                    string candidatePath = AssetDatabase.GUIDToAssetPath(candidateGuid);
                    int deleteOption = EditorUtility.DisplayDialogComplex(
                        $"This project already contains a script with the name '{className}'",
                        $"Do you want to delete the file at {candidatePath} before installing this package?",
                        "Yes, delete", "Cancel", "No, leave it"
                    );

                    switch (deleteOption)
                    {
                        case 0: AssetDatabase.DeleteAsset(candidatePath); break; // Yes, delete
                        case 1: return string.Empty; // Cancel
                    }
                }
            }

            DirectoryUtils.RequirePath(installPath);

            await File.WriteAllTextAsync(
                PathUtils.PathForOS(absoluteFilepath),
                package.content
            );

            AssetDatabase.Refresh();
            return PathUtils.Combine(installPath, package.filename);
        }

        public static string GetInstallationPath(string type, string category, string filename)
        {
            return PathUtils.Combine("Assets", GetAssetPath(type, category), filename);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static string GetAssetPath(string type, string category)
        {
            string path = type switch
            {
                "instruction" => PathUtils.Combine(GameCreatorHub.DOWNLOAD_PROJECT_PATH, PATH_INSTRUCTIONS),
                "condition" => PathUtils.Combine(GameCreatorHub.DOWNLOAD_PROJECT_PATH, PATH_CONDITIONS),
                "event" => PathUtils.Combine(GameCreatorHub.DOWNLOAD_PROJECT_PATH, PATH_EVENTS),
                _ => PathUtils.Combine(GameCreatorHub.DOWNLOAD_PROJECT_PATH, PATH_OTHERS)
            };

            string[] categories = category.Split('/');
            StringBuilder categoryBuilder = new StringBuilder();

            for (int i = 0; i < categories.Length - 1; ++i)
            {
                string section = TEXT_INFO.ToTitleCase(categories[i]).Replace(" ", string.Empty);
                section = REGEX_ALPHANUMERIC.Replace(section, string.Empty);
                
                categoryBuilder.Append(section).Append('/');
            }

            return PathUtils.Combine(path, categoryBuilder.ToString());
        }
    }
}