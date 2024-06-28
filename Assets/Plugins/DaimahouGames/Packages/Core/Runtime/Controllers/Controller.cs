using System;
using System.Linq;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaimahouGames.Runtime.Core
{
    [CreateAssetMenu(menuName = "Game Creator/Input/Controller")]
    public class Controller : ScriptableObject
    {
        //============================================================================================================||
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        
        [SerializeField] private UniqueID m_UID = new(); 
        
        [SerializeField] private InputActionAsset m_InputAsset;
        [SerializeReference] private InputModule[] m_InputModules;
        
        // ---|　Internal State ---------------------------------------------------------------------------------->|

        private Message<Pawn> m_PawnChangedMessage;
        private Pawn m_Pawn;

        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        
        public IdString ID => m_UID.Get;
        
        // ---|　Events ------------------------------------------------------------------------------------------>|

        private Message<Pawn> PawnChangedMessage => m_PawnChangedMessage ??= new Message<Pawn>();
        public MessageReceipt OnPawnChanged(Action<Pawn> action) => PawnChangedMessage.Subscribe(action);

        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※

        private void Initialize()
        {
            if (GameMode.TryRegister(this))
            {
                m_Pawn = null;
                m_PawnChangedMessage = null;
                foreach (var inputModule in m_InputModules) inputModule.Initialize(m_InputAsset);
            }
        }

        // ※  Public Methods: --------------------------------------------------------------------------------------|※
        
        public void Possess(Pawn pawn)
        {
            Initialize();
            
            Debug.Log($"[{name}]: possessing {pawn.name}");
            
            m_Pawn = pawn;
            m_Pawn.Get<Character>().Player.IsControllable = true;
           
            foreach (var module in m_InputModules)
            {
                module.Possess(m_Pawn);
            }

            PawnChangedMessage.Send(m_Pawn);
        }
        
        public Pawn GetPossessedPawn()
        {
            return m_Pawn;
        }
        
        public T GetInputProvider<T>() where T : class, IInputProvider
        {
            return m_InputModules.FirstOrDefault(i => i is T) as T;
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        //============================================================================================================||
    }
}