using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeShoulderL(Animator animator, float weight)
        {
            HumanBodyBones parent = HumanBodyBones.UpperChest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Chest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Spine;

            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(parent),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new SpringLimit(250f, 15f),
                new SpringLimit(250f, 15f),
                new TetherLimit(-15, 0.3f, 6f),
                new TetherLimit(15, 0.3f, 6f),
                new TetherLimit(15f, 0.3f, 3f),
                new TetherLimit(15f, 0.3f, 3f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.LeftShoulder,
                HumanBodyBones.LeftUpperArm,
                configurableJoint
            );
        }
        
        private static IVolume MakeShoulderR(Animator animator, float weight)
        {
            HumanBodyBones parent = HumanBodyBones.UpperChest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Chest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Spine;

            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(parent),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, 0, 1),
                new Vector3(0, -1, 0),
                new SpringLimit(250f, 15f),
                new SpringLimit(250f, 15f),
                new TetherLimit(-15, 0.3f, 6f),
                new TetherLimit(15, 0.3f, 6f),
                new TetherLimit(15f, 0.3f, 3f),
                new TetherLimit(15f, 0.3f, 3f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.RightShoulder,
                HumanBodyBones.RightUpperArm,
                configurableJoint
            );
        }
    }
}