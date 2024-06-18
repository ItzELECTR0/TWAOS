using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeLowerArmL(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.LeftUpperArm),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, -1, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(230f, 15f),
                new SpringLimit(230f, 15f),
                new TetherLimit(0, 0.3f, 25f),
                new TetherLimit(120, 0.3f, 25f),
                new TetherLimit(30f, 0.3f, 6f),
                new TetherLimit(0f, 0.3f, 0f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.LeftLowerArm,
                HumanBodyBones.LeftHand,
                configurableJoint
            );
        }
        
        private static IVolume MakeLowerArmR(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.RightUpperArm),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(230f, 15f),
                new SpringLimit(230f, 15f),
                new TetherLimit(0, 0.3f, 25f),
                new TetherLimit(120, 0.3f, 25f),
                new TetherLimit(30f, 0.3f, 6f),
                new TetherLimit(0f, 0.3f, 0f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.RightLowerArm,
                HumanBodyBones.RightHand,
                configurableJoint
            );
        }
    }
}