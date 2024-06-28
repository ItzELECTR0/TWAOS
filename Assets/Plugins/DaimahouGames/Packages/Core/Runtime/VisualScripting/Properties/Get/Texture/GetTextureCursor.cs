using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    [Title("Cursor")]
    [Category("Cursor")]
    
    [Image(typeof(IconCursor), ColorTheme.Type.Blue)]
    [Description("A reference to a Cursor asset")]

    [Serializable] [HideLabelsInEditor]
    public class GetTextureCursor : PropertyTypeGetTexture
    {
        [SerializeField] private CursorSetting m_Cursor;

        public override Texture Get(Args args) => m_Cursor != null ? m_Cursor.Texture : default;
        public override Texture Get(GameObject gameObject) => m_Cursor != null ? m_Cursor.Texture : default;

        public override string String => m_Cursor != null
            ? m_Cursor.name
            : "(none)";
    }
}