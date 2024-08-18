using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Description("Sets the Vector3 value of a Global Name Variable")]
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]

    [Serializable]
    public class SetVector3GlobalName : PropertyTypeSetVector3
    {
        [SerializeField]
        protected FieldSetGlobalName m_Variable = new FieldSetGlobalName(ValueVector3.TYPE_ID);

        public override void Set(Vector3 value, Args args) => this.m_Variable.Set(value, args);
        public override Vector3 Get(Args args) => (Vector3) this.m_Variable.Get(args);

        public static PropertySetVector3 Create => new PropertySetVector3(
            new SetVector3GlobalName()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}