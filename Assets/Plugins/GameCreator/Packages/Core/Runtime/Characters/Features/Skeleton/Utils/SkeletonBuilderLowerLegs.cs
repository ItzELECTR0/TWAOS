using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeLowerLegL(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.LeftUpperLeg),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(350f, 22f),
                new SpringLimit(350f, 22f),
                new TetherLimit(-90, 0.3f, 18f),
                new TetherLimit(0, 0.3f, 18f),
                new TetherLimit(10f, 0.3f, 2f),
                new TetherLimit(1f, 0.3f, 2f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.LeftLowerLeg,
                HumanBodyBones.LeftFoot,
                configurableJoint
            );
        }
        
        private static IVolume MakeLowerLegR(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.RightUpperLeg),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new SpringLimit(350f, 22f),
                new SpringLimit(350f, 22f),
                new TetherLimit(-90, 0.3f, 18f),
                new TetherLimit(0, 0.3f, 18f),
                new TetherLimit(10f, 0.3f, 2f),
                new TetherLimit(1f, 0.3f, 2f)
            );

            return MakeMiddleLimb(
                animator,
                weight,
                HumanBodyBones.RightLowerLeg,
                HumanBodyBones.RightFoot,
                configurableJoint
            );
        }
    }
}