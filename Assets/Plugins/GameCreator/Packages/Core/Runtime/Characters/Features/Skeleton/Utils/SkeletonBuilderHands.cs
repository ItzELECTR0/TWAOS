using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public static partial class SkeletonBuilder
    {
        private static IVolume MakeHandL(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.LeftLowerArm),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, 0, -1),
                new Vector3(0, -1, 0),
                new SpringLimit(160f, 10f),
                new SpringLimit(230f, 15f),
                new TetherLimit(-2f, 0.3f, 20f),
                new TetherLimit(95f, 0.3f, 20f),
                new TetherLimit(20f, 0.3f, 4f),
                new TetherLimit(15f, 0.3f, 3f)
            );
            
            return MakeHandLimb(
                animator,
                weight,
                HumanBodyBones.LeftHand,
                HumanBodyBones.LeftLowerArm,
                configurableJoint
            );
        }
        
        private static IVolume MakeHandR(Animator animator, float weight)
        {
            JointConfigurable configurableJoint = new JointConfigurable(
                new Bone(HumanBodyBones.RightLowerArm),
                ConfigurableJointMotion.Locked,
                ConfigurableJointMotion.Limited,
                new Vector3(0, 0, 1),
                new Vector3(0, -1, 0),
                new SpringLimit(160f, 10f),
                new SpringLimit(230f, 15f),
                new TetherLimit(-2f, 0.3f, 20f),
                new TetherLimit(95f, 0.3f, 20f),
                new TetherLimit(20f, 0.3f, 4f),
                new TetherLimit(15f, 0.3f, 3f)
            );
            
            return MakeHandLimb(
                animator,
                weight,
                HumanBodyBones.RightHand,
                HumanBodyBones.RightLowerArm,
                configurableJoint
            );
        }
        
        private static IVolume MakeHandLimb(
            Animator animator, float weight,
            HumanBodyBones bone,
            HumanBodyBones parentBone,
            IJoint joint
        )
        {
            Transform boneTransform = animator.GetBoneTransform(bone);
            Transform parentTransform = animator.GetBoneTransform(parentBone);

            Vector3 vector = boneTransform.position - parentTransform.position;
            float height = Mathf.Abs(vector.x * 0.75f);
            float radius = Mathf.Abs(vector.x * 0.15f);

            CalculateDirection(
                boneTransform.TransformPoint(Vector3.right),
                out int direction,
                out float _
            );

            Vector3 position = new Vector3(Mathf.Sign(vector.x) * height * 0.5f, 0f, 0f);
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