using UnityEngine;

using InternalManager = UnityEngine.Multiplayer.Internal.MultiplayerManager;

namespace Unity.Multiplayer
{
    /// <summary>
    /// Provides an api for managing multiplayer roles in runtime.
    /// </summary>
    public static class MultiplayerRolesManager
    {
        /// <summary>
        /// Gets the active multiplayer role mask.
        /// </summary>
        public static MultiplayerRoleFlags ActiveMultiplayerRoleMask
            => (MultiplayerRoleFlags)InternalManager.activeMultiplayerRoleMask;

        /// <summary>
        /// Gets the multiplayer role mask for a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject.</param>
        /// <returns>Returns the multiplayer role mask for the provided GameObject.</returns>
        public static MultiplayerRoleFlags GetMultiplayerRoleMaskForGameObject(GameObject gameObject)
            => (MultiplayerRoleFlags)InternalManager.GetMultiplayerRoleMaskForGameObject(gameObject);

        /// <summary>
        /// Gets the multiplayer role mask for a Component.
        /// </summary>
        /// <param name="component">The Component.</param>
        /// <returns>Returns the multiplayer role mask for the provided Component.</returns>
        public static MultiplayerRoleFlags GetMultiplayerRoleMaskForComponent(Component component)
            => (MultiplayerRoleFlags)InternalManager.GetMultiplayerRoleMaskForComponent(component);
    }
}
