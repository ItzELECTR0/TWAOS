using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeLowerLimb(
            Animator animator, 
            float weight,
            HumanBodyBones lowerBone,
            HumanBodyBones parentBone,
            IJoint joint
        )
        {
            Transform lowerBoneTransform = animator.GetBoneTransform(lowerBone);
            Transform parentBoneTransform = animator.GetBoneTransform(parentBone);

            Vector3 endPoint =
                lowerBoneTransform.position - parentBoneTransform.position +
                lowerBoneTransform.position;

            CalculateDirection(
                lowerBoneTransform.InverseTransformPoint(endPoint),
                out int direction,
                out float distance
            );

            Vector3 position = Vector3.zero;
            position[direction] = distance * 0.5f;

            float height = Mathf.Abs(distance);
            float radius = height * 0.25f;

            return new VolumeCapsule(
                parentBone,
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