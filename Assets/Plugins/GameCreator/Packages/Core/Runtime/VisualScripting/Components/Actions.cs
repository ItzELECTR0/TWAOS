using System;
using System.Threading.Tasks;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/visual-scripting/actions")]
    [AddComponentMenu("Game Creator/Visual Scripting/Actions")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT_LATER)]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoActions.png")]
    public class Actions : BaseActions
    {
        public override void Invoke(GameObject self = null)
        {
            Args args = new Args(self != null ? self : this.gameObject, this.gameObject);
            _ = this.Run(args);
        }

        // ASYNC METHODS: -------------------------------------------------------------------------

        public async Task Run()
        {
            try
            {
                await this.ExecInstructions();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString(), this);
            }
        }

        public async Task Run(Args args)
        {
            try
            {
                await this.ExecInstructions(args);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString(), this);
            }
        }
        
        // CANCEL METHOD: -------------------------------------------------------------------------

        public void Cancel()
        {
            this.StopExecInstructions();
        }
    }
}