using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public static class EditorSettingsRegistrar
    {
        private const string PATH = "Project/Game Creator";
        private const string LABEL_ROOT = "Game Creator";

        private static readonly HashSet<string> Settings = new HashSet<string>();
        
        // ROOT SETTINGS: -------------------------------------------------------------------------
        
        [SettingsProvider]
        private static SettingsProvider CreateRootSettings()
        {
            return new SettingsProvider(PATH, SettingsScope.Project)
            {
                label = LABEL_ROOT,
                activateHandler = (_, content) =>
                {
                    content.Add(new LabelTitle("Game Creator Settings"));
                    foreach (string name in Settings)
                    {
                        Button button = new Button(() =>
                        {
                            string path = $"{PATH}/{name}";
                            SettingsService.OpenProjectSettings(path);
                        })
                        { text = name };
                        content.Add(button);
                    }
                }
            };
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static SettingsProvider CreateSettings(string label, Action<string, VisualElement> callback)
        {
            return new SettingsProvider($"{PATH}/{label}", SettingsScope.Project)
            {
                label = label,
                activateHandler = callback
            };
        }

        public static void RegisterSettings(string name)
        {
            if (Settings.Contains(name)) return;
            Settings.Add(name);
        }
    }
}