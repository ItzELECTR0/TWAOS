using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class CameraAvoidClip
    {
        private static readonly Color GIZMOS_COLOR = new Color(0f, 1f, 0f, 0.5f);
        
        private const int GIZMOS_DIVISIONS = 5;
        private const int RAYCAST_BUFFER_SIZE = 50;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected bool m_Enabled;
        [SerializeField] protected LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        [SerializeField] protected float m_Radius = 0.4f;
        [SerializeField] protected float m_MinDistance = 0f;

        [SerializeField] protected float m_SmoothTime = 0.5f;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly RaycastHit[] m_HitBuffer;

        [NonSerialized] private float m_CurrentDistance;
        [NonSerialized] private float m_Velocity;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Enabled
        {
            get => this.m_Enabled;
            set => this.m_Enabled = value;
        }
        
        public LayerMask LayerMask
        {
            get => this.m_LayerMask;
            set => this.m_LayerMask = value;
        }
        
        public float Radius
        {
            get => this.m_Radius;
            set => this.m_Radius = value;
        }
        
        public float MinDistance
        {
            get => this.m_MinDistance;
            set => this.m_MinDistance = value;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public CameraAvoidClip()
        {
            this.m_HitBuffer = new RaycastHit[RAYCAST_BUFFER_SIZE];
            this.m_CurrentDistance = this.m_MinDistance + this.m_Radius;
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public Vector3 Update(TCamera camera, Transform target, Transform[] ignore)
        {
            if (!this.m_Enabled || target == null || 
                !camera.Transition.CurrentShotCamera.AvoidClipping || 
                camera.transform.position == target.position)
            {
                return camera.transform.position;
            }

            Vector3 origin = Vector3.MoveTowards(
                target.position,
                camera.transform.position,
                this.m_MinDistance + this.m_Radius
            );

            Vector3 direction = camera.transform.position - origin;
            float magnitude = Mathf.Max(direction.magnitude, this.m_MinDistance);

            int count = this.Raycast(
                origin, direction.normalized * magnitude,
                this.m_Radius, this.m_LayerMask
            );

            float targetDistance = magnitude;

            for (int i = 0; i < count; ++i)
            {
                RaycastHit hit = this.m_HitBuffer[i];
                if (this.IsChild(hit.transform, ignore)) continue;
                
                Character character = target.Get<Character>();
                if (character != null)
                {
                    Transform model = character.Animim.Animator.transform;
                    if (hit.transform.IsChildOf(model)) continue;
                }

                if (hit.distance < targetDistance)
                {
                    targetDistance = hit.distance;
                }
            }
            
            if (this.m_CurrentDistance > targetDistance)
            {
                this.m_CurrentDistance = targetDistance;
                this.m_Velocity = 0f;
            }
            else
            {
                this.m_CurrentDistance = Mathf.SmoothDamp(
                    this.m_CurrentDistance, 
                    targetDistance, 
                    ref this.m_Velocity,
                    this.m_SmoothTime,
                    Mathf.Infinity,
                    camera.Time.DeltaTime
                );
            }

            Vector3 position = Vector3.MoveTowards(
                origin, 
                camera.transform.position,
                this.m_CurrentDistance
            );
            
            return position;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected int Raycast(Vector3 origin, Vector3 direction, float radius, LayerMask layerMask)
        {
            if (radius <= float.Epsilon)
            {
                return Physics.RaycastNonAlloc(
                    origin,
                    direction.normalized,
                    this.m_HitBuffer,
                    direction.magnitude,
                    layerMask,
                    QueryTriggerInteraction.Ignore
                );
            }

            return Physics.SphereCastNonAlloc(
                origin,
                radius,
                direction.normalized,
                this.m_HitBuffer,
                direction.magnitude,
                layerMask,
                QueryTriggerInteraction.Ignore
            );
        }

        protected bool IsChild(Transform hit, Transform[] ignoreList)
        {
            foreach (Transform ignore in ignoreList)
            {
                if (ignore == null) continue;
                if (hit.IsChildOf(ignore)) return true;
            }

            return false;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public void OnDrawGizmos(TCamera camera)
        {
            
        }
        
        public void OnDrawGizmosSelected(TCamera camera)
        {
            if (this.m_Enabled)
            {
                Gizmos.color = GIZMOS_COLOR;
                
                GizmosExtension.Octahedron(
                    camera.transform.position,
                    camera.transform.rotation, 
                    this.m_Radius,
                    GIZMOS_DIVISIONS
                );
            }
        }
    }
}