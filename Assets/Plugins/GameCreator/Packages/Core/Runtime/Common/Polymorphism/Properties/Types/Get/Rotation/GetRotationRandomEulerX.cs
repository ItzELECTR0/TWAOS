using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random Euler X")]
    [Category("Random/Random Euler X")]
    
    [Image(typeof(IconDice), ColorTheme.Type.Yellow, typeof(OverlayX))]
    [Description("Creates a rotation with a random euler X axis")]

    [Serializable]
    public class GetRotationRandomEulerX : PropertyTypeGetRotation
    {
        public override Quaternion Get(Args args) => Quaternion.Euler(Vector3.right * UnityEngine.Random.Range(-360, 360f));
        public override Quaternion Get(GameObject gameObject) => Quaternion.Euler(Vector3.right * UnityEngine.Random.Range(-360, 360f));

        public override string String => "Random X";
    }
}