using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Editor
{
    internal class ProjectSettingsProvider : SettingsProvider
    {
        internal const string k_SettingsGroupPath = "Project/Multiplayer/";
        private static readonly string[] k_Keywords = new string[]
        {
            "Multiplayer",
            "Server",
        };

        [SettingsProviderGroup]
        public static SettingsProvider[] CreateDedicatedServerSettingsProvider()
        {
            var paths = TypeCache.GetTypesWithAttribute<ProjectSettingsSectionAttribute>()
                .Select
                (
                    t => t.GetCustomAttributes(typeof(ProjectSettingsSectionAttribute), true)
                    .Cast<ProjectSettingsSectionAttribute>()
                    .First()
                    .SettingsPath
                )
                .Distinct();

            var settingsProviders = new List<SettingsProvider>();

            foreach (var path in paths)
            {
                settingsProviders.Add(new ProjectSettingsProvider(path));
            }

            return settingsProviders.ToArray();
        }

        private string m_Path;

        public ProjectSettingsProvider(string path) : base(path, SettingsScope.Project)
        {
            keywords = new HashSet<string>(k_Keywords);
            activateHandler = ActivateHandler;
        }

        private void ActivateHandler(string searchContext, VisualElement root)
        {
            var container = new VisualElement();
            container.AddToClassList("dedicated-server-settings-container");
            container.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.dedicated-server/Common/Editor/ServerSettings/DedicatedServerSettings.uss"));

            var header = new Label(Path.GetFileName(this.settingsPath));
            header.AddToClassList("dedicated-server-settings-header");

            container.Add(header);

            var types = GetAllSectionTypes();
            foreach (var type in types)
            {
                Assert.IsTrue(type.IsSubclassOf(typeof(VisualElement)));

                var sectionAttribute = type.GetCustomAttributes(typeof(ProjectSettingsSectionAttribute), true).Cast<ProjectSettingsSectionAttribute>().First();

                var constructor = type.GetConstructor(new Type[0]);
                Assert.IsNotNull(constructor, $"The type {type} must have a parameterless constructor");

                if (sectionAttribute.Label != null)
                {
                    var title = new Label(sectionAttribute.Label);
                    title.AddToClassList("dedicated-server-settings-header2");
                    container.Add(title);
                }

                var section = (VisualElement)constructor.Invoke(null);
                container.Add(section);
            }

            root.Add(container);
        }

        private IEnumerable<Type> GetAllSectionTypes()
        {
            return TypeCache.GetTypesWithAttribute<ProjectSettingsSectionAttribute>()
                .Where(t => t.GetCustomAttributes(typeof(ProjectSettingsSectionAttribute), true).Cast<ProjectSettingsSectionAttribute>().First().SettingsPath == settingsPath);
        }
    }
}
