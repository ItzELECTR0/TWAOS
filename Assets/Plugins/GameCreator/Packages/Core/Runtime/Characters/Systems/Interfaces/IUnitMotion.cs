using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Motion")]
    
    public interface IUnitMotion : IUnitCommon
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        Vector3 MoveDirection { get; }
        Vector3 MovePosition  { get; }
        Vector3 MoveRotation  { get; }

        float StopThreshold { get; }

        float FollowMinDistance { get; }
        float FollowMaxDistance { get; }

        Character.MovementType MovementType { get; set; }
        
        float LinearSpeed      { get; set; }
        float AngularSpeed  { get; set; }

        AnimFloat StandLevel { get; }

        float Mass { get; set; }
        float Height { get; set; }
        float Radius { get; set; }

        bool UseAcceleration { get; set; }
        float Acceleration   { get; set; }
        float Deceleration   { get; set; }

        bool CanJump { get; set; }
        int AirJumps { get; set; }
        
        bool IsJumping { get; }
        float IsJumpingForce { get; }

        float GravityUpwards   { get; set; }
        float GravityDownwards { get; set; }
        float TerminalVelocity { get; set; }

        float JumpForce    { get; set; }
        float JumpCooldown { get; set; }
        
        int DashInSuccession { get; set; }
        bool DashInAir { get; set; }
        float DashCooldown { get; set; }

        float InteractionRadius { get; set; }
        InteractionMode InteractionMode { get; set; }

        // MOVE METHODS: --------------------------------------------------------------------------

        void SetMotionTransient(Vector3 direction, float speed, float duration, float fade);
        
        void MoveToDirection(Vector3 velocity, Space space, int priority = 1);
        void StopToDirection(int priority = 1);

        void MoveToLocation(Location location, float stopDistance, Action<Character, bool> onFinish, int priority = 1);
        void MoveToTransform(Transform target, float stopDistance, Action<Character, bool> onFinish, int priority = 1);
        void MoveToMarker(Marker marker, float stopDistance, Action<Character, bool> onFinish, int priority = 1);

        void StartFollowingTarget(Transform target, float minRadius, float maxRadius, int priority = 1);
        void StopFollowingTarget(int priority = 1);

        // OTHER METHODS: -------------------------------------------------------------------------
        
        void Jump();
        void Jump(float force);
    }
}