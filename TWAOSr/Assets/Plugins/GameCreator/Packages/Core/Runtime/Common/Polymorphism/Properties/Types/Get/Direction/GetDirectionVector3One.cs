using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("One")]
    [Category("Constants/One")]
    
    [Image(typeof(IconOne), ColorTheme.Type.TextNormal)]
    [Description("A Vector3 that has a unit in all components")]

    [Serializable]
    public class GetDirectionVector3One : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args) => Vector3.one;
        public override Vector3 Get(GameObject gameObject) => Vector3.one;

        public static PropertyGetDirection Create() => new PropertyGetDirection(
            new GetDirectionVector3One()
        );

        public override string String => "(1,1,1)";
        
        public override Vector3 EditorValue => Vector3.one;
    }
}