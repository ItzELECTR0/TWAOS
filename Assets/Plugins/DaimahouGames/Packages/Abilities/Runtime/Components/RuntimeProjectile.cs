using System.Linq;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DaimahouGames.Runtime.Abilities
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Creator/Abilities/Projectile")]
    
    [Icon(AbilityPaths.GIZMOS + "GizmoProjectile.png")]
    public class RuntimeProjectile : MonoBehaviour
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private float m_DeactivationDelay;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private ExtendedArgs m_Args;
        private Projectile m_Projectile;
        
        private Transform m_Transform;
        private Rigidbody m_Rigidbody;
        
        private Vector3 m_Direction;
        private Target m_Target;
        
        private float m_Duration;
        private float m_LerpTime;
        private float m_LerpDeviation;
        
        private Vector3 m_Position;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        
        private void Awake()
        {
            m_Transform = transform;
            SetupCollider();
            SetupRigidbody();
        }

        private void SetupCollider()
        {
            var projectileCollider = GetComponent<Collider>();
            if (projectileCollider == null)
            {
                var sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = 0.1f;
                projectileCollider = sphereCollider;
            }
            
            projectileCollider.isTrigger = true;
        }

        private void SetupRigidbody()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_Rigidbody == null) m_Rigidbody = gameObject.AddComponent<Rigidbody>();

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
        
        public void SetProjectile(Projectile projectile)
        {
            m_Projectile = projectile;
        }

        public void Initialize(ExtendedArgs args, Vector3 direction)
        {
            m_Args = args;

            m_Target = args.Get<Target>();
            m_Direction = direction.normalized;
            
            m_LerpTime = 0;
            m_LerpDeviation = m_Projectile.RandomizeDeviation && m_Projectile.LoopDeviation 
                ? Random.Range(0, 1f) 
                : 0;
            
            m_Position = m_Transform.position;

            var distance = m_Projectile.ConstantRange
                ? m_Projectile.ProjectileRange
                : Vector3.Distance(transform.position, m_Target.Position);


            var averageAcceleration = 0f;
            var sampleSize = distance / 0.05f;

            for (var i = 0; i < sampleSize; i++)
            {
                var sample = i / sampleSize;
                averageAcceleration += m_Projectile.EvaluateSpeedCurve(sample);
            }

            averageAcceleration /= sampleSize;

            m_Duration = distance / (m_Projectile.Speed * averageAcceleration);
            if(m_Projectile.LoopDeviation) m_Duration /= m_Projectile.Frequency;
            if (m_Duration == 0) m_Duration = 1;
        }
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void FixedUpdate()
        {
            m_LerpTime += Time.fixedDeltaTime / m_Duration;

            var speedMultiplier = m_Projectile.EvaluateSpeedCurve(m_LerpTime);

            m_LerpDeviation += Time.fixedDeltaTime / m_Duration;

            if (ShouldExplode())
            {
                m_Args.Set(new Target(transform.position));
                Trigger(m_Args);
                return;
            }
            
            if(m_Projectile.LoopDeviation) m_LerpDeviation %= 1;

            var speed = m_Projectile.Speed * speedMultiplier;
            var deviation = m_Projectile.EvaluateElevationCurve(m_LerpDeviation);
            
            var deviatedDirection = GetDirection() + transform.TransformDirection(deviation);
            var nextPosition = m_Position + deviatedDirection * (speed * Time.fixedDeltaTime);
            
            m_Position += GetDirection() * (speed * Time.fixedDeltaTime);

            m_Rigidbody.MovePosition(nextPosition);
        }

        private bool ShouldExplode()
        {
            if (!m_Projectile.AlwaysExplode) return false;
            if (m_Projectile.IsHoming && Vector3.Distance(m_Position, m_Target.Position) < 0.1f) return true;
            return !m_Projectile.IsHoming && m_LerpDeviation >= 1;
        }

        private Vector3 GetDirection()
        {
            UpdateHoming();
            return m_Direction;
        }

        private void UpdateHoming()
        {
            if (!m_Projectile.IsHoming) return;
            
            var direction = GetTargetDirection();
            m_Direction = Vector3.Lerp(
                m_Direction,
                direction,
                Time.fixedDeltaTime * m_Projectile.HomingPrecision
            );
        }

        private Vector3 GetTargetDirection() => (m_Target.Position - m_Position).normalized;

        private void OnTriggerEnter(Collider other)
        {
            m_Args.Set(new Target(other, transform.position));
            
            if (m_Projectile.Filters.Any(f => f.Filter(m_Args))) return;

            Trigger(m_Args);
        }

        private void Trigger(ExtendedArgs args)
        {
            var pawn = args.Target.Get<Pawn>();
            if (pawn) pawn.Message.Send(new MessageAbilityHit(args.Get<RuntimeAbility>()?.Caster?.GameObject));
            
            foreach (var effect in m_Projectile.Effects)
            {
                effect.Apply(args);
            }

            if (m_Projectile.IsPiercing) return;
            
            Deactivate();
        }

        private async void Deactivate()
        {
            enabled = false;
            await Awaiters.Seconds(m_DeactivationDelay);
            enabled = true;
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var radius = GetComponent<SphereCollider>()? GetComponent<SphereCollider>().radius : 0.25f;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        //============================================================================================================||
    }
}