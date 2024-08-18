using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Integer")]
    [Category("Integer")]

    [Image(typeof(IconNumber), ColorTheme.Type.TextNormal)]
    [Description("A constant integer number")]

    [Serializable] [HideLabelsInEditor]
    public class GetDecimalInteger : PropertyTypeGetDecimal
    {
        [SerializeField] protected int m_Value;

        public override double Get(Args args) => this.m_Value;
        public override double Get(GameObject gameObject) => this.m_Value;

        public GetDecimalInteger() : base()
        { }

        public GetDecimalInteger(int value) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetInteger Create(int value)
        {
            return new PropertyGetInteger(
                new GetDecimalInteger(value)
            );
        }

        public override string String => this.m_Value.ToString();

        public override double EditorValue => this.m_Value;
    }
}