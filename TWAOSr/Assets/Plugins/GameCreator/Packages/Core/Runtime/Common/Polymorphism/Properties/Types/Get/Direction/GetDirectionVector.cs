using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector")]
    [Category("Constants/Vector")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("A Vector3 that defines a direction")]

    [Serializable] [HideLabelsInEditor]
    public class GetDirectionVector : PropertyTypeGetDirection
    {
        [SerializeField] protected Vector3 m_Direction = Vector3.forward;
        
        public GetDirectionVector()
        { }
        
        public GetDirectionVector(Vector3 direction)
        {
            this.m_Direction = direction;
        }

        public override Vector3 Get(Args args) => this.m_Direction;
        public override Vector3 Get(GameObject gameObject) => this.m_Direction;

        public static PropertyGetDirection Create(Vector3 direction) => new PropertyGetDirection(
            new GetDirectionVector(direction)
        );

        public static PropertyGetDirection Create() => Create(Vector3.zero);

        public override string String => string.Format("({0:0.##}, {1:0.##}, {2:0.##})",
            this.m_Direction.x, 
            this.m_Direction.y,
            this.m_Direction.z
        );
        
        public override Vector3 EditorValue => this.m_Direction;
    }
}