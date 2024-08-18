using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector")]
    [Category("Constants/Vector")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow)]
    [Description("A Vector3 that represents the scale on each axis")]

    [Serializable] [HideLabelsInEditor]
    public class GetScaleVector3 : PropertyTypeGetScale
    {
        [SerializeField] protected Vector3 m_Scale;

        public override Vector3 Get(Args args) => this.m_Scale;
        public override Vector3 Get(GameObject gameObject) => this.m_Scale;

        public GetScaleVector3()
        {
            this.m_Scale = Vector3.zero;
        }
        
        public GetScaleVector3(Vector3 scale)
        {
            this.m_Scale = scale;
        }
        
        public static PropertyGetScale Create(Vector3 scale) => new PropertyGetScale(
            new GetScaleVector3(scale)
        );

        public override string String => this.m_Scale.ToString();
    }
}