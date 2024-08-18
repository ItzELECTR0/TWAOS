using System;

namespace GameCreator.Editor.Common
{
    public class InitRunner
    {
        private readonly Func<bool> m_CanRunFunction;
        private readonly Action m_RunFunction;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Order { get; }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public InitRunner(int order, Func<bool> canRun, Action run)
        {
            this.Order = order;
            
            this.m_CanRunFunction = canRun;
            this.m_RunFunction = run;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool CanRun() => this.m_CanRunFunction.Invoke();

        public void Run() => this.m_RunFunction.Invoke();
    }
}