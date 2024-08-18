using System;
using System.Collections.Generic;
using GameCreator.Editor.Installs;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    internal static class WelcomeCommands
    {
        private static readonly Dictionary<string, Action<string[]>> Commands;

        // INITIALIZERS: --------------------------------------------------------------------------

        static WelcomeCommands()
        {
            Commands = new Dictionary<string, Action<string[]>>
            {
                {"link", CommandLink},
                {"settings", CommandSettings},
                {"examples-manager", CommandExamplesManager}
            };
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static bool Parse(string[] steps)
        {
            if (steps.Length == 0) return false;
            if (Commands.TryGetValue(steps[0], out Action<string[]> callback))
            {
                callback.Invoke(steps);
                return true;
            }

            Debug.LogError($"Welcome command '{steps[0]}' not found");
            return false;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void CommandLink(string[] steps)
        {
            if (steps.Length < 2) return;
            Application.OpenURL(steps[1]);
        }
        
        private static void CommandSettings(string[] steps)
        {
            if (steps.Length < 2) return;
            SettingsWindow.OpenWindow(steps[1]);
        }
        
        private static void CommandExamplesManager(string[] steps)
        {
            InstallerManagerWindow.OpenWindow();
        }
    }
}