using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Follow Pointer")]
    [Image(typeof(IconCursor), ColorTheme.Type.Green)]

    [Category("Follow Pointer")]
    [Description("Moves the Player straight towards the pointer, relative to itself")]
    
    [Serializable]
    public class UnitPlayerFollowPointer : TUnitPlayer
    {
        private const int BUFFER_SIZE = 32;
        
        // RAYCAST COMPARER: ----------------------------------------------------------------------
        
        private static readonly RaycastComparer RAYCAST_COMPARER = new RaycastComparer();
        
        private class RaycastComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b)
            {
                return a.distance.CompareTo(b.distance);
            }
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] 
        private InputPropertyButton m_InputMove;

        [SerializeField]
        private PropertyGetInstantiate m_Indicator;

        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private RaycastHit[] m_HitBuffer;
        
        [NonSerialized] private Vector3 m_Direction;

        [NonSerialized] private bool m_PointerPress;
        [NonSerialized] private Vector3 m_Pointer;

        // INITIALIZERS: --------------------------------------------------------------------------

        public UnitPlayerFollowPointer()
        {
            this.m_Indicator = new PropertyGetInstantiate
            {
                usePooling = true,
                size = 5,
                hasDuration = true,
                duration = 1f
            };

            this.m_InputMove = InputButtonMouseWhilePressing.Create();
        }
        
        public override void OnStartup(Character character)
        {
            base.OnStartup(character);
            this.m_InputMove.OnStartup();
        }
        
        public override void OnDispose(Character character)
        {
            base.OnDispose(character);
            this.m_InputMove.OnDispose();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            this.m_HitBuffer = new RaycastHit[BUFFER_SIZE];
            
            this.m_InputMove.RegisterStart(this.OnStartPointer);
            this.m_InputMove.RegisterPerform(this.OnPerformPointer);
            
            this.m_Direction = Vector3.zero;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_HitBuffer = Array.Empty<RaycastHit>();
            
            this.m_InputMove.ForgetStart(this.OnStartPointer);
            this.m_InputMove.ForgetPerform(this.OnPerformPointer);
            
            this.Character.Motion?.MoveToDirection(Vector3.zero, Space.World, 0);
            this.m_Direction = Vector3.zero;
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public override void OnUpdate()
        {
            base.OnUpdate();
            this.m_InputMove.OnUpdate();
            
            if (!this.Character.IsPlayer) return;
            
            float speed = this.Character.Motion?.LinearSpeed ?? 0f;
            
            this.Character.Motion?.MoveToDirection(this.m_Direction * speed, Space.World, 0);

            if (this.m_PointerPress)
            {
                this.m_Indicator.Get(
                    this.Character.gameObject,
                    this.m_Pointer, Quaternion.identity
                );
            }
            
            this.m_Direction = Vector3.zero;
            this.m_PointerPress = false;
            this.m_Pointer = Vector3.zero;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnStartPointer()
        {
            if (!this.Character.IsPlayer) return;
            if (!this.Character.Player.IsControllable) return;

            this.m_PointerPress = true;
        }
        
        private void OnPerformPointer()
        {
            if (!this.Character.IsPlayer) return;
            
            this.m_Pointer = this.GetFollowPoint();
            this.m_Direction = (this.m_Pointer - this.Character.Feet).normalized;
        }

        private Vector3 GetFollowPoint()
        {
            if (!this.m_IsControllable) return this.Character.Feet;
            
            Camera camera = ShortcutMainCamera.Get<Camera>();
            Ray ray = camera.ScreenPointToRay(Application.isMobilePlatform
                ? Touchscreen.current.primaryTouch.position.ReadValue()
                : Mouse.current.position.ReadValue()
            );

            int hitCount = Physics.RaycastNonAlloc(
                ray, this.m_HitBuffer,
                Mathf.Infinity, -1,
                QueryTriggerInteraction.Ignore
            );
            
            Array.Sort(this.m_HitBuffer, 0, hitCount, RAYCAST_COMPARER);
            
            if (hitCount == 0) return this.Character.Feet;
            
            int colliderLayer = this.m_HitBuffer[0].transform.gameObject.layer;
            if ((colliderLayer & LAYER_UI) > 0) return this.Character.Feet;
            
            Plane plane = new Plane(Vector3.up, this.Character.Feet);
            if (!plane.Raycast(ray, out float rayDistance)) return this.Character.Feet;
            
            Vector3 pointer = ray.GetPoint(rayDistance);

            float curDistance = Vector3.Distance(this.Character.Feet, pointer); 
            float minDistance = this.Character.Motion.Radius;
            
            return curDistance >= minDistance ? pointer : this.Character.Feet;
        }

        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Follow Pointer";
    }
}