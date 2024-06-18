using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Vector3 value of a Global Name Variable")]

    [Serializable]
    public class GetPositionGlobalName : PropertyTypeGetPosition
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueVector3.TYPE_ID);

        public override Vector3 Get(Args args) => this.m_Variable.Get<Vector3>(args);

        public override string String => this.m_Variable.ToString();
    }
}