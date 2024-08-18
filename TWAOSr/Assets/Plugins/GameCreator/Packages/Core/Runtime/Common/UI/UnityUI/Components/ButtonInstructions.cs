using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Button")]
    [RequireComponent(typeof(Image))]
    [Icon(RuntimePaths.GIZMOS + "GizmoUIButton.png")]
    public class ButtonInstructions : Button
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private InstructionList m_Instructions = new InstructionList(
            new InstructionCommonDebugText("Click!")
        );
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Args m_Args;
        
        // INIT METHODS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Start();
            if (!Application.isPlaying) return;

            this.m_Args = new Args(gameObject);
            
            this.m_Instructions.EventStartRunning += this.OnStartRunning;
            this.m_Instructions.EventEndRunning += this.OnEndRunning;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!Application.isPlaying) return;
            
            this.m_Instructions.EventStartRunning -= this.OnStartRunning;
            this.m_Instructions.EventEndRunning -= this.OnEndRunning;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying) return;
            
            this.onClick.AddListener(this.RunInstructions);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!Application.isPlaying) return;
            
            this.onClick.RemoveListener(this.RunInstructions);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RunInstructions()
        {
            _ = this.m_Instructions.Run(this.m_Args);
        }
        
        private void OnStartRunning()
        {
            this.interactable = false;
        }
        
        private void OnEndRunning()
        {
            this.interactable = true;
        }
    }
}
