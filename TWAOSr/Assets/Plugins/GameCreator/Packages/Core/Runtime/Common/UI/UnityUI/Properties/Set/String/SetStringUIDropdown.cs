using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Dropdown")]
    [Category("UI/Dropdown")]
    
    [Description("Sets the Dropdown or TextMeshPro Dropdown text value")]
    [Image(typeof(IconUIDropdown), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetStringUIDropdown : PropertyTypeSetString
    {
        [SerializeField] private PropertyGetGameObject m_Dropdown = GetGameObjectInstance.Create();

        public override void Set(string value, Args args)
        {
            GameObject gameObject = this.m_Dropdown.Get(args);
            if (gameObject == null) return;

            Dropdown dropdown = gameObject.Get<Dropdown>();
            if (dropdown != null)
            {
                Dropdown.OptionData option = new Dropdown.OptionData(value);
                int index = dropdown.options.IndexOf(option);
                if (index >= 0) dropdown.value = index;
                return;
            }

            TMP_Dropdown dropdownTMP = gameObject.Get<TMP_Dropdown>();
            if (dropdownTMP != null)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(value);
                int index = dropdownTMP.options.IndexOf(option);
                if (index >= 0) dropdownTMP.value = index;
            }
        }

        public override string Get(Args args)
        {
            GameObject gameObject = this.m_Dropdown.Get(args);
            if (gameObject == null) return default;

            Dropdown dropdown = gameObject.Get<Dropdown>();
            if (dropdown != null) return dropdown.options[dropdown.value].text;

            TMP_Dropdown dropdownTMP = gameObject.Get<TMP_Dropdown>();
            return dropdownTMP != null 
                ? dropdownTMP.options[dropdown.value].text 
                : string.Empty;
        }

        public static PropertySetString Create => new PropertySetString(
            new SetStringUIDropdown()
        );
        
        public override string String => this.m_Dropdown.ToString();
    }
}