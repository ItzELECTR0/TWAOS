using System;
using System.Threading.Tasks;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    public abstract class BaseActions : MonoBehaviour
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        protected InstructionList m_Instructions = new InstructionList();

        private Args m_Args;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsRunning => this.m_Instructions.IsRunning;
        public int RunningIndex => this.m_Instructions.RunningIndex;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action EventInstructionStartRunning;
        public event Action EventInstructionEndRunning;
        public event Action<int> EventInstructionRun;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected BaseActions()
        {
            this.m_Instructions.EventRunInstruction += this.OnEventInstructionRun;
            this.m_Instructions.EventStartRunning += this.OnEventInstructionStartRunning;
            this.m_Instructions.EventEndRunning += this.OnEventInstructionStopRunning;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        public abstract void Invoke(GameObject self = null);
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected async Task ExecInstructions()
        {
            this.m_Args ??= new Args(this.gameObject);
            await this.ExecInstructions(this.m_Args);
        }

        protected async Task ExecInstructions(Args args)
        {
            await this.m_Instructions.Run(args);
        }

        protected void StopExecInstructions()
        {
            this.m_Instructions.Cancel();
        }

        // BEHAVIOR METHODS: ----------------------------------------------------------------------

        protected virtual void OnDisable()
        {
            this.StopExecInstructions();
        }

        protected virtual void OnDestroy()
        {
            this.EventInstructionStartRunning = null;
            this.EventInstructionEndRunning = null;
            this.EventInstructionRun = null;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnEventInstructionRun(int i)
        {
            this.EventInstructionRun?.Invoke(i);
        }
        
        private void OnEventInstructionStartRunning()
        {
            this.EventInstructionStartRunning?.Invoke();
        }
        
        private void OnEventInstructionStopRunning()
        {
            this.EventInstructionEndRunning?.Invoke();
        }
    }
}