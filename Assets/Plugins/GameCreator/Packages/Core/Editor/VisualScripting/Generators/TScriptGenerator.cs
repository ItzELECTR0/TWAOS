using System.IO;
using System.Text;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace GameCreator.Editor.VisualScripting
{
    internal abstract class TScriptGenerator
    {
        internal const string MENU_PATH = "Assets/Create/Game Creator/Developer/";
        
        private const string PATH_TEMPLATES = "Generators/Templates/";
        
        private const string NAMESPACE_TOKEN = "{{NAMESPACE}}";
        private const string CLASSNAME_TOKEN = "{{CLASSNAME}}";
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        public static void CreateScript(string templateName, string packageName, string fileName)
        {
            string selectionPath = GetSelectionPath();
            string[] selectionSplit = selectionPath.Split('/');

            string templatePath = Path.GetFullPath(PathUtils.Combine(
                EditorPaths.VISUAL_SCRIPTING, 
                PATH_TEMPLATES, 
                selectionSplit.Length > 1 && selectionSplit[0] == "Packages"
                    ? packageName
                    : templateName
            ));

            Texture2D icon = Path.GetExtension(fileName) switch
            {
                ".cs" => EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                _ => EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D
            };

            NameEditScript nameEditScript = ScriptableObject.CreateInstance<NameEditScript>();
            nameEditScript.Namespace = selectionSplit.Length > 1 
                ? selectionSplit[1]
                : string.Empty;
            
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0, nameEditScript, fileName,
                icon, templatePath
            );

            AssetDatabase.Refresh();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static string GetSelectionPath()
        {
            string path = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(PathUtils.PathForOS(path))) path = Path.GetDirectoryName(path);

                break;
            }

            return PathUtils.PathToUnix(path);
        }
        
        // EDIT NAME CLASS: -----------------------------------------------------------------------

        internal class NameEditScript : EndNameEditAction
        {
            public string Namespace { private get; set; } = string.Empty;
            
            public override void Action(int instanceId, string filePath, string resourceFilePath)
            {
                string fullFilePath = Path.GetFullPath(filePath);
                string content = File.ReadAllText(PathUtils.PathForOS(resourceFilePath));

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                fileName = TextUtils.ProcessScriptName(fileName);

                content = content.Replace(CLASSNAME_TOKEN, fileName);
                content = content.Replace(NAMESPACE_TOKEN, string.IsNullOrEmpty(this.Namespace) 
                    ? "GameCreator" 
                    : TextUtils.ProcessNamespace(this.Namespace)
                );
			
                UTF8Encoding encoding = new UTF8Encoding(true);
                File.WriteAllText(PathUtils.PathForOS(fullFilePath), content, encoding);

                AssetDatabase.ImportAsset(filePath);
                
                Object asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                ProjectWindowUtil.ShowCreatedAsset(asset);
            }
        }
    }
}
