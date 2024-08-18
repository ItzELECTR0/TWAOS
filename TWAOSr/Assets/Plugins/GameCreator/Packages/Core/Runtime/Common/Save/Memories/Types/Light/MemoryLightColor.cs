using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconLight), ColorTheme.Type.Green)]
    
    [Title("Color")]
    [Category("Light/Color")]
    
    [Description("Remembers the color of a Light component")]

    [Serializable]
    public class MemoryLightColor : Memory
    {
        public override string Title => "Color";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            Light light = target.Get<Light>();
            return new TokenLightColor(light);
        }

        public override void OnRemember(GameObject target, Token token)
        {
            if (token is not TokenLightColor tokenColor) return;
            
            Light light = target.Get<Light>();
            if (light != null) light.color = tokenColor.Color;
        }
    }
}