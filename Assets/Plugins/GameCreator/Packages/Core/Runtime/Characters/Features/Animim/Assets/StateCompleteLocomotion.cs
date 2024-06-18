using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.Characters
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/characters/animation/states")]
    [Icon(RuntimePaths.GIZMOS + "GizmoStateLocomotion.png")]
    public class StateCompleteLocomotion : StateOverrideAnimator
    {
        private enum AirborneMode
        {
            Single,
            Vertical,
            Directional
        }

        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private AirborneMode m_AirborneMode = AirborneMode.Single;
        
        [SerializeField] private Stand16Points m_Stand16Points = new Stand16Points();
        [SerializeField] private Crouch16Points m_Land16Points = new Crouch16Points();
        
        [SerializeField] private AirborneSingle m_AirborneSingle = new AirborneSingle();
        [SerializeField] private AirborneVertical m_AirborneVertical = new AirborneVertical();
        [SerializeField] private AirborneDirectional m_AirborneDirectional = new AirborneDirectional();

        // SERIALIZATION CALLBACKS: ---------------------------------------------------------------
        
        private const string N_AIR_UP_I = "Human@Air_Up_I";
        private const string N_AIR_UP_F = "Human@Air_Up_F";
        private const string N_AIR_UP_B = "Human@Air_Up_B";
        private const string N_AIR_UP_R = "Human@Air_Up_R";
        private const string N_AIR_UP_L = "Human@Air_Up_L";
        
        private const string N_AIR_DOWN_I = "Human@Air_Down_I";
        private const string N_AIR_DOWN_F = "Human@Air_Down_F";
        private const string N_AIR_DOWN_B = "Human@Air_Down_B";
        private const string N_AIR_DOWN_R = "Human@Air_Down_R";
        private const string N_AIR_DOWN_L = "Human@Air_Down_L";

        private const string N_CROUCH_IDLE    = "Human@Crouch_Idle";
        private const string N_CROUCH_FAST_F  = "Human@Crouch_Fast_F";
        private const string N_CROUCH_FAST_B  = "Human@Crouch_Fast_B";
        private const string N_CROUCH_FAST_R  = "Human@Crouch_Fast_R";
        private const string N_CROUCH_FAST_L  = "Human@Crouch_Fast_L";
        private const string N_CROUCH_FAST_FR = "Human@Crouch_Fast_FR";
        private const string N_CROUCH_FAST_FL = "Human@Crouch_Fast_FL";
        private const string N_CROUCH_FAST_BR = "Human@Crouch_Fast_BR";
        private const string N_CROUCH_FAST_BL = "Human@Crouch_Fast_BL";
        private const string N_CROUCH_SLOW_F  = "Human@Crouch_Slow_F";
        private const string N_CROUCH_SLOW_B  = "Human@Crouch_Slow_B";
        private const string N_CROUCH_SLOW_R  = "Human@Crouch_Slow_R";
        private const string N_CROUCH_SLOW_L  = "Human@Crouch_Slow_L";
        private const string N_CROUCH_SLOW_FR = "Human@Crouch_Slow_FR";
        private const string N_CROUCH_SLOW_FL = "Human@Crouch_Slow_FL";
        private const string N_CROUCH_SLOW_BR = "Human@Crouch_Slow_BR";
        private const string N_CROUCH_SLOW_BL = "Human@Crouch_Slow_BL";
        
        private const string N_STAND_IDLE    = "Human@Stand_Idle";
        private const string N_STAND_FAST_F  = "Human@Stand_Fast_F";
        private const string N_STAND_FAST_B  = "Human@Stand_Fast_B";
        private const string N_STAND_FAST_R  = "Human@Stand_Fast_R";
        private const string N_STAND_FAST_L  = "Human@Stand_Fast_L";
        private const string N_STAND_FAST_FR = "Human@Stand_Fast_FR";
        private const string N_STAND_FAST_FL = "Human@Stand_Fast_FL";
        private const string N_STAND_FAST_BR = "Human@Stand_Fast_BR";
        private const string N_STAND_FAST_BL = "Human@Stand_Fast_BL";
        private const string N_STAND_SLOW_F  = "Human@Stand_Slow_F";
        private const string N_STAND_SLOW_B  = "Human@Stand_Slow_B";
        private const string N_STAND_SLOW_R  = "Human@Stand_Slow_R";
        private const string N_STAND_SLOW_L  = "Human@Stand_Slow_L";
        private const string N_STAND_SLOW_FR = "Human@Stand_Slow_FR";
        private const string N_STAND_SLOW_FL = "Human@Stand_Slow_FL";
        private const string N_STAND_SLOW_BR = "Human@Stand_Slow_BR";
        private const string N_STAND_SLOW_BL = "Human@Stand_Slow_BL";
        
        protected sealed override void BeforeSerialize()
        {
            if (this.m_Controller == null) return;
            
            this.m_Controller[N_STAND_IDLE]     = this.m_Stand16Points.m_Idle;
            this.m_Controller[N_STAND_FAST_F]   = this.m_Stand16Points.m_ForwardFast;
            this.m_Controller[N_STAND_FAST_B]   = this.m_Stand16Points.m_BackwardFast;
            this.m_Controller[N_STAND_FAST_R]   = this.m_Stand16Points.m_RightFast;
            this.m_Controller[N_STAND_FAST_L]   = this.m_Stand16Points.m_LeftFast;
            this.m_Controller[N_STAND_FAST_FR]  = this.m_Stand16Points.m_ForwardRightFast;
            this.m_Controller[N_STAND_FAST_FL]  = this.m_Stand16Points.m_ForwardLeftFast;
            this.m_Controller[N_STAND_FAST_BR]  = this.m_Stand16Points.m_BackwardRightFast;
            this.m_Controller[N_STAND_FAST_BL]  = this.m_Stand16Points.m_BackwardLeftFast;
            this.m_Controller[N_STAND_SLOW_F]   = this.m_Stand16Points.m_ForwardSlow;
            this.m_Controller[N_STAND_SLOW_B]   = this.m_Stand16Points.m_BackwardSlow;
            this.m_Controller[N_STAND_SLOW_R]   = this.m_Stand16Points.m_RightSlow;
            this.m_Controller[N_STAND_SLOW_L]   = this.m_Stand16Points.m_LeftSlow;
            this.m_Controller[N_STAND_SLOW_FR]  = this.m_Stand16Points.m_ForwardRightSlow;
            this.m_Controller[N_STAND_SLOW_FL]  = this.m_Stand16Points.m_ForwardLeftSlow;
            this.m_Controller[N_STAND_SLOW_BR]  = this.m_Stand16Points.m_BackwardRightSlow;
            this.m_Controller[N_STAND_SLOW_BL]  = this.m_Stand16Points.m_BackwardLeftSlow;
            this.m_Controller[N_CROUCH_IDLE]    = this.m_Land16Points.m_Idle;
            this.m_Controller[N_CROUCH_FAST_F]  = this.m_Land16Points.m_ForwardFast;
            this.m_Controller[N_CROUCH_FAST_B]  = this.m_Land16Points.m_BackwardFast;
            this.m_Controller[N_CROUCH_FAST_R]  = this.m_Land16Points.m_RightFast;
            this.m_Controller[N_CROUCH_FAST_L]  = this.m_Land16Points.m_LeftFast;
            this.m_Controller[N_CROUCH_FAST_FR] = this.m_Land16Points.m_ForwardRightFast;
            this.m_Controller[N_CROUCH_FAST_FL] = this.m_Land16Points.m_ForwardLeftFast;
            this.m_Controller[N_CROUCH_FAST_BR] = this.m_Land16Points.m_BackwardRightFast;
            this.m_Controller[N_CROUCH_FAST_BL] = this.m_Land16Points.m_BackwardLeftFast;
            this.m_Controller[N_CROUCH_SLOW_F]  = this.m_Land16Points.m_ForwardSlow;
            this.m_Controller[N_CROUCH_SLOW_B]  = this.m_Land16Points.m_BackwardSlow;
            this.m_Controller[N_CROUCH_SLOW_R]  = this.m_Land16Points.m_RightSlow;
            this.m_Controller[N_CROUCH_SLOW_L]  = this.m_Land16Points.m_LeftSlow;
            this.m_Controller[N_CROUCH_SLOW_FR] = this.m_Land16Points.m_ForwardRightSlow;
            this.m_Controller[N_CROUCH_SLOW_FL] = this.m_Land16Points.m_ForwardLeftSlow;
            this.m_Controller[N_CROUCH_SLOW_BR] = this.m_Land16Points.m_BackwardRightSlow;
            this.m_Controller[N_CROUCH_SLOW_BL] = this.m_Land16Points.m_BackwardLeftSlow;

            switch (this.m_AirborneMode)
            {
                case AirborneMode.Single:
                    this.m_Controller[N_AIR_UP_I] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_UP_F] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_UP_B] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_UP_R] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_UP_L] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_DOWN_I] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_DOWN_F] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_DOWN_B] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_DOWN_R] = this.m_AirborneSingle.m_OnAir;
                    this.m_Controller[N_AIR_DOWN_L] = this.m_AirborneSingle.m_OnAir;
                    break;
                
                case AirborneMode.Vertical:
                    this.m_Controller[N_AIR_UP_I] = this.m_AirborneVertical.m_Up;
                    this.m_Controller[N_AIR_UP_F] = this.m_AirborneVertical.m_Up;
                    this.m_Controller[N_AIR_UP_B] = this.m_AirborneVertical.m_Up;
                    this.m_Controller[N_AIR_UP_R] = this.m_AirborneVertical.m_Up;
                    this.m_Controller[N_AIR_UP_L] = this.m_AirborneVertical.m_Up;
                    this.m_Controller[N_AIR_DOWN_I] = this.m_AirborneVertical.m_Down;
                    this.m_Controller[N_AIR_DOWN_F] = this.m_AirborneVertical.m_Down;
                    this.m_Controller[N_AIR_DOWN_B] = this.m_AirborneVertical.m_Down;
                    this.m_Controller[N_AIR_DOWN_R] = this.m_AirborneVertical.m_Down;
                    this.m_Controller[N_AIR_DOWN_L] = this.m_AirborneVertical.m_Down;
                    break;
                
                case AirborneMode.Directional:
                    this.m_Controller[N_AIR_UP_I] = this.m_AirborneDirectional.m_UpIdle;
                    this.m_Controller[N_AIR_UP_F] = this.m_AirborneDirectional.m_UpForward;
                    this.m_Controller[N_AIR_UP_B] = this.m_AirborneDirectional.m_UpBackward;
                    this.m_Controller[N_AIR_UP_R] = this.m_AirborneDirectional.m_UpRight;
                    this.m_Controller[N_AIR_UP_L] = this.m_AirborneDirectional.m_UpLeft;
                    this.m_Controller[N_AIR_DOWN_I] = this.m_AirborneDirectional.m_DownIdle;
                    this.m_Controller[N_AIR_DOWN_F] = this.m_AirborneDirectional.m_DownForward;
                    this.m_Controller[N_AIR_DOWN_B] = this.m_AirborneDirectional.m_DownBackward;
                    this.m_Controller[N_AIR_DOWN_R] = this.m_AirborneDirectional.m_DownRight;
                    this.m_Controller[N_AIR_DOWN_L] = this.m_AirborneDirectional.m_DownLeft;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected sealed override void AfterSerialize()
        { }
    }
}