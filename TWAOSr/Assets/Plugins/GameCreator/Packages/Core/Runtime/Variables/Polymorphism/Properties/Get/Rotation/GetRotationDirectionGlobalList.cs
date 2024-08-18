using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Direction Global List Variable")]
    [Category("Variables/Direction Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the direction vector value of a Global List Variable")]

    [Serializable]
    public class GetRotationDirectionGlobalList : PropertyTypeGetRotation
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueVector3.TYPE_ID);

        public override Quaternion Get(Args args)
        {
            return Quaternion.LookRotation(this.m_Variable.Get<Vector3>(args));
        }

        public override string String => this.m_Variable.ToString();
    }
}