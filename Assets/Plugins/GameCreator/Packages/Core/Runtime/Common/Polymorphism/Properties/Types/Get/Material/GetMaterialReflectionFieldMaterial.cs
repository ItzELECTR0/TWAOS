using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Material")]
    [Category("Reflection/Field Material")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Material' value of a public or private field of a component")]

    [Keywords("Component", "Script", "Property", "Member", "Variable", "Value")]
    
    [Serializable]
    public class GetMaterialReflectionFieldMaterial : PropertyTypeGetMaterial
    {
        [SerializeField] private ReflectionFieldMaterial m_Field = new ReflectionFieldMaterial();

        public override Material Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}