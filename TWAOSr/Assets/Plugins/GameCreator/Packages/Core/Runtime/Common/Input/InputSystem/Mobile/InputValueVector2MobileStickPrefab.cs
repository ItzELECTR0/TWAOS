using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Custom Stick")]
    [Category("Mobile/Custom Stick")]
    
    [Description("")]
    [Image(typeof(IconTouchstick), ColorTheme.Type.Blue)]
    
    [Keywords("Virtual", "Joystick", "Touchstick", "Direction")]
    
    [Serializable]
    public class InputValueVector2MobileStickPrefab : TInputValueVector2MobileStick
    {
        [SerializeField] private TouchStickSkin m_Touchstick;
        
        protected override ITouchStick CreateTouchStick()
        {
            if (!this.m_Touchstick.HasValue) return null;
            
            GameObject prefab = this.m_Touchstick.Value;
            GameObject instance = UnityEngine.Object.Instantiate(prefab);
            return instance.GetComponentInChildren<ITouchStick>();
        }
    }
}