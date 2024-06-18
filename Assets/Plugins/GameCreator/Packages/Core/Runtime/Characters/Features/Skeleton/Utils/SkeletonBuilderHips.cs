using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeHips(Animator animator, float weight)
        {
            Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform legTransform = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            
            float radius = Vector3.Distance(
                hipsTransform.position,
                legTransform.position
            );

            return new VolumeCapsule(
                HumanBodyBones.Hips,
                weight,
                new JointConfigurable(
                    Bone.CreateNone(),
                    ConfigurableJointMotion.Free,
                    ConfigurableJointMotion.Free,
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new SpringLimit(620f, 35f),
                    new SpringLimit(620f, 35f),
                    new TetherLimit(0f, 0.3f, 0f),
                    new TetherLimit(0f, 0.3f, 0f),
                    new TetherLimit(0f, 0.3f, 0f),
                    new TetherLimit(0f, 0.3f, 0f)
                ),
                Vector3.zero,
                radius * 2f,
                radius,
                VolumeCapsule.Direction.AxisX
            );
        }
    }
}