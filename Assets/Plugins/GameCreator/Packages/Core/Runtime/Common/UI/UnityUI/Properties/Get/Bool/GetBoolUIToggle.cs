using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Toggle")]
    [Category("UI/Toggle")]
    
    [Description("Gets the Toggle component on or off state")]
    [Image(typeof(IconUIToggle), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetBoolUIToggle : PropertyTypeGetBool
    {
        [SerializeField] private PropertyGetGameObject m_Toggle = GetGameObjectInstance.Create();

        public override bool Get(Args args)
        {
            GameObject gameObject = this.m_Toggle.Get(args);
            if (gameObject == null) return false;

            Toggle toggle = gameObject.Get<Toggle>();
            return toggle != null && toggle.isOn;
        }

        public static PropertyGetBool Create => new PropertyGetBool(
            new GetBoolUIToggle()
        );
        
        public override string String => this.m_Toggle.ToString();
    }
}