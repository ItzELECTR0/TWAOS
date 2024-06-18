using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    public interface IShotSystem
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        int Id { get; }
        
        // METHODS: -------------------------------------------------------------------------------
        
        void OnAwake(TShotType shotType);
        void OnStart(TShotType shotType);
        void OnDestroy(TShotType shotType);

        void OnUpdate(TShotType shotType);

        void OnEnable(TShotType shotType, TCamera camera);
        void OnDisable(TShotType shotType, TCamera camera);

        void OnDrawGizmos(TShotType shotType, Transform transform);
        void OnDrawGizmosSelected(TShotType shotType, Transform transform);
    }
}