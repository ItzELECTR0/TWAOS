using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random Animation Clip")]
    [Category("Random/Random Animation Clip")]

    [Image(typeof(IconDice), ColorTheme.Type.Yellow)]
    [Description("A random Animation Clip asset from a list")]

    [Serializable] [HideLabelsInEditor]
    public class GetAnimationRandom : PropertyTypeGetAnimation
    {
        [SerializeField] protected AnimationClip[] m_Values = Array.Empty<AnimationClip>();

        public override AnimationClip Get(Args args)
        {
            if ((this.m_Values?.Length ?? 0) == 0) return null;

            int index = UnityEngine.Random.Range(0, this.m_Values.Length);
            return this.m_Values[index];
        }

        public static PropertyGetAnimation Create => new PropertyGetAnimation(
            new GetAnimationRandom()
        );

        public override string String => "Random Clip";
    }
}
