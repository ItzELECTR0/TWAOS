using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    #if UNITY_EDITOR
    [Image(typeof(IconCircleSolid), ColorTheme.Type.Yellow)]
    #endif

    [Serializable]
    public abstract class Instruction : TPolymorphicItem<Instruction>
    {
        private const int DEFAULT_NEXT_INSTRUCTION = 1;

        protected static readonly TimeMode DefaultTime = new TimeMode(TimeMode.UpdateMode.GameTime);
        protected static readonly Task DefaultResult = Task.FromResult(true);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected int NextInstruction { get; set; }
        protected InstructionList Parent { get; private set; }

        protected bool IsCanceled => (this.Parent?.IsCancelled ?? false) || AsyncManager.ExitRequest;

        // RUNNERS: -------------------------------------------------------------------------------

        public async Task<InstructionResult> Schedule(Args args, InstructionList parent)
        {
            this.NextInstruction = DEFAULT_NEXT_INSTRUCTION;
            this.Parent = parent;
            
            if (this.Breakpoint) Debug.Break();
            if (this.IsEnabled) await this.Run(args);
            
            if (this.IsCanceled) return InstructionResult.Stop;
            
            if (this.NextInstruction == DEFAULT_NEXT_INSTRUCTION) return InstructionResult.Default;
            if (this.NextInstruction == int.MaxValue) return InstructionResult.Stop;
            
            return InstructionResult.JumpTo(this.NextInstruction);
        }

        protected abstract Task Run(Args args);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private async Task Yield()
        {
            if (this.IsCanceled) return;
            await Task.Yield();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        /// <summary>
        /// Suspends the execution until the next frame.
        /// </summary>
        protected async Task NextFrame() => await Yield();

        /// <summary>
        /// Suspends the execution for the given amount of game seconds.
        /// </summary>
        protected async Task Time(float duration) => await Time(duration, DefaultTime);

        /// <summary>
        /// Suspends the execution for the given amount of seconds using the provided time mode.
        /// </summary>
        protected async Task Time(float duration, TimeMode time)
        {
            float startTime = time.Time;
            while (!this.IsCanceled && time.Time < startTime + duration)
            {
                await Yield();
            }
        }

        /// <summary>
        /// Suspends the execution until the supplied delegate evaluates to false.
        /// </summary>
        protected async Task While(Func<bool> function)
        {
            while (!this.IsCanceled && function.Invoke()) await Yield();
        }

        /// <summary>
        /// Suspends the execution until the supplied delegate evaluates to true.
        /// </summary>
        protected async Task Until(Func<bool> function)
        {
            while (!this.IsCanceled && !function.Invoke()) await Yield();
        }
    }
}