using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sphere Volume")]
    [Category("Random/Sphere Volume")]
    
    [Image(typeof(IconDice), ColorTheme.Type.White)]
    [Description("Returns a random position inside a spherical volume")]

    [Serializable]
    public class GetPositionRandomSphere : PropertyTypeGetPosition
    {
        [SerializeField] protected PropertyGetDecimal m_Radius = GetDecimalConstantOne.Create;

        public override Vector3 Get(Args args)
        {
            float radius = (float) this.m_Radius.Get(args);
            return UnityEngine.Random.insideUnitSphere * radius;
        }

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionRandomSphere()
        );

        public override string String => "in Sphere";
    }
}