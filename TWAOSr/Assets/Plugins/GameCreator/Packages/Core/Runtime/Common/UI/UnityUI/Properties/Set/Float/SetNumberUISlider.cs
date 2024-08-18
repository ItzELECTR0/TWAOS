using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Slider")]
    [Category("UI/Slider")]
    
    [Description("Sets the Slider's min, max or current value")]
    [Image(typeof(IconUISlider), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetNumberUISlider : PropertyTypeSetNumber
    {
        private enum Property
        {
            Value,
            MinValue,
            MaxValue
        }

        [SerializeField] private Property m_Property = Property.Value;
        [SerializeField] private PropertyGetGameObject m_Slider = GetGameObjectInstance.Create();

        public override void Set(double value, Args args)
        {
            GameObject gameObject = this.m_Slider.Get(args);
            if (gameObject == null) return;

            Slider slider = gameObject.Get<Slider>();
            if (slider == null) return;

            switch (this.m_Property)
            {
                case Property.Value:
                    slider.value = (float) value;
                    break;
                
                case Property.MinValue:
                    slider.minValue = (float) value;
                    break;
                
                case Property.MaxValue:
                    slider.maxValue = (float) value;
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Slider.Get(args);
            if (gameObject == null) return default;

            Slider slider = gameObject.Get<Slider>();
            return this.m_Property switch
            {
                Property.Value => slider.value,
                Property.MinValue => slider.minValue,
                Property.MaxValue => slider.maxValue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberUISlider()
        );
        
        public override string String => string.Format(
            "{0} {1}", 
            this.m_Slider,
            TextUtils.Humanize(this.m_Property.ToString())
        );
    }
}