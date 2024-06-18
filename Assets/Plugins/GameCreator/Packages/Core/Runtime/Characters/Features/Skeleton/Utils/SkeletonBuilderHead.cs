using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeHead(Animator animator, float weight)
        {
            float width = Vector3.Distance(
                animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position,
                animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position
            );

            float radius = width * 0.25f;

            HumanBodyBones connectionBone = HumanBodyBones.Neck;
            if (animator.GetBoneTransform(connectionBone) == null) connectionBone = HumanBodyBones.UpperChest;
            if (animator.GetBoneTransform(connectionBone) == null) connectionBone = HumanBodyBones.Chest;
            if (animator.GetBoneTransform(connectionBone) == null) connectionBone = HumanBodyBones.Spine;
            
            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform connection = animator.GetBoneTransform(connectionBone);

            Vector3 direction = (head.position - connection.position).normalized;

            return new VolumeSphere(
                HumanBodyBones.Head,
                weight,
                new JointConfigurable(
                    new Bone(connectionBone),
                    ConfigurableJointMotion.Locked,
                    ConfigurableJointMotion.Limited,
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 0, 1),
                    new SpringLimit(280f, 17f),
                    new SpringLimit(280f, 17f),
                    new TetherLimit(-30f, 0.3f, 11f),
                    new TetherLimit(25f, 0.3f, 11f),
                    new TetherLimit(15f, 0.3f, 3f),
                    new TetherLimit(20f, 0.3f, 4f)
                ),
                direction * radius,
                radius
            );
        }
    }
}