using System;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    [Serializable]
    public struct Target
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        [SerializeField] private Vector3 m_Position;
        [SerializeField] private GameObject m_GameObject;
        [SerializeField] private bool m_HasPosition;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public Pawn Pawn => m_GameObject ? m_GameObject.Get<Pawn>() : null;
        public Vector3 Position => m_HasPosition ? m_Position : m_GameObject.transform.position;
        public Transform Transform => m_GameObject ? m_GameObject.transform : null;
        public GameObject GameObject => m_GameObject;
        public Quaternion Rotation => Transform != null ? Transform.rotation : Quaternion.identity;
        public bool HasPosition => m_HasPosition;

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        public Target(Pawn pawn)
        {
            m_GameObject = pawn.gameObject;
            m_Position = default;
            m_HasPosition = false;
        }

        public Target(Feature feature)
        {
            m_GameObject = feature.GameObject;
            m_Position = default;
            m_HasPosition = false;
        }

        public Target(GameObject target)
        {
            m_GameObject = target;
            m_Position = default;
            m_HasPosition = false;
        }

        public Target(Vector3 position)
        {
            m_GameObject = default;
            m_Position = position;
            m_HasPosition = true;
        }
        
        public Target(Component target)
        {
            m_GameObject = target.gameObject;
            m_Position = default;
            m_HasPosition = false;
        }

        public Target(Component target, Vector3 position)
        {
            m_GameObject = target.gameObject;
            m_Position = position;
            m_HasPosition = true;
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public Location GetLocation() => m_HasPosition
            ? new Location(Position)
            : new Location(new PositionTowards(this.m_GameObject.transform, Vector3.one, Vector3.zero, 0f),
                new RotationNone());
        
        public void ChangePosition(Vector3 position)
        {
            m_Position = position;
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}