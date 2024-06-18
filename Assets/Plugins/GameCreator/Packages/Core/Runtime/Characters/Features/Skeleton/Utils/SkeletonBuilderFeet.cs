using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeFootL(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.LeftLowerLeg),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new SpringLimit(260f, 16f),
                new SpringLimit(260f, 16f),
                new TetherLimit(-25f, 0.3f, 10f),
                new TetherLimit(25f, 0.3f, 10f),
                new TetherLimit(20f, 0.3f, 4f),
                new TetherLimit(20f, 0.3f, 4f)
            );
            
            return MakeFootLimb(
                animator,
                weight,
                HumanBodyBones.LeftLowerLeg,
                HumanBodyBones.LeftFoot,
                configurableJoint
            );
        }
        
        private static IVolume MakeFootR(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.RightLowerLeg),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new SpringLimit(260f, 16f),
                new SpringLimit(260f, 16f),
                new TetherLimit(-25f, 0.3f, 10f),
                new TetherLimit(25f, 0.3f, 10f),
                new TetherLimit(20f, 0.3f, 4f),
                new TetherLimit(20f, 0.3f, 4f)
            );
            
            return MakeFootLimb(
                animator,
                weight,
                HumanBodyBones.RightLowerLeg,
                HumanBodyBones.RightFoot,
                configurableJoint
            );
        }
        
        private static IVolume MakeFootLimb(
            Animator animator, float weight,
            HumanBodyBones parentBone,
            HumanBodyBones bone,
            IJoint joint
        )
        {
            Transform parentTransform = animator.GetBoneTransform(parentBone);
            Transform boneTransform = animator.GetBoneTransform(bone);

            float upperHeight = Vector3.Distance(boneTransform.position, parentTransform.position);
            
            float height = upperHeight * 0.5f;
            float radius = upperHeight * 0.15f;

            CalculateDirection(
                boneTransform.TransformPoint(Vector3.forward),
                out int direction,
                out float _
            );

            Vector3 position = new Vector3(0f, -radius, height * 0.5f - radius);
            position += boneTransform.position;

            return new VolumeCapsule(
                bone, 
                weight,
                joint,
                boneTransform.InverseTransformPoint(position), 
                height,
                radius,
                (VolumeCapsule.Direction) direction
            );
        }
    }
}