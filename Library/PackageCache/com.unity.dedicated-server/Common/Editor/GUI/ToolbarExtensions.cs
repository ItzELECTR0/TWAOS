using System;
using UnityEngine.UIElements;
using InternalManager = UnityEditor.Multiplayer.Internal.EditorMultiplayerManager;

namespace Unity.Multiplayer.Editor
{
    internal class ToolbarExtensions
    {
        public static event Action<VisualElement> CreatingPlayModeButtons
        {
            add => InternalManager.creatingPlayModeButtons += value;
            remove => InternalManager.creatingPlayModeButtons -= value;
        }
    }
}
