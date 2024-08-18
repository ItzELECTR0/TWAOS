using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Euler Global Name Variable")]
    [Category("Variables/Euler Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the euler rotation value of a Global Name Variable")]

    [Serializable]
    public class GetRotationEulerGlobalName : PropertyTypeGetRotation
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueVector3.TYPE_ID);

        public override Quaternion Get(Args args)
        {
            return Quaternion.Euler(this.m_Variable.Get<Vector3>(args));
        }

        public override string String => this.m_Variable.ToString();
    }
}