using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeSpine(Animator animator, float weight)
        {
            Transform headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            
            Transform legLTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            Transform legRTransform = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);


            float minRadius = Math.Max(
                Vector3.Distance(hipsTransform.position, legLTransform.position),
                Vector3.Distance(hipsTransform.position, legRTransform.position)
            );
            
            Transform armLTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            Transform armRTransform = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            
            float maxRadius = Math.Max(
                Vector3.Distance(hipsTransform.position, armLTransform.position) * 0.5f,
                Vector3.Distance(hipsTransform.position, armRTransform.position) * 0.5f
            );

            Transform spineTransform = animator.GetBoneTransform(HumanBodyBones.Spine);

            Bounds bounds = new Bounds(
                spineTransform.position,
                Vector3.one * minRadius
            );

            float radius = minRadius;
            
            Transform chestTransform = animator.GetBoneTransform(HumanBodyBones.Chest);
            Transform upperChestTransform = animator.GetBoneTransform(HumanBodyBones.UpperChest);
            
            if (chestTransform == null || upperChestTransform == null)
            {
                Vector3 upperChestPoint = Vector3.Lerp(
                    hipsTransform.position,
                    headTransform.position,
                    0.6f
                );
                
                bounds.Encapsulate(upperChestPoint);
                bounds.Encapsulate(armLTransform.position);
                bounds.Encapsulate(armRTransform.position);
                
                radius = maxRadius;
            }

            return new VolumeCapsule(
                HumanBodyBones.Spine,
                weight,
                new JointConfigurable(
                    new Bone(HumanBodyBones.Hips),
                    ConfigurableJointMotion.Locked,
                    ConfigurableJointMotion.Limited,
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 0, 1),
                    new SpringLimit(550, 30f),
                    new SpringLimit(550f, 30f),
                    new TetherLimit(0f, 0.3f, 20f),
                    new TetherLimit(95f, 0.3f, 20f),
                    new TetherLimit(5f, 0.3f, 1f),
                    new TetherLimit(10f, 0.3f, 2f)
                ),
                spineTransform.InverseTransformPoint(bounds.center),
                bounds.size.x,
                radius,
                VolumeCapsule.Direction.AxisX
            );
        }
    }
}