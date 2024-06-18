using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeUpperChest(Animator animator, float weight)
        {
            Transform headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform upperChestTransform = animator.GetBoneTransform(HumanBodyBones.UpperChest);

            Transform armLTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            Transform armRTransform = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            
            float radius = Vector3.Distance(
                headTransform.position,
                upperChestTransform.position
            );

            float height = Vector3.Distance(
                armLTransform.position,
                armRTransform.position
            );

            return new VolumeCapsule(
                HumanBodyBones.UpperChest,
                weight,
                new JointConfigurable(
                    new Bone(HumanBodyBones.Chest),
                    ConfigurableJointMotion.Locked,
                    ConfigurableJointMotion.Limited,
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 0, 1),
                    new SpringLimit(400f, 25f),
                    new SpringLimit(400f, 25f),
                    new TetherLimit(-5f, 0.3f, 5f),
                    new TetherLimit(15f, 0.3f, 5f),
                    new TetherLimit(5f, 0.3f, 1f),
                    new TetherLimit(10f, 0.3f, 2f)
                ),
                Vector3.zero, 
                height,
                radius,
                VolumeCapsule.Direction.AxisX
            );
        }
    }
}