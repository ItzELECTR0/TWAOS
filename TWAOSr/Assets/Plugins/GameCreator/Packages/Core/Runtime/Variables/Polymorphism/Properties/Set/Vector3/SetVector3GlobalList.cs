using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Description("Sets the Vector3 value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetVector3GlobalList : PropertyTypeSetVector3
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueVector3.TYPE_ID);

        public override void Set(Vector3 value, Args args) => this.m_Variable.Set(value, args);
        public override Vector3 Get(Args args) => (Vector3) this.m_Variable.Get(args);

        public static PropertySetVector3 Create => new PropertySetVector3(
            new SetVector3GlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}