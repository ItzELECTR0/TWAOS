using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Cursor")]
    [Image(typeof(IconCursor), ColorTheme.Type.Yellow)]
    
    [Category("UI/Cursor")]
    [Description("Changes the cursor image when hovering the Hotspot")]

    [Serializable]
    public class SpotCursor : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected PropertyGetTexture m_Texture = new PropertyGetTexture();
        [SerializeField] protected PropertyGetPosition m_Origin = GetPositionVector2.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Change Cursor to {this.m_Texture}";
        
        [field: NonSerialized] private bool IsPointerHovering { get; set; }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnPointerEnter(Hotspot hotspot)
        {
            base.OnPointerEnter(hotspot);
            
            this.IsPointerHovering = true;
            this.RefreshCursor(hotspot.IsActive && this.IsPointerHovering, hotspot.Args);
        }

        public override void OnPointerExit(Hotspot hotspot)
        {
            base.OnPointerExit(hotspot);
            
            this.IsPointerHovering = false;
            this.RefreshCursor(hotspot.IsActive && this.IsPointerHovering, hotspot.Args);
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);

            if (hotspot.IsActive && this.IsPointerHovering)
            {
                this.RefreshCursor(false, hotspot.Args);
            }
            
            this.IsPointerHovering = false;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshCursor(bool customCursor, Args args)
        {
            switch (customCursor)
            {
                case true:
                    Texture2D texture = this.m_Texture.Get(args) as Texture2D;
                    Vector3 origin = this.m_Origin.Get(args).XY();
                    Cursor.SetCursor(texture, origin, CursorMode.Auto);
                    break;
                
                case false:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }
        }
    }
}