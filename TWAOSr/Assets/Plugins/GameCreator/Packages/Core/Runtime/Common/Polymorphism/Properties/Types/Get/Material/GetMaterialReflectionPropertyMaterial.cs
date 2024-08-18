using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Property Material")]
    [Category("Reflection/Property Material")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Description("A 'Material' value of a property of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetMaterialReflectionPropertyMaterial : PropertyTypeGetMaterial
    {
        [SerializeField] private ReflectionPropertyMaterial m_Property = new ReflectionPropertyMaterial();

        public override Material Get(Args args) => this.m_Property.Value;

        public override string String => this.m_Property.ToString();
    }
}