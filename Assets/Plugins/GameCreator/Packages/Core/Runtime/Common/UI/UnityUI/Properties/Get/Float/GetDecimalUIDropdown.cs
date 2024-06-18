using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Dropdown")]
    [Category("UI/Dropdown")]
    
    [Description("Gets the Dropdown or TextMeshPro Dropdown selected index option")]
    [Image(typeof(IconUIDropdown), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetDecimalUIDropdown : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetGameObject m_Dropdown = GetGameObjectInstance.Create();

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Dropdown.Get(args);
            if (gameObject == null) return 0f;

            Dropdown dropdown = gameObject.Get<Dropdown>();
            if (dropdown != null) return dropdown.value;

            TMP_Dropdown dropdownTMP = dropdown.Get<TMP_Dropdown>();
            return dropdownTMP != null ? dropdownTMP.value : 0f;
        }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalUIDropdown()
        );

        public override string String => this.m_Dropdown.ToString();
    }
}