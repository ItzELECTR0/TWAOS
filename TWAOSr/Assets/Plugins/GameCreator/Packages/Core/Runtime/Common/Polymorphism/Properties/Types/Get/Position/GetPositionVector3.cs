using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector3")]
    [Category("Constants/Vector3")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow)]
    [Description("Returns a world-space point in space")]

    [Serializable] [HideLabelsInEditor]
    public class GetPositionVector3 : PropertyTypeGetPosition
    {
        [SerializeField] protected Vector3 m_Position;

        public override Vector3 Get(Args args) => this.m_Position;
        public override Vector3 Get(GameObject gameObject) => this.m_Position;

        public GetPositionVector3()
        {
            this.m_Position = Vector3.zero;
        }
        
        public GetPositionVector3(Vector3 position)
        {
            this.m_Position = position;
        }

        public static PropertyGetPosition Create() => Create(Vector3.zero);
        
        public static PropertyGetPosition Create(Vector3 position) => new PropertyGetPosition(
            new GetPositionVector3(position)
        );

        public override string String => string.Format("({0:0.##}, {1:0.##}, {2:0.##})",
            this.m_Position.x, 
            this.m_Position.y,
            this.m_Position.z
        );
        
        public override Vector3 EditorValue => this.m_Position;
    }
}