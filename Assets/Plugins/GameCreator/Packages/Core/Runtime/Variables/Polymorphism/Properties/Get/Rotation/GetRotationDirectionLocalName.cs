using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Direction Local Name Variable")]
    [Category("Variables/Direction Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the direction vector value of a Local Name Variable")]
    
    [Serializable]
    public class GetRotationDirectionLocalName : PropertyTypeGetRotation
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueVector3.TYPE_ID);

        public override Quaternion Get(Args args)
        {
            return Quaternion.LookRotation(this.m_Variable.Get<Vector3>(args));
        }

        public override string String => this.m_Variable.ToString();
    }
}