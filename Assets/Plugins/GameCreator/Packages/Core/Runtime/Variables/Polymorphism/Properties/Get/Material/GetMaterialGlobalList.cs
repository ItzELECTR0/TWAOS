using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]

    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the Material value of a Global List Variable")]

    [Serializable]
    public class GetMaterialGlobalList : PropertyTypeGetMaterial
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueMaterial.TYPE_ID);

        public override Material Get(Args args) => this.m_Variable.Get<Material>(args);

        public override string String => this.m_Variable.ToString();
    }
}
