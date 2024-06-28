using System;
using System.Linq;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaimahouGames.Runtime.Core
{
    [Serializable]
    public class InputMapping
    {
        //============================================================================================================||\

        [Serializable]
        public class Mapping : IGenericItem
        {
            public string ActionMap; 
            public string Action;
            
#if UNITY_EDITOR
            public string Title => $"{ActionMap} -> {Action}";
            public Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
            public bool IsExpanded { get; set; }
            public string[] Info { get; } = Array.Empty<string>();
#endif
        }

        public class ButtonMessage
        {
            private readonly Message<InputAction.CallbackContext> m_OnStarted = new();
            private readonly Message<InputAction.CallbackContext> m_OnPerformed = new();
            private readonly Message<InputAction.CallbackContext> m_OnCanceled = new();

            public ButtonMessage(InputAction inputAction)
            {
                inputAction.started += m_OnStarted.Send;
                inputAction.performed += m_OnPerformed.Send;
                inputAction.canceled += m_OnCanceled.Send;
            }

            public MessageReceipt OnStarted(Action<InputAction.CallbackContext> onStart)
            {
                return m_OnStarted.Subscribe(onStart);
            }

            public MessageReceipt OnPerformed(Action<InputAction.CallbackContext> onStart)
            {
                return m_OnPerformed.Subscribe(onStart);
            }

            public MessageReceipt OnCanceled(Action<InputAction.CallbackContext> onStart)
            {
                return m_OnCanceled.Subscribe(onStart);
            }
        }
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeReference] private Mapping[] m_InputSettings;

        // ---|　Internal State ------------------------------------------------------------------------------------->|
        
        [NonSerialized] private InputAction[] m_InputAction;
        [NonSerialized] private ButtonMessage[] m_InputMessages;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        private InputAction this[int index] => m_InputAction[index];
        public int Count => m_InputSettings.Length;

        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|

        public void Initialize(InputActionAsset inputAsset)
        {
            // Debug.Log($"initializing {GetType().GetNiceName()} [{m_InputSettings.Length} actions]");
            
            m_InputMessages = new ButtonMessage[m_InputSettings.Length];
            m_InputAction = new InputAction[m_InputSettings.Length];

            for (var i = 0; i < m_InputSettings.Length; i++)
            {
                if (m_InputSettings.Count() <= i)
                {
                    Debug.LogError($"No input set at index {i}");
                    continue;
                }
            
                if (inputAsset == null)
                {
                    Debug.LogError("Input Action Asset not found");
                    return;
                }

                var map = inputAsset.FindActionMap(m_InputSettings[i].ActionMap);
                if (map != null)
                {
                    m_InputAction[i] = inputAsset.FindAction(m_InputSettings[i].Action);
                    if(m_InputAction[i] != null)
                    {
                        // Debug.Log($"\tInput set [{i}] > [{m_InputAction[i]?.name ?? "(null)"}]");
                        continue;
                    }
                }
            
                Debug.LogErrorFormat(
                    "Unable to find Input Action for asset: {0}. Map: {1} and Action: {2}",
                    inputAsset != null ? inputAsset.name : "(null)",
                    m_InputSettings[i].ActionMap,
                    m_InputSettings[i].Action
                );
            }
        }
        
        // ※  Public Methods: ----------------------------------------------------------------------------------------|

        public void Enable()
        {
            foreach (var inputAction in m_InputAction)
            {
                inputAction.Enable();
            }
        }

        public void Disable()
        {
            foreach (var inputAction in m_InputAction)
            {
                inputAction.Disable();
            }
        }

        public ButtonMessage GetMessages(int index)
        {
            return m_InputMessages[index] ?? (m_InputMessages[index] = new ButtonMessage(this[index]));
        }
        
        public void ForEach(Action<ButtonMessage, int> action)
        {
            for (var i = 0; i < m_InputSettings.Length; i++)
            {
                action(GetMessages(i), i);
            }
        }
        
        public void For(Action<InputAction, int> action)
        {
            for (var i = 0; i < m_InputSettings.Length; i++)
            {
                action(this[i], i);
            }
        }
        
        public InputAction GetInputIndex(int slot)
        {
            if(m_InputAction == null) return default;
            if(slot >= 0 && slot < m_InputAction.Length) return m_InputAction[slot];

            return default;
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}