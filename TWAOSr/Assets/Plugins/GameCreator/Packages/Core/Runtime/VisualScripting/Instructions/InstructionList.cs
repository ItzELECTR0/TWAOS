using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class InstructionList : TPolymorphicList<Instruction>, ICancellable
    {
        [SerializeReference]
        private Instruction[] m_Instructions = Array.Empty<Instruction>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsRunning { get; private set; }
        public bool IsStopped { get; private set; }

        public ICancellable Cancellable { get; private set; }
        public bool IsCancelled => this.IsStopped || (Cancellable?.IsCancelled ?? false);

        public int RunningIndex { get; private set; }

        public override int Length => this.m_Instructions.Length;
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventStartRunning;
        public event Action EventEndRunning;

        public event Action<int> EventRunInstruction;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstructionList()
        {
            this.IsRunning = false;
            this.IsStopped = false;
        }

        public InstructionList(params Instruction[] instructions) : this()
        {
            this.m_Instructions = instructions;
        }
        
        public InstructionList(InstructionList instructionList) : this()
        {
            this.m_Instructions = instructionList.m_Instructions;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public async Task Run(Args args, int fromIndex = 0)
        {
            await this.Run(args, null, fromIndex);
        }
        
        public async Task Run(Args args, ICancellable cancellable, int fromIndex = 0)
        {
            if (this.IsRunning) return;

            this.Cancellable = cancellable;
            this.IsRunning = true;
            this.RunningIndex = Math.Max(0, fromIndex);

            this.IsStopped = false;
            this.EventStartRunning?.Invoke();

            while (this.RunningIndex < this.Length)
            {
                if (this.IsCancelled)
                {
                    this.IsStopped = true;
                    this.IsRunning = false;
                    
                    this.EventEndRunning?.Invoke();
                    return;
                }

                if (this.m_Instructions[this.RunningIndex] == null)
                {
                    this.RunningIndex += 1;
                    continue;
                }

                EventRunInstruction?.Invoke(this.RunningIndex);
                
                Instruction instruction = this.m_Instructions[this.RunningIndex];
                InstructionResult result = await instruction.Schedule(args, this);

                if (result.DontContinue)
                {
                    this.IsRunning = false;
                    this.EventEndRunning?.Invoke();
                    return;
                }
                
                this.RunningIndex += result.NextInstruction;
            }

            this.IsRunning = false;
            this.EventEndRunning?.Invoke();
        }

        public void Cancel()
        {
            this.IsStopped = true;
        }
        
        public Instruction Get(int index)
        {
            index = Mathf.Clamp(index, 0, this.Length - 1);
            return this.m_Instructions[index];
        }
    }
}