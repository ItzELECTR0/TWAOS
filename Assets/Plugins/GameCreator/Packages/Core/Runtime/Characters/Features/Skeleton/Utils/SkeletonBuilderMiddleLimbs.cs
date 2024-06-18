using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeMiddleLimb(
            Animator animator, 
            float weight,
            HumanBodyBones upperBone, 
            HumanBodyBones lowerBone,
            IJoint joint
        )
        {
            Transform upperBoneTransform = animator.GetBoneTransform(upperBone);
            Transform lowerBoneTransform = animator.GetBoneTransform(lowerBone);

            Vector3 endPoint = lowerBoneTransform.position;
            
            CalculateDirection(
                upperBoneTransform.InverseTransformPoint(endPoint), 
                out int direction, 
                out float distance
            );
            
            Vector3 position = Vector3.zero;
            position[direction] = distance * 0.5f;

            float height = Mathf.Abs(distance);
            float radius = height * 0.15f;

            return new VolumeCapsule(
                upperBone, 
                weight,
                joint,
                position,
                height,
                radius,
                (VolumeCapsule.Direction) direction
            );
        }
    }
}