using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sphere Surface")]
    [Category("Random/Sphere Surface")]
    
    [Image(typeof(IconDice), ColorTheme.Type.White)]
    [Description("Returns a random position at the edges a spherical volume")]

    [Serializable]
    public class GetPositionRandomSphereSurface : PropertyTypeGetPosition
    {
        [SerializeField] protected PropertyGetDecimal m_Radius = GetDecimalConstantOne.Create;

        public override Vector3 Get(Args args)
        {
            float radius = (float) this.m_Radius.Get(args);
            return UnityEngine.Random.onUnitSphere * radius;
        }

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionRandomSphereSurface()
        );

        public override string String => "on Sphere";
    }
}