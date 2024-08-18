using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector2")]
    [Category("Constants/Vector2")]
    
    [Image(typeof(IconVector2), ColorTheme.Type.Yellow)]
    [Description("Returns a world-space point in a 2D (XY) space")]

    [Serializable] [HideLabelsInEditor]
    public class GetPositionVector2 : PropertyTypeGetPosition
    {
        [SerializeField] protected Vector2 m_Point;

        public override Vector3 Get(Args args) => this.m_Point;
        public override Vector3 Get(GameObject gameObject) => this.m_Point;

        public GetPositionVector2()
        {
            this.m_Point = Vector2.zero;
        }
        
        public GetPositionVector2(Vector2 position)
        {
            this.m_Point = position;
        }

        public static PropertyGetPosition Create() => Create(Vector3.zero);
        
        public static PropertyGetPosition Create(Vector2 point) => new PropertyGetPosition(
            new GetPositionVector2(point)
        );
        
        public override string String => string.Format("({0:0.##}, {1:0.##})",
            this.m_Point.x, 
            this.m_Point.y
        );

        public override Vector3 EditorValue => this.m_Point;
    }
}