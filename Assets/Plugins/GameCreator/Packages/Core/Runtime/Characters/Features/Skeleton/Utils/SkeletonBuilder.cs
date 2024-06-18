using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static Volumes Make(Animator animator)
        {
            if (animator == null) return null;
            if (!animator.isHuman) return null;

            List<IVolume> volumes = new List<IVolume>
            {
                MakeHips(animator, 1f),
                MakeSpine(animator, 0.9f),
                MakeHead(animator, 0.45f),
                MakeUpperLegL(animator, 0.85f),
                MakeLowerLegL(animator, 0.6f),
                MakeFootL(animator, 0.4f),
                MakeUpperLegR(animator, 0.85f),
                MakeLowerLegR(animator, 0.6f),
                MakeFootR(animator, 0.4f),
                MakeUpperArmL(animator, 0.6f),
                MakeLowerArmL(animator, 0.35f),
                MakeHandL(animator, 0.25f),
                MakeUpperArmR(animator, 0.6f),
                MakeLowerArmR(animator, 0.35f),
                MakeHandR(animator, 0.25f),
            };

            if (animator.GetBoneTransform(HumanBodyBones.Chest) != null &&
                animator.GetBoneTransform(HumanBodyBones.UpperChest) != null)
            {
                volumes.Add(MakeChest(animator, 0.75f));
                volumes.Add(MakeUpperChest(animator, 0.65f));
            }

            if (animator.GetBoneTransform(HumanBodyBones.RightShoulder) != null &&
                animator.GetBoneTransform(HumanBodyBones.LeftShoulder) != null)
            {
                volumes.Add(MakeShoulderL(animator, 0.35f));
                volumes.Add(MakeShoulderR(animator, 0.35f));
            }

            if (animator.GetBoneTransform(HumanBodyBones.Neck) != null)
            {
                volumes.Add(MakeNeck(animator, 0.6f));
            }

            return new Volumes(volumes.ToArray());
        }
    }
}
