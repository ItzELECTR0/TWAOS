using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Multiplayer.Internal;
using System;
using UnityEditor.Build.Profile;

namespace Unity.Multiplayer.Editor
{
    internal static class BuildOptionsExtensions
    {
        private static List<IMultiplayerBuildOptionsSection> s_BuildOptionsSections;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            s_BuildOptionsSections = TypeCache.GetTypesDerivedFrom<IMultiplayerBuildOptionsSection>()
                .Select(t => (IMultiplayerBuildOptionsSection)Activator.CreateInstance(t))
                .OrderBy(s => s.Order)
                .ToList();

            EditorMultiplayerManager.drawingMultiplayerBuildOptionsForBuildProfile += OnDrawingBuildOptions;
        }

        private static void OnDrawingBuildOptions(BuildProfile profile)
        {
            foreach (var section in s_BuildOptionsSections)
            {
                section.DrawBuildOptions(profile);
            }
        }
    }
}
