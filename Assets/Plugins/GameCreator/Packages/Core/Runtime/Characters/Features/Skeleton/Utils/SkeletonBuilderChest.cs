using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeChest(Animator animator, float weight)
        {
            Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform upperChestTransform = animator.GetBoneTransform(HumanBodyBones.UpperChest);
            
            Transform legLTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            Transform legRTransform = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            
            float minRadius = Math.Max(
                Vector3.Distance(hipsTransform.position, legLTransform.position),
                Vector3.Distance(hipsTransform.position, legRTransform.position)
            );

            float maxRadius = Vector3.Distance(
                headTransform.position,
                upperChestTransform.position
            );

            return new VolumeCapsule(
                HumanBodyBones.Chest,
                weight,
                new JointConfigurable(
                    new Bone(HumanBodyBones.Spine),
                    ConfigurableJointMotion.Locked,
                    ConfigurableJointMotion.Limited,
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 0, 1),
                    new SpringLimit(470f, 30f),
                    new SpringLimit(470f, 30f),
                    new TetherLimit(-5f, 0.3f, 5f),
                    new TetherLimit(20f, 0.3f, 5f),
                    new TetherLimit(5f, 0.3f, 1f),
                    new TetherLimit(10f, 0.3f, 2f)
                ),
                Vector3.zero, 
                Mathf.Lerp(minRadius, maxRadius, 0.5f),
                Mathf.Lerp(minRadius, maxRadius, 0.5f),
                VolumeCapsule.Direction.AxisX
            );
        }
    }
}