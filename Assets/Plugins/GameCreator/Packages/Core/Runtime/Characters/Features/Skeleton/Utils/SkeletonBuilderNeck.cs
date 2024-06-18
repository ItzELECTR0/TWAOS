using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeNeck(Animator animator, float weight)
        {
            HumanBodyBones parent = HumanBodyBones.UpperChest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Chest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Spine;
            
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(parent),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(350f, 20f),
                new SpringLimit(350f, 20f),
                new TetherLimit(-30f, 0.3f, 15f),
                new TetherLimit(10f, 0.3f, 15f),
                new TetherLimit(10f, 0.3f, 5f),
                new TetherLimit(10f, 0.3f, 5f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.Neck,
                HumanBodyBones.Head,
                configurableJoint
            );
        }
    }
}