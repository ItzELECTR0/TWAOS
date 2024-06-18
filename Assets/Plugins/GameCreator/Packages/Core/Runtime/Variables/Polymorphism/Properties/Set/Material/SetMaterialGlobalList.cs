using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]

    [Description("Sets the Material value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetMaterialGlobalList : PropertyTypeSetMaterial
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueMaterial.TYPE_ID);

        public override void Set(Material value, Args args) => this.m_Variable.Set(value, args);
        public override Material Get(Args args) => this.m_Variable.Get(args) as Material;

        public static PropertySetMaterial Create => new PropertySetMaterial(
            new SetMaterialGlobalList()
        );

        public override string String => this.m_Variable.ToString();
    }
}
