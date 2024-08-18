using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global List Variable")]
    [Category("Variables/Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns the Game Object value of a Global List Variable")]

    [Serializable]
    public class GetGameObjectGlobalList : PropertyTypeGetGameObject
    {
        [SerializeField]
        protected FieldGetGlobalList m_Variable = new FieldGetGlobalList(ValueGameObject.TYPE_ID);

        public override GameObject Get(Args args) => this.m_Variable.Get<GameObject>(args);

        public override string String => this.m_Variable.ToString();
    }
}