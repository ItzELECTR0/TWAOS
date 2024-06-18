using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Euler Local Name Variable")]
    [Category("Variables/Euler Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the euler rotation value of a Local Name Variable")]
    
    [Serializable]
    public class GetRotationEulerLocalName : PropertyTypeGetRotation
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueVector3.TYPE_ID);

        public override Quaternion Get(Args args)
        {
            return Quaternion.Euler(this.m_Variable.Get<Vector3>(args));
        }

        public override string String => this.m_Variable.ToString();
    }
}