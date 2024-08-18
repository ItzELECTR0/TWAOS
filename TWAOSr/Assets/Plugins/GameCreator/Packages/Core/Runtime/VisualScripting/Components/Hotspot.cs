using System;
using GameCreator.Runtime.Characters;
using UnityEngine;
using GameCreator.Runtime.Common;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.VisualScripting
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/visual-scripting/hotspots")]
    [AddComponentMenu("Game Creator/Visual Scripting/Hotspot")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT_LATER)]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoHotspot.png")]
    public class Hotspot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly Color GIZMOS_COLOR = Color.red;

        private const float TRANSITION_SMOOTH_TIME = 0.25f;
        
        private const float GIZMOS_ALPHA_ON = 0.25f;
        private const float GIZMOS_ALPHA_OFF = 0.1f;

        public static bool ActiveInRadius { get; set; } = true;
        public static bool ActiveInteractive { get; set; } = true;
        public static bool ActiveAlways { get; set; } = true;
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            ActiveInRadius = true;
            ActiveInteractive = true;
            ActiveAlways = true;
        }

        #endif
        
        // ENUMS: ---------------------------------------------------------------------------------
        
        public enum HotspotMode
        {
            InRadius,
            OnInteractionFocus,
            OnInteractionReach,
            AlwaysActive
        }

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        [SerializeField] private HotspotMode m_Mode = HotspotMode.InRadius;
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(10f);
        
        [SerializeField]
        protected SpotList m_Spots = new SpotList();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private float m_Velocity;

        // PROPERTIES: ----------------------------------------------------------------------------

        public GameObject Target => this.m_Target.Get(this.Args);

        [field: NonSerialized] public Args Args { get; private set; }

        [field: NonSerialized] public bool IsActive { get; private set; }
        [field: NonSerialized] public float Animation { get; private set; }
        
        [field: NonSerialized] public float Distance { get; private set; }
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventOnActivate;
        public event Action EventOnDeactivate;

        // MAIN METHODS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.Args = new Args(this);
            this.m_Spots.OnAwake(this);
        }

        private void Start()
        {
            this.m_Spots.OnStart(this);
        }

        private void Update()
        {
            if (this.m_Mode == HotspotMode.OnInteractionFocus || 
                this.m_Mode == HotspotMode.OnInteractionReach)
            {
                InteractionTracker.Require(this.gameObject);
            }
            
            bool wasActive = this.IsActive;

            switch (this.m_Mode)
            {
                case HotspotMode.InRadius: this.UpdateInRadius(); break;
                case HotspotMode.OnInteractionFocus: this.UpdateInFocus(); break;
                case HotspotMode.OnInteractionReach: this.UpdateInRange(); break;
                case HotspotMode.AlwaysActive: this.UpdateAlwaysActive(); break;
                default: throw new ArgumentOutOfRangeException();
            }

            this.Animation = Mathf.SmoothDamp(
                this.Animation,
                this.IsActive ? 1f : 0f,
                ref this.m_Velocity,
                TRANSITION_SMOOTH_TIME
            );

            this.m_Spots.OnUpdate(this);
            
            switch (wasActive)
            {
                case false when this.IsActive: this.EventOnActivate?.Invoke(); break;
                case true when !this.IsActive: this.EventOnDeactivate?.Invoke(); break;
            }
        }

        private void OnEnable()
        {
            this.m_Velocity = 0f;
            this.Animation = 0f;
            this.m_Spots.OnEnable(this);
        }

        private void OnDisable()
        {
            this.m_Velocity = 0f;
            this.Animation = 0f;
            this.m_Spots.OnDisable(this);
        }
        
        // POINTER METHODS: -----------------------------------------------------------------------

        void IPointerEnterHandler.OnPointerEnter(PointerEventData pointerEventData)
        {
            this.m_Spots.OnPointerEnter(this);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData pointerEventData)
        {
            this.m_Spots.OnPointerExit(this);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float GetRadius(Args args)
        {
            switch (this.m_Mode)
            {
                case HotspotMode.InRadius: return (float) this.m_Radius.Get(args);
                case HotspotMode.OnInteractionFocus:
                case HotspotMode.OnInteractionReach:
                {
                    Character character = this.m_Target.Get<Character>(args);
                    return character != null ? character.Motion.InteractionRadius : 0f;
                }
                case HotspotMode.AlwaysActive: return Mathf.Infinity;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void UpdateInRadius()
        {
            if (ActiveInRadius == false)
            {
                this.IsActive = false;
                return;
            }
            
            GameObject target = this.Target;
            
            if (target == null)
            {
                this.IsActive = false;
                this.Distance = float.MaxValue;
            }
            else
            {
                this.Distance = Vector3.Distance(
                    target.transform.position,
                    this.transform.position
                );
                
                double radius = this.m_Radius.Get(this.Args);
                this.IsActive = this.Distance <= radius;
            }
        }
        
        private void UpdateInFocus()
        {
            if (ActiveInteractive == false)
            {
                this.IsActive = false;
                return;
            }
            
            GameObject target = this.Target;
            Character character = target.Get<Character>();
            
            if (target == null || character == null)
            {
                this.IsActive = false;
                this.Distance = float.MaxValue;
            }
            else
            {
                this.Distance = Vector3.Distance(
                    target.transform.position,
                    this.transform.position
                );
                
                this.IsActive = character.Interaction.Target?.Instance == this.gameObject;
            }
        }

        private void UpdateInRange()
        {
            if (ActiveInteractive == false)
            {
                this.IsActive = false;
                return;
            }
            
            GameObject target = this.Target;
            Character character = target.Get<Character>();
            
            if (target == null || character == null)
            {
                this.IsActive = false;
                this.Distance = float.MaxValue;
            }
            else
            {
                this.Distance = Vector3.Distance(
                    target.transform.position,
                    this.transform.position
                );

                if (this.gameObject == character.Interaction.Target?.Instance)
                {
                    this.IsActive = false;
                }
                else
                {
                    bool isCandidate = false;
                    foreach (ISpatialHash interaction in character.Interaction.Interactions)
                    {
                        if (interaction is not IInteractive interactive) continue;
                        if (interactive.Instance == this.gameObject)
                        {
                            float distance = Vector3.Distance(
                                interactive.Position,
                                character.transform.position
                            );

                            isCandidate = distance <= character.Motion.InteractionRadius;
                            break;
                        }
                    }
                    
                    this.IsActive = isCandidate;
                }
            }
        }
        
        private void UpdateAlwaysActive()
        {
            this.IsActive = ActiveAlways;
            
            GameObject target = this.Target;
            this.Distance = target != null
                ? Vector3.Distance(target.transform.position, this.transform.position)
                : int.MaxValue;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            float alpha = Mathf.Lerp(
                GIZMOS_ALPHA_OFF,
                GIZMOS_ALPHA_ON,
                this.IsActive ? 1f : 0f
            );

            Gizmos.color = new Color(
                GIZMOS_COLOR.r,
                GIZMOS_COLOR.g,
                GIZMOS_COLOR.b,
                alpha
            );

            if (this.m_Mode == HotspotMode.InRadius)
            {
                GizmosExtension.Octahedron(
                    this.transform.position,
                    this.transform.rotation,
                    (float) this.m_Radius.EditorValue
                );
            }

            this.m_Spots.OnGizmos(this);
            
            if (!Application.isPlaying) return;
            
            if (this.Target != null)
            {
                Gizmos.DrawLine(
                    this.Target.transform.position,
                    this.transform.position
                );
            }
        }
    }
}