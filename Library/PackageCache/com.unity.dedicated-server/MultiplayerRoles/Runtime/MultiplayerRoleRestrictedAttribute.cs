using System;

namespace Unity.Multiplayer
{
    /// <summary>
    /// Marks a MonoBehaviour as multiplayer role restricted so it cannot be
    /// stripped in Server or Client platforms.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class MultiplayerRoleRestrictedAttribute : Attribute
    {
    }
}
