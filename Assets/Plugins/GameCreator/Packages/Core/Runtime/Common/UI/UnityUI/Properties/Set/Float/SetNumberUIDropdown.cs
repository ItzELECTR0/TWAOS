using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Dropdown")]
    [Category("UI/Dropdown")]
    
    [Description("Sets the Dropdown or TextMeshPro Dropdown selected index option")]
    [Image(typeof(IconUIDropdown), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetNumberUIDropdown : PropertyTypeSetNumber
    {
        [SerializeField] private PropertyGetGameObject m_Dropdown = GetGameObjectInstance.Create();

        public override void Set(double value, Args args)
        {
            GameObject gameObject = this.m_Dropdown.Get(args);
            if (gameObject == null) return;

            Dropdown dropdown = gameObject.Get<Dropdown>();
            if (dropdown != null)
            {
                dropdown.value = (int) Math.Floor(value);
                return;
            }

            TMP_Dropdown dropdownTMP = gameObject.Get<TMP_Dropdown>();
            if (dropdownTMP != null) dropdownTMP.value = (int) Math.Floor(value);
        }

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Dropdown.Get(args);
            if (gameObject == null) return default;

            Dropdown dropdown = gameObject.Get<Dropdown>();
            if (dropdown != null) return dropdown.value;

            TMP_Dropdown dropdownTMP = gameObject.Get<TMP_Dropdown>();
            return dropdownTMP != null ? dropdownTMP.value : 0f;
        }

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberUIDropdown()
        );
        
        public override string String => this.m_Dropdown.ToString();
    }
}