using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Driver")]
    
    public interface IUnitDriver : IUnitCommon
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        Vector3 WorldMoveDirection { get; }
        Vector3 LocalMoveDirection { get; }
        
        float SkinWidth { get; }
        bool IsGrounded { get; }
        Vector3 FloorNormal { get; }
        
        float GravityInfluence { get; }

        bool Collision { get; set; }
        Axonometry Axonometry { get; set; }
        
        // POSITION MODIFIERS: --------------------------------------------------------------------

        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);
        void SetScale(Vector3 scale);

        void AddPosition(Vector3 amount);
        void AddRotation(Quaternion amount);
        void AddScale(Vector3 amount);
        
        // GRAVITY METHODS: -----------------------------------------------------------------------

        void ResetVerticalVelocity();
        void SetGravityInfluence(int key, float influence);
        void RemoveGravityInfluence(int key);
    }
}