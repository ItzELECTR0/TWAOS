using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Global Name Variable")]
    [Category("Variables/Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Game Object value of a Global Name Variable")]

    [Serializable]
    public class GetGameObjectGlobalName : PropertyTypeGetGameObject
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueGameObject.TYPE_ID);

        public override GameObject Get(Args args) => this.m_Variable.Get<GameObject>(args);

        public override string String => this.m_Variable.ToString();
    }
}