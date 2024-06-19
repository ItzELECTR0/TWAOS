using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.Collections.Generic;
using UnityEditor.Build.Profile;
using Unity.DedicatedServer.Editor.Internal;

namespace Unity.Multiplayer.Editor
{
    [FilePath("ProjectSettings/Packages/com.unity.dedicated-server/MultiplayerRolesSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class MultiplayerRolesSettings : ScriptableSingleton<MultiplayerRolesSettings>
    {
        private class SaveAssetsProcessor : AssetModificationProcessor
        {
            static string[] OnWillSaveAssets(string[] paths)
            {
                MultiplayerRolesSettings.instance.SaveIfDirty();
                return paths;
            }
        }

        static private MultiplayerRoleFlags GetDefaultMultiplayerRoleForBuildProfile(BuildProfile profile)
            => InternalUtility.IsServerProfile(profile) ? MultiplayerRoleFlags.Server : MultiplayerRoleFlags.Client;

        static private MultiplayerRoleFlags GetDefaultMultiplayerRoleForBuildTarget(NamedBuildTarget namedBuildTarget)
            => namedBuildTarget == NamedBuildTarget.Server ? MultiplayerRoleFlags.Server : MultiplayerRoleFlags.Client;

        // The key used for classic profiles is the platform id.
        [SerializeField] private SerializedDictionary<string, MultiplayerRoleFlags> m_MultiplayerRoleForClassicProfile = new();

        // This is a SerializedDictionary even as a private field so it persists domain reloads.
        private SerializedDictionary<BuildProfile, MultiplayerRoleData> m_MultiplayerRoleForBuildProfile = new(new InstanceIdComparer<BuildProfile>());
        private class InstanceIdComparer<T> : Comparer<T> where T : UnityEngine.Object
        {
            public override int Compare(T x, T y) => x.GetInstanceID().CompareTo(y.GetInstanceID());
        }

        internal void SaveIfDirty()
        {
            if (EditorUtility.IsDirty(this))
                Save(true);
        }

        private static MultiplayerRoleData GetOrCreateRoleDataForBuildProfile(BuildProfile profile)
        {
            if (instance.m_MultiplayerRoleForBuildProfile.TryGetValue(profile, out var data) && data != null)
                return data;

            var assetPath = AssetDatabase.GetAssetPath(profile);
            data = assetPath != null ? AssetDatabase.LoadAssetAtPath<MultiplayerRoleData>(assetPath) : null;

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MultiplayerRoleData>();
                data.name = "Multiplayer Role Data";
                data.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                data.multiplayerRole = GetDefaultMultiplayerRoleForBuildProfile(profile);
            }

            instance.m_MultiplayerRoleForBuildProfile[profile] = data;
            return data;
        }

        public MultiplayerRoleFlags GetMultiplayerRoleForBuildProfile(BuildProfile profile)
        {
            if (InternalUtility.IsClassicProfile(profile))
            {
                var key = InternalUtility.GetUniqueKeyForClassicProfile(profile);
                if (m_MultiplayerRoleForClassicProfile.TryGetValue(key, out var mask))
                    return mask;

                return GetDefaultMultiplayerRoleForBuildProfile(profile);
            }

            return GetOrCreateRoleDataForBuildProfile(profile).multiplayerRole;
        }

        public void SetMultiplayerRoleForBuildProfile(BuildProfile profile, MultiplayerRoleFlags mask)
        {
            if (GetMultiplayerRoleForBuildProfile(profile) == mask)
                return;

            if (InternalUtility.IsClassicProfile(profile))
            {
                var key = InternalUtility.GetUniqueKeyForClassicProfile(profile);

                if (GetDefaultMultiplayerRoleForBuildProfile(profile) == mask)
                    m_MultiplayerRoleForClassicProfile.Remove(key);
                else
                    m_MultiplayerRoleForClassicProfile[key] = mask;

                EditorUtility.SetDirty(this);
                return;
            }

            var data = GetOrCreateRoleDataForBuildProfile(profile);
            data.multiplayerRole = mask;

            if (GetDefaultMultiplayerRoleForBuildProfile(profile) == mask)
                AssetDatabase.RemoveObjectFromAsset(data);
            else if (string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(data)))
                AssetDatabase.AddObjectToAsset(data, profile);

            EditorUtility.SetDirty(data);
        }
    }
}
