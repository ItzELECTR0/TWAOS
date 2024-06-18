using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public interface IVolume
    {
        public float Weight { get; }

        GameObject UpdatePass1Physics(Animator animator, float mass, Skeleton skeleton);
        void UpdatePass2Joints(GameObject bone, Animator animator, Skeleton skeleton);
        
        void DrawGizmos(Animator animator, Volumes.Display display);
    }
}