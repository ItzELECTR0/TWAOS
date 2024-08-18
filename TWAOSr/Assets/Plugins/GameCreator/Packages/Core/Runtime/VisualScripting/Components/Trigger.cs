using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.VisualScripting
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/visual-scripting/triggers")]
    [AddComponentMenu("Game Creator/Visual Scripting/Trigger")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT_LATER)]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoTrigger.png")]
    public class Trigger : 
        BaseActions, 
        IPointerEnterHandler, 
        IPointerExitHandler,
        ISelectHandler,
        IDeselectHandler,
        ISignalReceiver
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeReference]
        protected Event m_TriggerEvent = new EventOnStart();
        
        [NonSerialized] private Args m_Args;

        [NonSerialized] private Rigidbody m_Rigidbody3D;
        [NonSerialized] private Rigidbody2D m_Rigidbody2D;

        [NonSerialized] private Collider m_Collider3D;
        [NonSerialized] private Collider2D m_Collider2D;
        
        [NonSerialized] private IInteractive m_Interactive;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsExecuting { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventBeforeExecute;
        public event Action EventAfterExecute;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public static void Reconfigure(Trigger trigger, Event triggerEvent, InstructionList instructions)
        {
            trigger.m_TriggerEvent = triggerEvent;
            trigger.m_Instructions = instructions;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void Invoke(GameObject self = null)
        {
            Args args = new Args(self != null ? self : this.gameObject, this.gameObject);
            _ = this.Execute(args);
        }
        
        public async Task Execute(Args args)
        {
            if (this.IsExecuting) return;
            this.IsExecuting = true;
            
            this.EventBeforeExecute?.Invoke();

            try
            {
                await this.ExecInstructions(args);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString(), this);
            }

            this.IsExecuting = false;
            this.EventAfterExecute?.Invoke();
        }
        
        public async Task Execute(GameObject target)
        {
            if (this.IsExecuting) return;
            
            this.m_Args.ChangeTarget(target);
            await this.Execute(this.m_Args);
        }
        
        public async Task Execute()
        {
            if (this.IsExecuting) return;
            
            this.m_Args.ChangeTarget(null);
            await this.Execute(this.m_Args);
        }

        public void Cancel()
        {
            this.StopExecInstructions();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // CALLBACKS: -----------------------------------------------------------------------------

        protected void Awake()
        {
            this.m_Args = new Args(this);
            this.m_TriggerEvent?.OnAwake(this);
        }

        protected void Start()
        {
            this.m_TriggerEvent?.OnStart(this);
        }

        protected void OnEnable()
        {
            this.m_TriggerEvent?.OnEnable(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.Cancel();
            this.m_TriggerEvent?.OnDisable(this);
        }

        protected override void OnDestroy()
        {
            this.m_TriggerEvent?.OnDestroy(this);
            base.OnDestroy();
        }

        protected void OnBecameVisible()
        {
            this.m_TriggerEvent?.OnBecameVisible(this);
        }

        protected void OnBecameInvisible()
        {
            this.m_TriggerEvent?.OnBecameInvisible(this);
        }

        protected void Update()
        {
            this.m_TriggerEvent?.OnUpdate(this);
        }

        protected void LateUpdate()
        {
            this.m_TriggerEvent?.OnLateUpdate(this);
        }
        
        protected void FixedUpdate()
        {
            this.m_TriggerEvent?.OnFixedUpdate(this);
        }

        protected void OnApplicationFocus(bool f)
        {
            this.m_TriggerEvent?.OnApplicationFocus(this, f);
        }

        protected void OnApplicationPause(bool s)
        {
            this.m_TriggerEvent?.OnApplicationPause(this, s);
        }

        protected void OnApplicationQuit()
        {
            this.m_TriggerEvent?.OnApplicationQuit(this);
        }

        // PHYSICS 3D: ----------------------------------------------------------------------------

        protected void OnCollisionEnter(Collision c)
        {
            this.m_TriggerEvent?.OnCollisionEnter3D(this, c);
        }

        protected void OnCollisionExit(Collision c)
        {
            this.m_TriggerEvent?.OnCollisionExit3D(this, c);
        }

        protected void OnCollisionStay(Collision c)
        {
            this.m_TriggerEvent?.OnCollisionStay3D(this, c);
        }

        protected void OnTriggerEnter(Collider c)
        {
            this.m_TriggerEvent?.OnTriggerEnter3D(this, c);
        }

        protected void OnTriggerExit(Collider c)
        {
            this.m_TriggerEvent?.OnTriggerExit3D(this, c);
        }

        protected void OnTriggerStay(Collider c)
        {
            this.m_TriggerEvent?.OnTriggerStay3D(this, c);
        }

        protected void OnJointBreak(float force)
        {
            this.m_TriggerEvent?.OnJointBreak3D(this, force);
        }

        // PHYSICS 2D: ----------------------------------------------------------------------------

        protected void OnCollisionEnter2D(Collision2D c)
        {
            this.m_TriggerEvent?.OnCollisionEnter2D(this, c);
        }

        protected void OnCollisionExit2D(Collision2D c)
        {
            this.m_TriggerEvent?.OnCollisionExit2D(this, c);
        }

        protected void OnCollisionStay2D(Collision2D c)
        {
            this.m_TriggerEvent?.OnCollisionStay2D(this, c);
        }

        protected void OnTriggerEnter2D(Collider2D c)
        {
            this.m_TriggerEvent?.OnTriggerEnter2D(this, c);
        }

        protected void OnTriggerExit2D(Collider2D c)
        {
            this.m_TriggerEvent?.OnTriggerExit2D(this, c);
        }

        protected void OnTriggerStay2D(Collider2D c)
        {
            this.m_TriggerEvent?.OnTriggerStay2D(this, c);
        }

        protected void OnJointBreak2D(Joint2D joint)
        {
            this.m_TriggerEvent?.OnJointBreak2D(this, joint);
        }

        // INPUT: ---------------------------------------------------------------------------------

        protected void OnMouseDown()
        {
            this.m_TriggerEvent?.OnMouseDown(this);
        }

        protected void OnMouseUp()
        {
            this.m_TriggerEvent?.OnMouseUp(this);
        }

        protected void OnMouseUpAsButton()
        {
            this.m_TriggerEvent?.OnMouseUpAsButton(this);
        }

        protected void OnMouseEnter()
        {
            this.m_TriggerEvent?.OnMouseEnter(this);
        }

        protected void OnMouseOver()
        {
            this.m_TriggerEvent?.OnMouseOver(this);
        }

        protected void OnMouseExit()
        {
            this.m_TriggerEvent?.OnMouseExit(this);
        }

        protected void OnMouseDrag()
        {
            this.m_TriggerEvent?.OnMouseDrag(this);
        }
        
        // UI: ------------------------------------------------------------------------------------
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            this.m_TriggerEvent?.OnPointerEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.m_TriggerEvent?.OnPointerExit(this);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            this.m_TriggerEvent?.OnSelect(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.m_TriggerEvent?.OnDeselect(this);
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        protected virtual void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            this.m_TriggerEvent?.OnDrawGizmos(this);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            this.m_TriggerEvent?.OnDrawGizmosSelected(this);
        }
        
        // SIGNALS: -------------------------------------------------------------------------------

        void ISignalReceiver.OnReceiveSignal(SignalArgs args)
        {
            
            this.m_TriggerEvent?.OnReceiveSignal(this, args);
        }
        
        // CUSTOM CALLBACKS: ----------------------------------------------------------------------

        /// <summary>
        /// Attempts to invoke a command that can be interpreted by the Event. If the event
        /// is listening to this event, it will be executed.
        /// </summary>
        /// <param name="args">The command name and optional target to execute</param>
        public void OnReceiveCommand(CommandArgs args)
        {
            this.m_TriggerEvent?.OnReceiveCommand(this, args);
        }
        
        /// <summary>
        /// Attempts to invoke a command that can be interpreted by the Event. If the event
        /// is listening to this event, it will be executed.
        /// </summary>
        /// <param name="command">The name of the command to execute</param>
        [Obsolete("Soon to deprecate. Use OnReceiveCommand(CommandArgs) instead")]
        public void OnReceiveCommand(PropertyName command)
        {
            this.m_TriggerEvent?.OnReceiveCommand(this, new CommandArgs(command));
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // PHYSICS METHODS: -----------------------------------------------------------------------

        public void RequireRigidbody()
        {
            if (this.m_Collider3D == null) this.m_Collider3D = this.Get<Collider>();
            if (this.m_Collider2D == null) this.m_Collider2D = this.Get<Collider2D>();
            
            if (this.m_Collider3D != null) this.RequireRigidbody3D();
            if (this.m_Collider2D != null) this.RequireRigidbody2D();
        }
        
        private void RequireRigidbody3D()
        {
            if (this.m_Rigidbody3D != null) return;
            
            this.m_Rigidbody3D = this.Get<Rigidbody>();
            if (this.m_Rigidbody3D != null) return;
            
            if (this.m_Collider3D == null)
            {
                this.m_Collider3D = this.Get<Collider>();
                if (this.m_Collider3D == null) return;
            }

            this.m_Rigidbody3D = this.Add<Rigidbody>();
            this.m_Rigidbody3D.isKinematic = true;
            this.m_Rigidbody3D.hideFlags = HideFlags.HideInInspector;
        }
        
        private void RequireRigidbody2D()
        {
            if (this.m_Rigidbody2D != null) return;
            
            this.m_Rigidbody2D = this.Get<Rigidbody2D>();
            if (this.m_Rigidbody2D != null) return;
            
            if (this.m_Collider2D == null)
            {
                this.m_Collider2D = this.Get<Collider2D>();
                if (this.m_Collider2D == null) return;
            }

            this.m_Rigidbody2D = this.Add<Rigidbody2D>();
            this.m_Rigidbody2D.isKinematic = true;
            this.m_Rigidbody2D.hideFlags = HideFlags.HideInInspector;
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // INTERACTION: ---------------------------------------------------------------------------

        internal void RequireInteractionTracker()
        {
            InteractionTracker tracker = InteractionTracker.Require(this.gameObject);
            
            this.m_Interactive = tracker;
            
            tracker.EventInteract -= this.OnStartInteraction;
            tracker.EventInteract += this.OnStartInteraction;
        }

        private void OnStartInteraction(Character character, IInteractive interactive)
        {
            this.EventAfterExecute -= this.OnStopInteraction;
            this.EventAfterExecute += this.OnStopInteraction;
            
            this.m_TriggerEvent?.OnInteract(this, character);
        }

        private void OnStopInteraction()
        {
            this.EventAfterExecute -= this.OnStopInteraction;
            this.m_Interactive?.Stop();
        }
    }
}