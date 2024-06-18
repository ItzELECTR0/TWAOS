using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Camera Shot")]
    
    public interface IShotType
    {
        bool IsActive { get; }
        Transform[] Ignore { get; }
        
        ShotCamera ShotCamera { get; }
        IShotSystem[] ShotSystems { get; }
        
        Vector3 Position { get; }
        Quaternion Rotation { get; }

        bool UseSmoothPosition { get; }
        bool UseSmoothRotation { get; }

        Transform Target { get; }
        bool HasObstacle { get; }
        
        Args Args { get; }

        // METHODS: -------------------------------------------------------------------------------
        
        void Awake(ShotCamera shotCamera);
        void Start(ShotCamera shotCamera);
        void Destroy(ShotCamera shotCamera);

        void Update();

        void OnEnable(TCamera camera);
        void OnDisable(TCamera camera);

        void DrawGizmos(Transform transform);
        void DrawGizmosSelected(Transform transform);
        
        // GETTERS: -------------------------------------------------------------------------------

        public IShotSystem GetSystem(int systemID);
    }
}