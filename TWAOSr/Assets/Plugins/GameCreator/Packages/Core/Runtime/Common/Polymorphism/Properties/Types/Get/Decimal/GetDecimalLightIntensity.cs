using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Intensity")]
    [Category("Light/Intensity")]
    
    [Image(typeof(IconLight), ColorTheme.Type.Yellow)]
    [Description("The intensity of a Light source")]

    [Keywords("Light", "Lux")]
    [Serializable]
    public class GetDecimalLightIntensity : PropertyTypeGetDecimal
    {
        [SerializeField] private PropertyGetGameObject m_Light = GetGameObjectInstance.Create();

        public override double Get(Args args)
        {
            Light light = this.m_Light.Get<Light>(args);
            return light != null ? light.intensity : 0;
        }

        public override string String => $"{this.m_Light}[Intensity]";
    }
}