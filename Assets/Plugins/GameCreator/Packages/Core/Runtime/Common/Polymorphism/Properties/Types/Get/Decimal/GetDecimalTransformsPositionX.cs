using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Position X")]
    [Category("Transforms/Position X")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Red)]
    [Description("The X component of a Vector3 that represents a position in space")]

    [Keywords("Position", "Vector3", "Right", "Left")]
    
    [Serializable]
    public class GetDecimalTransformsPositionX : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetPosition m_Position = GetPositionCharacter.Create;

        public override double Get(Args args) => this.m_Position.Get(args).x;
        public override double Get(GameObject gameObject) => this.m_Position.Get(gameObject).x;

        public override string String => $"{this.m_Position}.X";
    }
}