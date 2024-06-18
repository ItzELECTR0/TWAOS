using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    
    public class ImageAttribute : Attribute
    {
        private readonly IIcon m_Icon;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public Texture2D Image => this.m_Icon.Texture;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ImageAttribute(Type iconType, ColorTheme.Type color)
            : this(iconType, ColorTheme.Get(color))
        { }

        public ImageAttribute(Type iconType, Color color)
            : this(iconType, color, null)
        { }
        
        public ImageAttribute(Type iconType, ColorTheme.Type iconColor, Type overlayType)
            : this(iconType, ColorTheme.Get(iconColor), overlayType)
        { }
        
        public ImageAttribute(Type iconType, Color iconColor, Type overlayType)
        {
            IIcon overlay = overlayType != null
                ? Activator.CreateInstance(overlayType, Color.white, null) as IIcon
                : null;
            
            this.m_Icon = Activator.CreateInstance(iconType, iconColor, overlay) as IIcon;
        }
    }   
}
