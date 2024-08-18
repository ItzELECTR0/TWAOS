using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Local Name Variable")]
    [Category("Variables/Local Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Game Object value of a Local Name Variable")]

    [Serializable]
    public class GetGameObjectLocalName : PropertyTypeGetGameObject
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueGameObject.TYPE_ID);

        public override GameObject Get(Args args) => this.m_Variable.Get<GameObject>(args);

        public override string String => this.m_Variable.ToString();
    }
}