using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Vector3 value of a Local Name Variable")]
    
    [Serializable]
    public class GetScaleLocalName : PropertyTypeGetScale
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueVector3.TYPE_ID);

        public override Vector3 Get(Args args) => this.m_Variable.Get<Vector3>(args);

        public override string String => this.m_Variable.ToString();
    }
}