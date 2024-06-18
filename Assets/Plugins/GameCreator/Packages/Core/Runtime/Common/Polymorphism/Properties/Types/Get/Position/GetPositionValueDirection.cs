using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Direction")]
    [Category("Values/Direction")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("A Position vector from a Direction value")]

    [Serializable]
    public class GetPositionValueDirection : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetDirection m_Direction = new PropertyGetDirection();

        public override Vector3 Get(Args args) => this.m_Direction.Get(args);
        public override Vector3 Get(GameObject args) => this.m_Direction.Get(args);

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionValueDirection()
        );

        public override string String => this.m_Direction.ToString();

        public override Vector3 EditorValue => this.m_Direction.EditorValue;
    }
}