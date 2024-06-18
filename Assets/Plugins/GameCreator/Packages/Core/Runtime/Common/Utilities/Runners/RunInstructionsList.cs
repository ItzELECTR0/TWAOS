using System;
using System.Threading.Tasks;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class RunInstructionsList : TRun<InstructionList>
    {
        private const int PREWARM_COUNTER = 5;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InstructionList m_Instructions;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override InstructionList Value => this.m_Instructions;
        
        protected override GameObject Template
        {
            get
            {
                if (this.m_Template == null) this.m_Template = CreateTemplate(this.Value);
                return this.m_Template;
            }
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RunInstructionsList()
        {
            this.m_Instructions = new InstructionList();
        }
        
        public RunInstructionsList(params Instruction[] instructions)
        {
            this.m_Instructions = new InstructionList(instructions);
        }

        public RunInstructionsList(InstructionList instructionList)
        {
            this.m_Instructions = new InstructionList(instructionList);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public async Task Run(Args args)
        {
            await this.Run(args, RunnerConfig.Default);
        }
        
        public async Task Run(Args args, RunnerConfig config)
        {
            if (ApplicationManager.IsExiting) return;
            if ((this.m_Instructions?.Length ?? 0) == 0) return;
            
            GameObject template = this.Template;
            await Run(args, template, config);
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static async Task Run(Args args, GameObject template)
        {
            await Run(args, template, RunnerConfig.Default);
        }
        
        public static async Task Run(Args args, GameObject template, RunnerConfig config)
        {
            if (ApplicationManager.IsExiting) return;
            if ((template.Get<RunnerInstructionsList>().Value?.Length ?? 0) == 0) return;

            RunnerInstructionsList runner = RunnerInstructionsList.Pick<RunnerInstructionsList>(
                template,
                config,
                PREWARM_COUNTER
            );

            if (runner == null) return;
            
            await runner.Value.Run(args, config.Cancellable);
            if (runner != null) RunnerInstructionsList.Restore(runner);
        }
        
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static GameObject CreateTemplate(InstructionList value)
        {
            return RunnerInstructionsList.CreateTemplate<RunnerInstructionsList>(value);
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Instructions.Length switch
            {
                0 => string.Empty,
                1 => this.m_Instructions.Get(0).ToString(),
                _ => $"{this.m_Instructions.Get(0)} +{this.m_Instructions.Length - 1}"
            };
        }
    }
}