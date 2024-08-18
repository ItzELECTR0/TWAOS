using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Description("Sets the Game Object value of a Local List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]

    [Serializable]
    public class SetGameObjectLocalList : PropertyTypeSetGameObject
    {
        [SerializeField]
        protected FieldSetLocalList m_Variable = new FieldSetLocalList(ValueGameObject.TYPE_ID);

        public override void Set(GameObject value, Args args) => this.m_Variable.Set(value, args);
        public override GameObject Get(Args args) => this.m_Variable.Get(args) as GameObject;

        public static PropertySetGameObject Create => new PropertySetGameObject(
            new SetGameObjectLocalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}