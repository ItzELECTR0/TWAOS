using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeUpperArmL(Animator animator, float weight)
        {
            HumanBodyBones parent = HumanBodyBones.LeftShoulder;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.UpperChest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Chest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Spine;

            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(parent),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, -1, 0),
                new Vector3(0, 0, -1),
                new SpringLimit(380f, 22f),
                new SpringLimit(380f, 22f),
                new TetherLimit(-90, 0.3f, 20f),
                new TetherLimit(10, 0.3f, 20f),
                new TetherLimit(60f, 0.3f, 12f),
                new TetherLimit(30f, 0.3f, 6f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.LeftUpperArm,
                HumanBodyBones.LeftLowerArm,
                configurableJoint
            );
        }
        
        private static IVolume MakeUpperArmR(Animator animator, float weight)
        {
            HumanBodyBones parent = HumanBodyBones.RightShoulder;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.UpperChest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Chest;
            if (animator.GetBoneTransform(parent) == null) parent = HumanBodyBones.Spine;
            
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(parent),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, 1, 0),
                new Vector3(0, 0, -1),
                new SpringLimit(380f, 22f),
                new SpringLimit(380f, 22f),
                new TetherLimit(-90, 0.3f, 20f),
                new TetherLimit(10, 0.3f, 20f),
                new TetherLimit(60f, 0.3f, 12f),
                new TetherLimit(30f, 0.3f, 6f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.RightUpperArm,
                HumanBodyBones.RightLowerArm,
                configurableJoint
            );
        }
    }
}