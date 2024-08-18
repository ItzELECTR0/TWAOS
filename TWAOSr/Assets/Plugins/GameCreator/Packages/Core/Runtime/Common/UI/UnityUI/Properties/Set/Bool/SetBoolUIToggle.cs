using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Toggle")]
    [Category("UI/Toggle")]
    
    [Description("Sets the Toggle component on or off")]
    [Image(typeof(IconUIToggle), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetBoolUIToggle : PropertyTypeSetBool
    {
        [SerializeField] private PropertyGetGameObject m_Toggle = GetGameObjectInstance.Create();

        public override void Set(bool value, Args args)
        {
            GameObject gameObject = this.m_Toggle.Get(args);
            if (gameObject == null) return;

            Toggle toggle = gameObject.Get<Toggle>();
            if (toggle == null) return;

            toggle.isOn = value;
        }

        public override bool Get(Args args)
        {
            GameObject gameObject = this.m_Toggle.Get(args);
            if (gameObject == null) return false;

            Toggle toggle = gameObject.Get<Toggle>();
            return toggle != null && toggle.isOn;
        }

        public static PropertySetBool Create => new PropertySetBool(
            new SetBoolUIToggle()
        );
        
        public override string String => this.m_Toggle.ToString();
    }
}