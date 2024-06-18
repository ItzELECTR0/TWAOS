using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]

    [Description("Sets the Material value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetMaterialGlobalName : PropertyTypeSetMaterial
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueMaterial.TYPE_ID);

        public override void Set(Material value, Args args) => this.m_Variable.Set(value, args);
        public override Material Get(Args args) => this.m_Variable.Get(args) as Material;

        public static PropertySetMaterial Create => new PropertySetMaterial(
            new SetMaterialGlobalName()
        );

        public override string String => this.m_Variable.ToString();
    }
}
