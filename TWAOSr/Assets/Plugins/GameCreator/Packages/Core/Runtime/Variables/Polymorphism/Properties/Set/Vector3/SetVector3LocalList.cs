using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Description("Sets the Vector3 value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetVector3LocalList : PropertyTypeSetVector3
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueVector3.TYPE_ID);

        public override void Set(Vector3 value, Args args) => this.m_Variable.Set(value, args);
        public override Vector3 Get(Args args) => (Vector3) this.m_Variable.Get(args);

        public static PropertySetVector3 Create => new PropertySetVector3(
            new SetVector3LocalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}