using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local List Variable")]
    [Category("Variables/Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Game Object value of a Local List Variable")]

    [Serializable]
    public class GetGameObjectLocalList : PropertyTypeGetGameObject
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueGameObject.TYPE_ID);

        public override GameObject Get(Args args) => this.m_Variable.Get<GameObject>(args);

        public override string String => this.m_Variable.ToString();
    }
}