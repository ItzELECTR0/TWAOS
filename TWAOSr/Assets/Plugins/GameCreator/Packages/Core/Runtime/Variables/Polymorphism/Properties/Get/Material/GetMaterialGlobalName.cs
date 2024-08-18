using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Material value of a Global Name Variable")]

    [Serializable]
    public class GetMaterialGlobalName : PropertyTypeGetMaterial
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueMaterial.TYPE_ID);

        public override Material Get(Args args) => this.m_Variable.Get<Material>(args);

        public override string String => this.m_Variable.ToString();
    }
}
