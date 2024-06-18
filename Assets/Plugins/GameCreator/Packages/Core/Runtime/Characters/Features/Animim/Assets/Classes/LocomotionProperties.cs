using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class LocomotionProperties
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private EnablerBool m_IsControllable = new EnablerBool(false, true);
        
        [SerializeField] private EnablerFloat m_Speed = new EnablerFloat(4f);
        [SerializeField] private EnablerFloat m_Rotation = new EnablerFloat(1800f);
        
        [SerializeField] private EnablerFloat m_Mass = new EnablerFloat(80f);
        [SerializeField] private EnablerFloat m_Height = new EnablerFloat(2f);
        [SerializeField] private EnablerFloat m_Radius = new EnablerFloat(0.2f);
        
        [SerializeField] private EnablerFloat m_GravityUpwards = new EnablerFloat(-9.81f);
        [SerializeField] private EnablerFloat m_GravityDownwards = new EnablerFloat(-9.81f);
        [SerializeField] private EnablerFloat m_TerminalVelocity = new EnablerFloat(-53f);

        [SerializeField] private EnablerBool m_UseAcceleration = new EnablerBool(true);
        [SerializeField] private EnablerFloat m_Acceleration = new EnablerFloat(10f);
        [SerializeField] private EnablerFloat m_Deceleration = new EnablerFloat(4f);
        
        [SerializeField] private EnablerBool m_CanJump = new EnablerBool(true);
        [SerializeField] private EnablerInt m_AirJumps = new EnablerInt(0);
        [SerializeField] private EnablerFloat m_JumpForce = new EnablerFloat(5f);
        [SerializeField] private EnablerFloat m_JumpCooldown = new EnablerFloat(0.5f);
        
        [SerializeField] private EnablerInt m_DashInSuccession = new EnablerInt(0);
        [SerializeField] private EnablerBool m_DashInAir = new EnablerBool(false);
        [SerializeField] private EnablerFloat m_DashCooldown = new EnablerFloat(1f);
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void Update(Character character, float t)
        {
            if (this.m_IsControllable.IsEnabled) character.Player.IsControllable = this.m_IsControllable.Value;
            
            if (this.m_Speed.IsEnabled) character.Motion.LinearSpeed = Mathf.Lerp(
                character.Motion.LinearSpeed, 
                this.m_Speed.Value,
                t
            );
            
            if (this.m_Rotation.IsEnabled) character.Motion.AngularSpeed = Mathf.Lerp(
                character.Motion.AngularSpeed,
                this.m_Rotation.Value,
                t
             );
            
            if (this.m_Mass.IsEnabled) character.Motion.Mass = Mathf.Lerp(
                character.Motion.Mass,
                this.m_Mass.Value,
                t
            );
            
            if (this.m_Height.IsEnabled) character.Motion.Height = Mathf.Lerp(
                character.Motion.Height,
                this.m_Height.Value, 
                t
            );
            
            if (this.m_Radius.IsEnabled) character.Motion.Radius = Mathf.Lerp(
                character.Motion.Radius, 
                this.m_Radius.Value,
                t
            );
            
            if (this.m_GravityUpwards.IsEnabled) character.Motion.GravityUpwards = Mathf.Lerp(
                character.Motion.GravityUpwards,
                this.m_GravityUpwards.Value,
                t
            );
            
            if (this.m_GravityDownwards.IsEnabled) character.Motion.GravityDownwards = Mathf.Lerp(
                character.Motion.GravityDownwards,
                this.m_GravityDownwards.Value,
                t
            );
            
            if (this.m_TerminalVelocity.IsEnabled) character.Motion.TerminalVelocity = Mathf.Lerp(
                character.Motion.TerminalVelocity,
                this.m_TerminalVelocity.Value,
                t
            );
            
            if (this.m_UseAcceleration.IsEnabled) character.Motion.UseAcceleration = this.m_UseAcceleration.Value;
            
            if (this.m_Acceleration.IsEnabled) character.Motion.Acceleration = Mathf.Lerp(
                character.Motion.Acceleration,
                this.m_Acceleration.Value,
                t
            );
            
            if (this.m_Deceleration.IsEnabled) character.Motion.Deceleration = Mathf.Lerp(
                character.Motion.Deceleration,
                this.m_Deceleration.Value,
                t
            );
            
            if (this.m_CanJump.IsEnabled) character.Motion.CanJump = this.m_CanJump.Value;
            if (this.m_AirJumps.IsEnabled) character.Motion.AirJumps = this.m_AirJumps.Value;
            
            if (this.m_JumpForce.IsEnabled) character.Motion.JumpForce = Mathf.Lerp(
                character.Motion.JumpForce,
                this.m_JumpForce.Value,
                t);
            
            if (this.m_JumpCooldown.IsEnabled) character.Motion.JumpCooldown = Mathf.Lerp(
                character.Motion.JumpCooldown, 
                this.m_JumpCooldown.Value,
                t
            );
            
            if (this.m_DashInSuccession.IsEnabled) character.Motion.DashInSuccession = this.m_DashInSuccession.Value;
            if (this.m_DashInAir.IsEnabled) character.Motion.DashInAir = this.m_DashInAir.Value;
            
            if (this.m_DashCooldown.IsEnabled) character.Motion.DashCooldown = Mathf.Lerp(
                character.Motion.DashCooldown,
                this.m_DashCooldown.Value,
                t
            );
        }
    }
}