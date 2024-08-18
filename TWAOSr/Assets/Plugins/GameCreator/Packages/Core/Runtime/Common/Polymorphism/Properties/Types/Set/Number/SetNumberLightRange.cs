using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Range")]
    [Category("Light/Range")]

    [Image(typeof(IconLight), ColorTheme.Type.Yellow)]
    [Description("The range of a Light source component")]

    [Serializable]
    public class SetNumberLightRange : PropertyTypeSetNumber
    {
        [SerializeField] private PropertyGetGameObject m_Light = GetGameObjectInstance.Create();

        public override void Set(double value, Args args)
        {
            Light light = this.m_Light.Get<Light>(args);
            if (light != null) light.range = (float) value;
        }

        public override double Get(Args args)
        {
            Light light = this.m_Light.Get<Light>(args);
            return light != null ? light.range : 0;
        }

        public override string String => $"{this.m_Light}[Range]";
    }
}