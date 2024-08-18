using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Direction Global Name Variable")]
    [Category("Variables/Direction Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the direction vector value of a Global Name Variable")]

    [Serializable]
    public class GetRotationDirectionGlobalName : PropertyTypeGetRotation
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueVector3.TYPE_ID);

        public override Quaternion Get(Args args)
        {
            return Quaternion.LookRotation(this.m_Variable.Get<Vector3>(args));
        }

        public override string String => this.m_Variable.ToString();
    }
}