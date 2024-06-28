using System;
using System.Collections.Generic;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaimahouGames.Runtime.Core
{
    [Serializable]
    public abstract class InputModule : IGenericItem
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        #region EditorInfo
        [SerializeField] private bool m_IsExpanded;
        public virtual string Title { get; } = "feature - (no name)";
        public virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
        public bool IsExpanded { get => m_IsExpanded; set => m_IsExpanded = value; }
        public virtual string[] Info { get; } = Array.Empty<string>();
        #endregion

        public const string ON_INPUT_PRESSED = "ValidateOnInputPressed";
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private InputMapping m_Input;

        // ---|　Internal State ------------------------------------------------------------------------------------->|

        protected Pawn m_Pawn;
        private List<MessageReceipt> m_InputReceipts = new();
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|

        public void Initialize(InputActionAsset inputActionAsset) => m_Input.Initialize(inputActionAsset);

        private void Enable()
        {
            Disable();
            if (m_Input == null) return;

            m_Input.Enable();
            m_Input.ForEach((m, i) =>
            {
                m_InputReceipts.Add(m.OnStarted(_ => Started(i)));
                m_InputReceipts.Add(m.OnPerformed(_ => Performed(i)));
                m_InputReceipts.Add(m.OnCanceled(_ => Canceled(i)));
            });
        }
        
        private void Disable()
        {
            foreach (var receipt in m_InputReceipts)
            {
                receipt.Dispose();
            }
            m_InputReceipts.Clear();
            
            if (m_Input == null) return;

            m_Pawn = null;
            
            m_Input.Disable();
        }
        
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public void Possess(Pawn pawn)
        {
            if(m_Pawn != null) Disable();
            if(pawn != null) Enable();
            
            m_Pawn = pawn;
        }

        public string GetInputName(int slot)
        {
            if(m_Input == null) return string.Empty;
            return m_Input.Count <= slot ? "not defined" : m_Input.GetInputIndex(slot)?.GetBindingDisplayString();
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|

        protected virtual void Started(int index)
        {
            //Debug.Log($"input {index} started");
        }

        protected virtual void Performed(int index)
        {
            //Debug.Log($"input {index} performed");
        }

        protected virtual void Canceled(int index)
        {
            //Debug.Log($"input {index} canceled");           
        }
        
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}