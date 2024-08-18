using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class BranchList : TPolymorphicList<Branch>, ICancellable
    {
        [SerializeReference]
        private Branch[] m_Branches = Array.Empty<Branch>();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public bool IsRunning { get; private set; }
        public bool IsStopped { get; private set; }

        public int EvaluatingIndex { get; private set; }
        
        public ICancellable Cancellable { get; private set; }
        public bool IsCancelled => this.IsStopped || (Cancellable?.IsCancelled ?? false);

        public override int Length => this.m_Branches.Length;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventStartRunning;
        public event Action EventEndRunning;
        
        public event Action<int> EventRunBranch;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public BranchList()
        {
            this.IsRunning = false;
            this.IsStopped = false;
        }

        public BranchList(params Branch[] branches) : this()
        {
            this.m_Branches = branches;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public async Task Evaluate(Args args)
        {
            await this.Evaluate(args, null);
        }
        
        public async Task Evaluate(Args args, ICancellable cancellable)
        {
            if (this.IsRunning) return;

            this.Cancellable = cancellable;
            this.EvaluatingIndex = -1;
            
            this.IsRunning = true;
            this.IsStopped = false;
            
            this.EventStartRunning?.Invoke();
            
            for (var i = 0; i < this.Length; ++i)
            {
                Branch branch = this.m_Branches[i];
                if (branch == null) continue;

                this.EvaluatingIndex = i;
                EventRunBranch?.Invoke(this.EvaluatingIndex);
                
                BranchResult result = await branch.Evaluate(args, this);
                
                if (result.Value) break;
            }

            this.EvaluatingIndex = -1;
            this.IsRunning = false;
            
            this.EventEndRunning?.Invoke();
        }

        public void Cancel()
        {
            this.IsStopped = true;
        }
    }
}
