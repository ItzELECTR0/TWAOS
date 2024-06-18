using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Any Global List Variable")]
    [Category("Variables/Any Global List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal, typeof(OverlayDot))]
    [Description("Returns true if the Global List Variable has at least one element")]

    [Serializable]
    public class GetBoolGlobalListAny : PropertyTypeGetBool
    {
        [SerializeField] private GlobalListVariables m_List;

        public override bool Get(Args args)
        {
            return (this.m_List != null ? this.m_List.Count : 0) > 0;
        }

        public override bool Get(GameObject gameObject)
        {
            return (this.m_List != null ? this.m_List.Count : 0) > 0;
        }

        public override string String => string.Format(
            "Any in {0}",
            this.m_List != null ? this.m_List.name : "(none)"
        );
    }
}