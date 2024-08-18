using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Description("Sets the Texture value of a Local Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]

    [Serializable]
    public class SetTextureLocalName : PropertyTypeSetTexture
    {
        [SerializeField]
        protected FieldSetLocalName m_Variable = new FieldSetLocalName(ValueTexture.TYPE_ID);

        public override void Set(Texture value, Args args) => this.m_Variable.Set(value, args);
        public override Texture Get(Args args) => this.m_Variable.Get(args) as Texture;

        public static PropertySetTexture Create => new PropertySetTexture(
            new SetTextureLocalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}