using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/visual-scripting/conditions")]
    [AddComponentMenu("Game Creator/Visual Scripting/Conditions")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT_LATER)]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoConditions.png")]
    public class Conditions : MonoBehaviour
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        protected BranchList m_Branches = new BranchList();
        
        private Args m_Args;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsRunning => this.m_Branches.IsRunning;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action EventStartRunning;
        public event Action EventEndRunning;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Invoke(GameObject self = null)
        {
            Args args = new Args(self != null ? self : this.gameObject, this.gameObject);
            _ = this.Run(args);
        }
        
        public async Task Run()
        {
            this.m_Args ??= new Args(this.gameObject);
            await this.Run(this.m_Args);
        }

        public async Task Run(Args args)
        {
            this.EventStartRunning?.Invoke();
            
            await this.m_Branches.Evaluate(args);
            
            this.EventEndRunning?.Invoke();
        }

        public void Cancel()
        {
            this.m_Branches.Cancel();
        }
    }
}
