using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace DaimahouGames.Runtime.Abilities
{
    [CreateAssetMenu(menuName = "Game Creator/Abilities/Projectile")]    
    [Icon(AbilityPaths.GIZMOS + "GizmoProjectile.png")]
    
    public class Projectile : ScriptableObject
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
     
        [SerializeField] private PropertyGetInstantiate m_Prefab = new();
        
        [SerializeField] private AnimationCurve m_VerticalDeviation = new(new Keyframe[]
        {
            new(0, 0),
            new(1, 0)
        });
        
        [SerializeField] private AnimationCurve m_HorizontalDeviation = new(new Keyframe[]
        {
            new(0, 0),
            new(1, 0)
        });
        
        [SerializeField] private AnimationCurve m_BackwardDeviation = new(new Keyframe[]
        {
            new(0, 0),
            new(1, 0)
        });
        
        [SerializeField] private AnimationCurve m_Acceleration = new(new Keyframe[]
        {
            new(0, 1),
            new(1, 1)
        });
        
        [SerializeField] private float m_Speed = 5f;
        [SerializeField] private float m_Frequency = 1f;
        [SerializeField] private float m_ProjectileRange = 5f;
        [SerializeReference] private AbilityFilter[] m_Filters;
        [SerializeReference] private AbilityEffect[] m_Effects;

        [SerializeField] private Vector3 m_DeviationMultiplier;
        [SerializeField] private bool m_EnableDeviation;
        [SerializeField] private bool m_RandomizeDeviation;
        [SerializeField] private bool m_LoopDeviation;
        
        [SerializeField] private bool m_PierceTarget;
        [FormerlySerializedAs("m_AlwaysDestroy")] [SerializeField] private bool m_AlwaysExplode;
        [SerializeField] private bool m_ConstantRange;
        
        [SerializeField] private bool m_Homing;
        [SerializeField] private float m_HomingPrecision = 10;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public IEnumerable<AbilityFilter> Filters => m_Filters;
        public IEnumerable<AbilityEffect> Effects => m_Effects;
        public float Speed => m_Speed;
        public bool IsPiercing => m_PierceTarget;
        public bool RandomizeDeviation => m_RandomizeDeviation;
        public bool LoopDeviation => m_LoopDeviation;
        public float Frequency => m_Frequency;
        public bool ConstantRange => m_ConstantRange;
        public bool AlwaysExplode => m_AlwaysExplode;
        
        public bool IsHoming => m_Homing;
        public float HomingPrecision => m_HomingPrecision;
        public float ProjectileRange => m_ProjectileRange;

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public RuntimeProjectile Get(Args args, Vector3 spawnLocation) => Get(args, spawnLocation, Quaternion.identity);
        
        public RuntimeProjectile Get(Args args, Vector3 spawnLocation, Quaternion rotation)
        {
            var instance = m_Prefab.Get(args, spawnLocation, rotation);
            var runtimeProjectile = instance.GetComponent<RuntimeProjectile>();

            if (runtimeProjectile == null) runtimeProjectile = instance.AddComponent<RuntimeProjectile>();

            runtimeProjectile.SetProjectile(this);
            
            return runtimeProjectile;
        }

        public float EvaluateSpeedCurve(float value)
        {
            return m_Acceleration.Evaluate(value);
        }

        public Vector3 EvaluateElevationCurve(float sample)
        {
            if(!m_EnableDeviation) return Vector3.zero;
            
            return new Vector3
            (
                m_HorizontalDeviation.Evaluate(sample) * m_DeviationMultiplier.x,
                m_VerticalDeviation.Evaluate(sample) * m_DeviationMultiplier.y,
                m_BackwardDeviation.Evaluate(sample) * m_DeviationMultiplier.z
            );
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}