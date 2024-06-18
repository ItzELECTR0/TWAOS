using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random Euler Y")]
    [Category("Random/Random Euler Y")]
    
    [Image(typeof(IconDice), ColorTheme.Type.Yellow, typeof(OverlayY))]
    [Description("Creates a rotation with a random euler Y axis")]

    [Serializable]
    public class GetRotationRandomEulerY : PropertyTypeGetRotation
    {
        public override Quaternion Get(Args args) => Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(-360, 360f));
        public override Quaternion Get(GameObject gameObject) => Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(-360, 360f));

        public override string String => "Random Y";
    }
}