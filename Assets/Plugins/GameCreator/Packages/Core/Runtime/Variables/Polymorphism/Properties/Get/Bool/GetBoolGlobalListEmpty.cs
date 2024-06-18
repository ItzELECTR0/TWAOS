using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Empty Global List Variable")]
    [Category("Variables/Empty Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns true if the Global List Variable is empty")]

    [Serializable]
    public class GetBoolGlobalListEmpty : PropertyTypeGetBool
    {
        [SerializeField] private GlobalListVariables m_List;

        public override bool Get(Args args)
        {
            return (this.m_List != null ? this.m_List.Count : 0) == 0;
        }

        public override bool Get(GameObject gameObject)
        {
            return (this.m_List != null ? this.m_List.Count : 0) == 0;
        }

        public override string String => string.Format(
            "{0} is Empty",
            this.m_List != null ? this.m_List.name : "(none)"
        );
    }
}