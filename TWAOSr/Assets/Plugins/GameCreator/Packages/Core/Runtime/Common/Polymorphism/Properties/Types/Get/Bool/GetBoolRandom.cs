using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random")]
    [Category("Random/Random")]
    
    [Image(typeof(IconDice), ColorTheme.Type.Red)]
    [Description("Randomly returns true or false with equal probability")]
    
    [Keywords("Dice", "Any")]
    [Serializable]
    public class GetBoolRandom : PropertyTypeGetBool
    {
        public override bool Get(Args args) => UnityEngine.Random.Range(0, 2) == 0;
        public override bool Get(GameObject gameObject) => UnityEngine.Random.Range(0, 2) == 0;

        public GetBoolRandom() : base()
        { }

        public static PropertyGetBool Create => new PropertyGetBool(
            new GetBoolRandom()
        );

        public override string String => "Random";
    }
}