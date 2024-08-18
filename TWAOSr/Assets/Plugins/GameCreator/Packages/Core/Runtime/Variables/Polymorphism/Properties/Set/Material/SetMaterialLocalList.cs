using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]

    [Description("Sets the Material value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetMaterialLocalList : PropertyTypeSetMaterial
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueMaterial.TYPE_ID);

        public override void Set(Material value, Args args) => this.m_Variable.Set(value, args);
        public override Material Get(Args args) => this.m_Variable.Get(args) as Material;

        public static PropertySetMaterial Create => new PropertySetMaterial(
            new SetMaterialLocalList()
        );

        public override string String => this.m_Variable.ToString();
    }
}
