using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class CursorHelper
    {
        private static CursorType m_CurrentCursor = CursorType.None;
        
        public static void SetCursor(CursorType cursorType)
        {
            if (m_CurrentCursor == cursorType) return;

            var cursor = Resources.Load<CursorSetting>("Cursors/Cursor_" + cursorType);
            if (cursor == null)
            {
                Debug.LogWarning($"No cursor [{cursorType}] found in resources folder");
                return;
            }
            
            Cursor.SetCursor(
                cursor.Texture,
                cursor.Hotspot, 
                cursor.Mode
            );

            m_CurrentCursor = cursorType;
        }
    }
}