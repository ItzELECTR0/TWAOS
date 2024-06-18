using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconBranch), ColorTheme.Type.Green)]
    
    [Serializable]
    public class Branch : TPolymorphicItem<Branch>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private string m_Description = "";
        
        [SerializeField] 
        private ConditionList m_ConditionList = new ConditionList();
        
        [SerializeField]
        private InstructionList m_InstructionList = new InstructionList();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.IsNullOrEmpty(this.m_Description)
            ? "Branch"
            : this.m_Description;

        // RUNNERS: -------------------------------------------------------------------------------

        public async Task<BranchResult> Evaluate(Args args, ICancellable cancellable)
        {
            if (!this.IsEnabled) return BranchResult.False;
            if (this.Breakpoint) Debug.Break();
            
            if (!this.m_ConditionList.Check(args, CheckMode.And)) return BranchResult.False;
            
            await this.m_InstructionList.Run(args, cancellable);
            return BranchResult.True;
        }
    }
}