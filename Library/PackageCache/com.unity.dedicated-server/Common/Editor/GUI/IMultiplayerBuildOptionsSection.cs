using UnityEditor.Build.Profile;

namespace Unity.Multiplayer.Editor
{
    internal interface IMultiplayerBuildOptionsSection
    {
        int Order { get; }
        void DrawBuildOptions(BuildProfile profile);
    }
}
