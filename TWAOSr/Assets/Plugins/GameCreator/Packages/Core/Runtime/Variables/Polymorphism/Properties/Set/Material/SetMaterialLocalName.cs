using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Description("Sets the Material value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetMaterialLocalName : PropertyTypeSetMaterial
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueMaterial.TYPE_ID);

        public override void Set(Material value, Args args) => this.m_Variable.Set(value, args);
        public override Material Get(Args args) => this.m_Variable.Get(args) as Material;

        public static PropertySetMaterial Create => new PropertySetMaterial(
            new SetMaterialLocalName()
        );

        public override string String => this.m_Variable.ToString();
    }
}
