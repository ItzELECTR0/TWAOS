using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("No Ragdoll")]
    
    [Serializable]
    public class RagdollNone : TRagdollSystem
    {
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        protected internal override void OnStartup(Character character)
        { }

        protected internal override void OnDispose(Character character)
        { }

        protected internal override void OnEnable(Character character)
        { }

        protected internal override void OnDisable(Character character)
        { }
        
        // UPDATE METHODS: ------------------------------------------------------------------------
        
        protected internal override void OnUpdate(Character character)
        { }

        protected internal override void OnLateUpdate(Character character)
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        protected internal override Task StartRagdoll(Character character)
        {
            return Task.CompletedTask;
        }

        protected internal override Task StopRagdoll(Character character)
        {
            return Task.CompletedTask;
        }

        protected internal override Task RecoverRagdoll(Character character)
        {
            return Task.CompletedTask;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        protected internal override void OnDrawGizmos(Character character)
        { }
    }
}