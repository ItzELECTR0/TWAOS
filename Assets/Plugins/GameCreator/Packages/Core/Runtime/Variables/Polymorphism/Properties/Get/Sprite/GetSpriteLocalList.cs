using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Sprite value of a Local List Variable")]

    [Serializable]
    public class GetSpriteLocalList : PropertyTypeGetSprite
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueSprite.TYPE_ID);

        public override Sprite Get(Args args) => this.m_Variable.Get<Sprite>(args);

        public override string String => this.m_Variable.ToString();
    }
}