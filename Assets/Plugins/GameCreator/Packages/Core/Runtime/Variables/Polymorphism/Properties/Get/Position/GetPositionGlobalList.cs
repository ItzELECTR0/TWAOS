using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the Vector3 value of a Global List Variable")]

    [Serializable]
    public class GetPositionGlobalList : PropertyTypeGetPosition
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueVector3.TYPE_ID);

        public override Vector3 Get(Args args) => this.m_Variable.Get<Vector3>(args);

        public override string String => this.m_Variable.ToString();
    }
}