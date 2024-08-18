using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconLight), ColorTheme.Type.Green)]
    
    [Title("Intensity")]
    [Category("Light/Intensity")]
    
    [Description("Remembers the intensity of a Light component")]

    [Serializable]
    public class MemoryLightIntensity : Memory
    {
        public override string Title => "Intensity";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            Light light = target.Get<Light>();
            return new TokenLightIntensity(light);
        }

        public override void OnRemember(GameObject target, Token token)
        {
            if (token is not TokenLightIntensity tokenIntensity) return;
            
            Light light = target.Get<Light>();
            if (light != null) light.intensity = tokenIntensity.Intensity;
        }
    }
}