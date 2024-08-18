using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Position Y")]
    [Category("Transforms/Position Y")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("The Y component of a Vector3 that represents a position in space")]

    [Keywords("Position", "Vector3", "Up", "Down")]
    
    [Serializable]
    public class GetDecimalTransformsPositionY : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetPosition m_Position = GetPositionCharacter.Create;

        public override double Get(Args args) => this.m_Position.Get(args).y;
        public override double Get(GameObject gameObject) => this.m_Position.Get(gameObject).y;

        public override string String => $"{this.m_Position}.Y";
    }
}