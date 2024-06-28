using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Pawns
{
    [Serializable]
    public class MoveState : TPawnState
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|

        private const float STUCK_TIMEOUT = 1;
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Vector3 m_CurrentPosition;
        private float m_Distance;
        private float m_StuckTimer;
        
        private Location m_Destination;
        private CharacterController m_Controller;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string[] Info { get; } = new string[1];
        public override string Title => "Movement State";
        public float Speed { get; set; }
        public string LastDestination => m_Destination.GetPosition(Pawn.gameObject).ToString();

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        
        public MoveState(FiniteStateMachine ownerFiniteStateMachine) : base(ownerFiniteStateMachine) {}

        protected override void Initialize()
        {
            m_Controller = Pawn.GetComponent<CharacterController>();
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public bool TryEnter(Vector3 destination)
        {
            m_Destination = new Location(destination);
            return StateMachine.TryResetState(this);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override void OnEnterState()
        {
            m_StuckTimer = 0;
            
            if (!Character) return;
            
            Character.Gestures.Stop(0, .25f);
            Character.Motion.MoveToLocation(m_Destination, 0.1f, null, 0);
        }

        protected override void OnExitState()
        {
            Character.Kernel.Motion.MoveToDirection(Vector3.zero, Space.World, 0);
        }

        protected override void Update()
        {
            var currentPosition = Pawn.Position;

            m_StuckTimer = currentPosition == m_CurrentPosition ? m_StuckTimer + Time.deltaTime : 0;
            
            m_Distance = VectorHelper.Distance2D(Pawn.Position, m_Destination.GetPosition(Pawn.gameObject));
            m_CurrentPosition = currentPosition;
            
            if(m_StuckTimer > STUCK_TIMEOUT || m_Distance < 0.1f)
            {
                Info[0] = $"Last destination: {m_Destination.GetPosition(Pawn.gameObject)}";
                ForceDefaultState();
                return;
            }
            
            Info[0] = $"Moving to {m_Destination.GetPosition(Pawn.gameObject)}";

            if (Character) return;
            
            m_Controller.Move(UpdateMoveToPosition());
        }

        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private Vector3 UpdateMoveToPosition()
        {
            var velocity = Speed;

            const int brakeRadiusHeuristic = 1;
            
            if (m_Distance < brakeRadiusHeuristic)
            {
                velocity = Mathf.Lerp(
                    velocity, velocity * 0.25f,
                    1f - Mathf.Clamp01(m_Distance / brakeRadiusHeuristic)
                );
            }

            var direction = m_Destination.GetPosition(Pawn.gameObject) - m_CurrentPosition;
            return direction.normalized * (velocity * Time.deltaTime);
        }

        //============================================================================================================||
    }
}