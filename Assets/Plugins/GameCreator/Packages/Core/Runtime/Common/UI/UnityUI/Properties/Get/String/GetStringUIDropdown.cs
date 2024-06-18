using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Dropdown")]
    [Category("UI/Dropdown")]
    
    [Description("Gets the Dropdown or TextMeshPro Dropdown text value")]
    [Image(typeof(IconUIDropdown), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetStringUIDropdown : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetGameObject m_Dropdown = GetGameObjectInstance.Create();

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

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringUIDropdown()
        );
        
        public override string String => this.m_Dropdown.ToString();
    }
}