using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    [CreateAssetMenu(menuName = "Game Creator/Settings/Cursor", fileName = "Cursor_CURSORNAME")]
    public class CursorSetting : ScriptableObject
    {
        public Texture2D Texture;
        public Vector2 Hotspot;
        public CursorMode Mode = CursorMode.Auto;
    }
}