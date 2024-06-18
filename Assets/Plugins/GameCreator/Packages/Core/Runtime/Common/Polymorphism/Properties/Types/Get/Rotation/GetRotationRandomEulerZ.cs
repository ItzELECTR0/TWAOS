using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random Euler Z")]
    [Category("Random/Random Euler Z")]
    
    [Image(typeof(IconDice), ColorTheme.Type.Yellow, typeof(OverlayZ))]
    [Description("Creates a rotation with a random euler Z axis")]

    [Serializable]
    public class GetRotationRandomEulerZ : PropertyTypeGetRotation
    {
        public override Quaternion Get(Args args) => Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(-360, 360f));
        public override Quaternion Get(GameObject gameObject) => Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(-360, 360f));
        
        public override string String => "Random Z";
    }
}