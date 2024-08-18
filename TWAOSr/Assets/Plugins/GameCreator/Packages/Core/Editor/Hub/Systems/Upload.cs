using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Hub
{
    internal static class Upload
    {
        [Serializable]
        private struct SendUpload
        {
            public string username;
            public string passcode;
            public Package package;
        }

        [Serializable]
        private struct RecvUpload
        {
            public string pid;
            public int code;
        }

        // CONSTANTS: -----------------------------------------------------------------------------

        private const string CF_UPLOAD = "editorUpload";

        private const string MENU_PATH_UPLOAD = "Assets/Game Creator/Upload to Game Creator Hub";
        private const int MENU_PRIORITY = 180;

        private static readonly Regex[] SKIP_PATHS =
        {
            new Regex(@"^Assets\/Plugins\/GameCreator\/Packages(\/.*)?$")
        };

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool Uploading { get; private set; }
        
        // EVENTS: --------------------------------------------------------------------------------

        public static event Action EventAfterUpload;

        // MENU: ----------------------------------------------------------------------------------

        [MenuItem(MENU_PATH_UPLOAD, priority = MENU_PRIORITY)]
        public static async void MenuUpload()
        {
            TextAsset textAsset = Selection.activeObject as TextAsset;
            if (!ValidateUpload(textAsset)) return;
            
            if (!Auth.IsAuthenticated)
            {
                HubSettingsWindow.OpenWindow();
                return;
            }

            Package package = GeneratePackage(textAsset);
            await Send(package);
        }

        [MenuItem(MENU_PATH_UPLOAD, validate = true)]
        public static bool MenuUploadValidate()
        {
            if (Selection.activeObject == null) return false;
            
            TextAsset textAsset = Selection.activeObject as TextAsset;
            return ValidateUpload(textAsset);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static async Task Send(TextAsset textAsset)
        {
            if (!ValidateUpload(textAsset)) return;
            
            if (!Auth.IsAuthenticated)
            {
                HubSettingsWindow.OpenWindow();
                return;
            }

            Package package = GeneratePackage(textAsset);
            await Send(package);
        }
        
        public static async Task Send(Package package)
        {
            if (Uploading) return;
            if (!Auth.IsAuthenticated) return;

            Uploading = true;
            EditorUtility.DisplayProgressBar(
                $"Uploading {package.name} to Game Creator Hub",
                "This should take less than a minute",
                0.1f + UnityEngine.Random.value * 0.85f
            );

            SendUpload sendData = new SendUpload
            {
                username = Auth.Username,
                passcode = Auth.Passcode,
                package = package
            };
            
            Http.ReceiveData response = await Http.Send(CF_UPLOAD, sendData);
            EditorUtility.ClearProgressBar();

            if (response.error)
            {
                EditorUtility.DisplayDialog(
                    "Error when uploading package",
                    response.data,
                    "Accept"
                );
            }
            else
            {
                RecvUpload recvData = JsonUtility.FromJson<RecvUpload>(response.data);
                string message = string.Empty;
                switch (recvData.code)
                {
                    case 100: message = "New package has been successfully created"; break;
                    case 101: message = "Package has been successfully updated"; break;
                }

                bool option = EditorUtility.DisplayDialog(
                    $"Uploading {package.name} complete",
                    message, "Ok", "Open on Game Creator Hub"
                );

                if (!option)
                {
                    string uri = GameCreatorHub.URI_PACKAGE + recvData.pid;
                    Application.OpenURL(uri);
                }
            }

            Uploading = false;
            EventAfterUpload?.Invoke();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Package GeneratePackage(TextAsset textAsset)
        {
            Type type = GetTypeFromTextAsset(textAsset);

            if (type == null) return null;
            return new Package
            {
                name = GetAttribute<TitleAttribute>(type)?.Title,
                description = GetAttribute<DescriptionAttribute>(type)?.Description,
                category = GetAttribute<CategoryAttribute>(type)?.ToString(),

                type = GetPackageType(type),
                user = string.Empty,

                filename = Path.ChangeExtension(textAsset.name, "cs"),
                content = textAsset.text,

                version = new Version
                {
                    x = GetAttribute<VersionAttribute>(type)?.X ?? default,
                    y = GetAttribute<VersionAttribute>(type)?.Y ?? default,
                    z = GetAttribute<VersionAttribute>(type)?.Z ?? default,
                },

                keywords = GetKeywords(type),
                dependencies = GetDependencies(type),
                parameters = GetParameters(type),
                examples = GetExamples(type),
                renderPipelines = new RenderPipelines
                {
                    builtin = GetAttribute<RenderPipelineAttribute>(type)?.Builtin ?? false,
                    urp = GetAttribute<RenderPipelineAttribute>(type)?.URP ?? false,
                    hdrp = GetAttribute<RenderPipelineAttribute>(type)?.HDRP ?? false,
                }
            };
        }
        
        private static bool ValidateUpload(TextAsset textAsset)
        {
            if (textAsset == null) return false;

            string textAssetPath = AssetDatabase.GetAssetPath(textAsset);
            foreach (Regex skipPathRegex in SKIP_PATHS)
            {
                if (skipPathRegex.IsMatch(textAssetPath))
                {
                    return false;
                }
            }

            Type type = GetTypeFromTextAsset(textAsset);
            return GetSupportedType(type) != string.Empty;
        }

        // PRIVATE UTILITIES: ---------------------------------------------------------------------

        private static Type[] GetTypesByName(string className)
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type assemblyType in assemblyTypes)
                {
                    if (assemblyType.Name == className)
                    {
                        types.Add(assemblyType);
                    }
                }
            }

            return types.ToArray();
        }
        
        private static Type GetTypeFromTextAsset(TextAsset textAsset)
        {
            Type[] typeCandidates = GetTypesByName(textAsset.name);
            return typeCandidates.Length >= 1 ? typeCandidates[0] : null;
        }

        private static string GetSupportedType(Type type)
        {
            if (type == null) return string.Empty;
            return GameCreatorHub.TYPES_SUPPORTED.Contains(type.Name) 
                ? type.Name 
                : GetSupportedType(type.BaseType);
        }

        private static T GetAttribute<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        private static string GetPackageType(Type type)
        {
            return GetSupportedType(type).ToLowerInvariant();
        }

        private static string[] GetKeywords(Type type)
        {
            List<KeywordsAttribute> keywordsAttributes = new List<KeywordsAttribute>(
                type.GetCustomAttributes<KeywordsAttribute>(true)
            );

            return keywordsAttributes
                .SelectMany(keywordsAttribute => keywordsAttribute.Keywords)
                .ToArray();
        }

        private static Dependency[] GetDependencies(Type type)
        {
            List<DependencyAttribute> dependenciesAttributes = new List<DependencyAttribute>(
                type.GetCustomAttributes<DependencyAttribute>(true)
            );

            return dependenciesAttributes.Select(
                dependencyAttribute => new Dependency(dependencyAttribute.ID, dependencyAttribute.Version)
            ).ToArray();
        }

        private static Parameter[] GetParameters(Type type)
        {
            List<ParameterAttribute> parametersAttributes = new List<ParameterAttribute>(
                type.GetCustomAttributes<ParameterAttribute>(true)
            );

            return parametersAttributes.Select(parameterAttribute => new Parameter(
                parameterAttribute.Name, 
                parameterAttribute.Description
            )).ToArray();
        }
        
        private static string[] GetExamples(Type type)
        {
            List<ExampleAttribute> examplesAttributes = new List<ExampleAttribute>(
                type.GetCustomAttributes<ExampleAttribute>(true)
            );

            return examplesAttributes
                .Select(exampleAttribute => exampleAttribute.Content)
                .ToArray();
        }
    }
}
