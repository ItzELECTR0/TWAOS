using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Field Material")]
    [Category("Reflection/Field Material")]

    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    [Description("A 'Material' value of a public or private field of a component")]

    [Serializable]
    public class SetMaterialReflectionFieldMaterial : PropertyTypeSetMaterial
    {
        [SerializeField] private ReflectionFieldMaterial m_Field = new ReflectionFieldMaterial();

        public override void Set(Material value, Args args) => this.m_Field.Value = value;
        public override Material Get(Args args) => this.m_Field.Value;

        public override string String => this.m_Field.ToString();
    }
}