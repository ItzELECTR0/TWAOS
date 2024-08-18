using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Direction Local List Variable")]
    [Category("Variables/Direction Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the direction vector value of a Local List Variable")]

    [Serializable]
    public class GetRotationDirectionLocalList : PropertyTypeGetRotation
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueVector3.TYPE_ID);

        public override Quaternion Get(Args args)
        {
            return Quaternion.LookRotation(this.m_Variable.Get<Vector3>(args));
        }

        public override string String => this.m_Variable.ToString();
    }
}