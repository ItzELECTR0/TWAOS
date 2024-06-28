using System;
using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Pawns
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Creator/Gameplay/Pawn")]
    
    [Icon(DaimahouPaths.GIZMOS + "GizmoPawn.png")]
    public class Pawn : MonoBehaviour
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeReference] private List<Feature> m_Features = new();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Transform m_Transform;
        private readonly FiniteStateMachine m_StateMachine = new(typeof(DefaultState));
        private readonly List<IPawnState> m_States = new();
        
        private Message<IPawnMessage> m_Message;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public IPawnState CurrentState => m_StateMachine.CurrentState as IPawnState;
        public Vector3 Position => Transform.position;
        public Transform Transform => m_Transform == null ? m_Transform = transform : m_Transform;

        // ---| Events --------------------------------------------------------------------------------------------->|

        public Message<IPawnMessage> Message => m_Message ??= new Message<IPawnMessage>();
        
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        private void Awake()
        {
            ((IPawnState) m_StateMachine.DefaultState).Initialize(this);
            
            foreach (var feature in m_Features.Cast<IFeature>()) feature.Awake();
        }

        private void Start()
        {
            foreach (var feature in m_Features.Cast<IFeature>()) feature.Start();
        }

        private void OnEnable()
        {
            foreach (var feature in m_Features.Cast<IFeature>()) feature.Enable();
        }

        private void OnDisable()
        {
            foreach (var feature in m_Features.Cast<IFeature>()) feature.Disable();
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public virtual T GetState<T>() where T : class, IPawnState
        {
            if (m_States.Any(s => s is T))
            {
                return m_States.First(s => s is T) as T;
            }
            
            var state = (T) Activator.CreateInstance(typeof(T), m_StateMachine);
            state.Initialize(this);

            m_States.Add(state);
            return state;
        }
        
        public bool TryGetFeature<T>(out T feature) where T : Feature
        {
            feature = m_Features.FirstOrDefault(m => m is T) as T;
            return feature != null;
        }

        public T GetFeature<T>() where T : Feature, new()
        {
            return m_Features.FirstOrDefault(m => m is T) as T;
        }

        public Feature GetFeature(Type type)
        {
            return m_Features.FirstOrDefault(m => m.GetType() == type).RequiredOn(gameObject, type.Name);
        }

        public void FaceLocation(Location location)
        {
            transform.LookAt(location.GetPosition(gameObject));
        }

        public override string ToString() => gameObject.name;

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void Update()
        {
            m_StateMachine.CurrentState.Update();
        }

        //============================================================================================================||
    }
}