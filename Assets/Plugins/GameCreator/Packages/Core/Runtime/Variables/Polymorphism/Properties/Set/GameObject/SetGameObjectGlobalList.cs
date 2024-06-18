using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Description("Sets the Game Object value of a Global List Variable")]
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]

    [Serializable]
    public class SetGameObjectGlobalList : PropertyTypeSetGameObject
    {
        [SerializeField]
        protected FieldSetGlobalList m_Variable = new FieldSetGlobalList(ValueGameObject.TYPE_ID);

        public override void Set(GameObject value, Args args) => this.m_Variable.Set(value, args);
        public override GameObject Get(Args args) => this.m_Variable.Get(args) as GameObject;

        public static PropertySetGameObject Create => new PropertySetGameObject(
            new SetGameObjectGlobalList()
        );
        
        public override string String => this.m_Variable.ToString();
    }
}