using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Footsteps : TPolymorphicList<Footstep>
    {
        private const float RAYCAST_DISTANCE_PERCENTAGE = 0.25f;
        private const int RAYCAST_BUFFER_SIZE = 5;
        
        private static readonly Color COLOR_GIZMO_GROUND_OFF = new Color(
            Color.yellow.r,
            Color.yellow.g, 
            Color.yellow.b,
            0.25f
        );
        
        private static readonly Color COLOR_GIZMO_GROUND_ON = new Color(
            Color.green.r,
            Color.green.g, 
            Color.green.b,
            0.5f
        );

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeReference] private Footstep[] m_Feet =
        {
            new Footstep(HumanBodyBones.LeftFoot),
            new Footstep(HumanBodyBones.RightFoot),
        };

        [SerializeField] private MaterialSounds m_FootstepSounds = new MaterialSounds();
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Character m_Character;
        
        private Dictionary<Transform, Footprint> m_Footprints = new Dictionary<Transform, Footprint>();
        
        private readonly RaycastHit[] m_HitsBuffer = new RaycastHit[RAYCAST_BUFFER_SIZE];

        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Feet.Length;

        public GameObject LastFootstep { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Transform> EventStep;

        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        internal void OnStartup(Character character)
        {
            this.m_Character = character;
            this.m_FootstepSounds.OnStartup();
        }
        
        internal void AfterStartup(Character character)
        { }

        internal void OnDispose(Character character)
        {
            this.m_Character = character;
        }

        internal void OnEnable()
        { }

        internal void OnDisable()
        { }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        internal void OnUpdate()
        {
            Animator animator = this.m_Character.Animim.Animator;
            if (animator == null) return;

            bool isGrounded = this.m_Character.Driver.IsGrounded;
            
            for (int i = 0; i < this.m_Feet.Length && i < Phases.Count; i++)
            {
                Footstep foot = this.m_Feet[i];
                Transform bone = foot.Bone.GetTransform(animator);
                if (bone == null) continue;

                bool phaseGround = this.m_Character.Phases.IsGround(i);

                if (isGrounded && this.m_Footprints.TryGetValue(bone, out Footprint footprint))
                {
                    if (phaseGround && !footprint.WasGrounded)
                    {
                        this.OnStep(i, bone);
                    }

                    footprint.WasGrounded = phaseGround;
                }
                else
                {
                    this.m_Footprints[bone] = new Footprint
                    {
                        WasGrounded = true,
                    };
                }
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ChangeFootstepSounds(MaterialSoundsAsset materialSoundsAsset)
        {
            this.m_FootstepSounds.ChangeSoundsAsset(materialSoundsAsset);
        }

        public void PlayFootstepSound(MaterialSoundsAsset materialSoundsAsset)
        {
            if (materialSoundsAsset == null) return;
            
            RaycastHit hit = this.GetGroundHit(this.m_Character.Feet);
            if (hit.collider == null) return;
            
            Args args = new Args(this.m_Character.gameObject, hit.collider.gameObject);
            float yaw = this.m_Character.transform.localRotation.eulerAngles.y;
            
            MaterialSounds.Play(args, hit, materialSoundsAsset, yaw);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnStep(int phase, Transform bone)
        {
            this.LastFootstep = bone.gameObject;
            this.EventStep?.Invoke(bone);

            RaycastHit hit = this.GetGroundHit(bone.position);
            if (hit.collider == null) return;
            
            float speed = Mathf.Clamp01(this.m_Character.Motion.LinearSpeed > 0f
                ? this.m_Character.Driver.WorldMoveDirection.magnitude /
                  this.m_Character.Motion.LinearSpeed 
                : 0f
            );
            
            Args args = new Args(this.m_Character.gameObject, hit.collider.gameObject);
            float yaw = this.m_Character.transform.localRotation.eulerAngles.y;
            
            this.m_FootstepSounds.Play(bone, hit, speed, args, yaw);
        }

        private RaycastHit GetGroundHit(Vector3 position)
        {
            int numHits = Physics.RaycastNonAlloc(
                position, -this.m_Character.transform.up,
                this.m_HitsBuffer,
                this.m_Character.Motion.Height * RAYCAST_DISTANCE_PERCENTAGE,
                this.m_FootstepSounds.LayerMask,
                QueryTriggerInteraction.Ignore
            );

            RaycastHit hit = new RaycastHit();
            float minDistance = Mathf.Infinity;

            for (int i = 0; i < numHits; ++i)
            {
                float distance = Vector3.Distance(
                    this.m_HitsBuffer[i].transform.position,
                    position
                );

                if (distance > minDistance) continue;
                
                hit = this.m_HitsBuffer[i];
                minDistance = distance;
            }

            return hit;
        }

        // GIZMOS: --------------------------------------------------------------------------------
        
        internal void OnDrawGizmos(Character character)
        {
            Gizmos.color = Color.blue;
            
            if (!Application.isPlaying) return;
            if (!this.m_Character.Driver.IsGrounded) return;
            
            Animator animator = this.m_Character.Animim.Animator;
            if (animator == null) return;

            float diameter = this.m_Character.Motion.Radius;
            
            for (int i = 0; i < this.m_Feet.Length && i < Phases.Count; ++i)
            {
                Footstep foot = this.m_Feet[i];
                Transform bone = foot.Bone.GetTransform(animator);
                
                if (bone == null) continue;
                bool isOnGround = this.m_Character.Phases.IsGround(i);

                Gizmos.color = isOnGround ? COLOR_GIZMO_GROUND_ON : COLOR_GIZMO_GROUND_OFF;
                Vector3 position = bone.transform.position;
                position.y = this.m_Character.Feet.y;
                
                GizmosExtension.Circle(position, diameter);
                GizmosExtension.Circle(position, diameter);
            }
        }
    }
}
