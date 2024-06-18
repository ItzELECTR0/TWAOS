using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeUpperLegL(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.Hips),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(520f, 30f),
                new SpringLimit(520f, 30f),
                new TetherLimit(-20f, 0.3f, 18f),
                new TetherLimit(70f, 0.3f, 18f),
                new TetherLimit(30f, 0.3f, 6f),
                new TetherLimit(8f, 0.3f, 1.5f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.LeftUpperLeg,
                HumanBodyBones.LeftLowerLeg,
                configurableJoint
            );
        }
        
        private static IVolume MakeUpperLegR(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.Hips),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(520f, 30f),
                new SpringLimit(520f, 30f),
                new TetherLimit(-20f, 0.3f, 18f),
                new TetherLimit(70f, 0.3f, 18f),
                new TetherLimit(30f, 0.3f, 6f),
                new TetherLimit(8f, 0.3f, 1.5f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.RightUpperLeg,
                HumanBodyBones.RightLowerLeg,
                configurableJoint
            );
        }
    }
}