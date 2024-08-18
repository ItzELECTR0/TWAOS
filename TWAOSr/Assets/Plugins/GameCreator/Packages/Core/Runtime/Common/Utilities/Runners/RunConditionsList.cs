using System;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class RunConditionsList : TRun<ConditionList>
    {
        private const int PREWARM_COUNTER = 1;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private ConditionList m_Conditions;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override ConditionList Value => this.m_Conditions;

        protected override GameObject Template
        {
            get
            {
                if (this.m_Template == null) this.m_Template = CreateTemplate(this.Value);
                return this.m_Template;
            }
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RunConditionsList()
        {
            this.m_Conditions = new ConditionList();
        }
        
        public RunConditionsList(params Condition[] conditions)
        {
            this.m_Conditions = new ConditionList(conditions);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Check(Args args)
        {
            return this.Check(args, RunnerConfig.Default);
        }

        public bool Check(Args args, RunnerConfig config)
        {
            if ((this.m_Conditions?.Length ?? 0) == 0) return true;
            
            GameObject template = this.Template;
            return Check(args, template, config);
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static bool Check(Args args, GameObject template)
        {
            return Check(args, template, RunnerConfig.Default);
        }
        
        public static bool Check(Args args, GameObject template, RunnerConfig config)
        {
            if ((template.Get<RunnerConditionsList>().Value?.Length ?? 0) == 0) return true;
            
            RunnerConditionsList runner = RunnerConditionsList.Pick<RunnerConditionsList>(
                template,
                config,
                PREWARM_COUNTER
            );
            
            if (runner == null) return false;
            
            bool result = runner.Value.Check(args, CheckMode.And);
            if (runner != null) RunnerConditionsList.Restore(runner);
            
            return result;
        }
        
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static GameObject CreateTemplate(ConditionList value)
        {
            return RunnerConditionsList.CreateTemplate<RunnerConditionsList>(value);
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Conditions.Length switch
            {
                0 => string.Empty,
                1 => this.m_Conditions.Get(0)?.Title,
                _ => $"{this.m_Conditions.Get(0)?.Title} +{this.m_Conditions.Length - 1}"
            };
        }
    }
}