using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector Zero")]
    [Category("Constants/Vector Zero")]
    
    [Image(typeof(IconZero), ColorTheme.Type.Yellow)]
    [Description("Returns zeroed Vector3 position")]

    [Serializable]
    public class GetPositionVectorZero : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args) => Vector3.zero;
        public override Vector3 Get(GameObject gameObject) => Vector3.zero;

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionVectorZero()
        );
        
        public override string String => "Zero";

        public override Vector3 EditorValue => Vector3.zero;
    }
}