using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Sprite value of a Local Name Variable")]
    
    [Serializable]
    public class GetSpriteLocalName : PropertyTypeGetSprite
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueSprite.TYPE_ID);

        public override Sprite Get(Args args) => this.m_Variable.Get<Sprite>(args);

        public override string String => this.m_Variable.ToString();
    }
}