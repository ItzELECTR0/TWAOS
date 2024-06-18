using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.Characters
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/characters/animation/states")]
    [Icon(RuntimePaths.GIZMOS + "GizmoStateLocomotion.png")]
    public class StateBasicLocomotion : StateOverrideAnimator
    {
        private enum AirborneMode
        {
            Single,
            Vertical,
            Directional
        }

        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private AirborneMode m_AirborneMode = AirborneMode.Single;

        [SerializeField] private Stand8Points m_Stand8Points = new Stand8Points();
        [SerializeField] private Crouch8Points m_Land8Points = new Crouch8Points();

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
        private const string N_CROUCH_F  = "Human@Crouch_Fast_F";
        private const string N_CROUCH_B  = "Human@Crouch_Fast_B";
        private const string N_CROUCH_R  = "Human@Crouch_Fast_R";
        private const string N_CROUCH_L  = "Human@Crouch_Fast_L";
        private const string N_CROUCH_FR = "Human@Crouch_Fast_FR";
        private const string N_CROUCH_FL = "Human@Crouch_Fast_FL";
        private const string N_CROUCH_BR = "Human@Crouch_Fast_BR";
        private const string N_CROUCH_BL = "Human@Crouch_Fast_BL";

        private const string N_STAND_IDLE    = "Human@Stand_Idle";
        private const string N_STAND_F  = "Human@Stand_Fast_F";
        private const string N_STAND_B  = "Human@Stand_Fast_B";
        private const string N_STAND_R  = "Human@Stand_Fast_R";
        private const string N_STAND_L  = "Human@Stand_Fast_L";
        private const string N_STAND_FR = "Human@Stand_Fast_FR";
        private const string N_STAND_FL = "Human@Stand_Fast_FL";
        private const string N_STAND_BR = "Human@Stand_Fast_BR";
        private const string N_STAND_BL = "Human@Stand_Fast_BL";

        protected sealed override void BeforeSerialize()
        {
            if (this.m_Controller == null) return;
            
            this.m_Controller[N_STAND_IDLE] = this.m_Stand8Points.m_Idle;
            this.m_Controller[N_STAND_F]    = this.m_Stand8Points.m_Forward;
            this.m_Controller[N_STAND_B]    = this.m_Stand8Points.m_Backward;
            this.m_Controller[N_STAND_R]    = this.m_Stand8Points.m_Right;
            this.m_Controller[N_STAND_L]    = this.m_Stand8Points.m_Left;
            this.m_Controller[N_STAND_FR]   = this.m_Stand8Points.m_ForwardRight;
            this.m_Controller[N_STAND_FL]   = this.m_Stand8Points.m_ForwardLeft;
            this.m_Controller[N_STAND_BR]   = this.m_Stand8Points.m_BackwardRight;
            this.m_Controller[N_STAND_BL]   = this.m_Stand8Points.m_BackwardLeft;
            this.m_Controller[N_STAND_F]    = this.m_Stand8Points.m_Forward;
            this.m_Controller[N_STAND_B]    = this.m_Stand8Points.m_Backward;
            this.m_Controller[N_STAND_R]    = this.m_Stand8Points.m_Right;
            this.m_Controller[N_STAND_L]    = this.m_Stand8Points.m_Left;
            this.m_Controller[N_STAND_FR]   = this.m_Stand8Points.m_ForwardRight;
            this.m_Controller[N_STAND_FL]   = this.m_Stand8Points.m_ForwardLeft;
            this.m_Controller[N_STAND_BR]   = this.m_Stand8Points.m_BackwardRight;
            this.m_Controller[N_STAND_BL]   = this.m_Stand8Points.m_BackwardLeft;
            this.m_Controller[N_CROUCH_IDLE] = this.m_Land8Points.m_Idle;
            this.m_Controller[N_CROUCH_F]    = this.m_Land8Points.m_Forward;
            this.m_Controller[N_CROUCH_B]    = this.m_Land8Points.m_Backward;
            this.m_Controller[N_CROUCH_R]    = this.m_Land8Points.m_Right;
            this.m_Controller[N_CROUCH_L]    = this.m_Land8Points.m_Left;
            this.m_Controller[N_CROUCH_FR]   = this.m_Land8Points.m_ForwardRight;
            this.m_Controller[N_CROUCH_FL]   = this.m_Land8Points.m_ForwardLeft;
            this.m_Controller[N_CROUCH_BR]   = this.m_Land8Points.m_BackwardRight;
            this.m_Controller[N_CROUCH_BL]   = this.m_Land8Points.m_BackwardLeft;
            this.m_Controller[N_CROUCH_F]    = this.m_Land8Points.m_Forward;
            this.m_Controller[N_CROUCH_B]    = this.m_Land8Points.m_Backward;
            this.m_Controller[N_CROUCH_R]    = this.m_Land8Points.m_Right;
            this.m_Controller[N_CROUCH_L]    = this.m_Land8Points.m_Left;
            this.m_Controller[N_CROUCH_FR]   = this.m_Land8Points.m_ForwardRight;
            this.m_Controller[N_CROUCH_FL]   = this.m_Land8Points.m_ForwardLeft;
            this.m_Controller[N_CROUCH_BR]   = this.m_Land8Points.m_BackwardRight;
            this.m_Controller[N_CROUCH_BL]   = this.m_Land8Points.m_BackwardLeft;

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