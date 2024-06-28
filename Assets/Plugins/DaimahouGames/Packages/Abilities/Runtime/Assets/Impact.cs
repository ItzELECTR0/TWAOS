using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [CreateAssetMenu(menuName = "Game Creator/Abilities/Impact")]    
    [Icon(AbilityPaths.GIZMOS + "GizmoExplosion.png")]
    
    public class Impact : ScriptableObject
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private PropertyGetInstantiate m_Prefab = new();

        [SerializeReference] private AbilityEffect[] m_Effects;
        [SerializeReference] private AbilityFilter[] m_Filters;

        [SerializeField] private float m_Delay;
        [SerializeField] private PropertyGetDecimal m_Radius;
        [SerializeField] private LayerMask m_TargetLayer;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public AbilityEffect[] Effects => m_Effects;
        public AbilityFilter[] Filters => m_Filters;
        public int LayerMask => m_TargetLayer;

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public RuntimeImpact Get(Args args, Vector3 spawnLocation, Quaternion rotation)
        {
            var instance = m_Prefab.Get(args, spawnLocation, rotation);
            var impact = instance.GetComponent<RuntimeImpact>();
            if (impact == null) impact = instance.AddComponent<RuntimeImpact>();

            impact.Initialize(this);
            return impact;
        }

        public float Delay => m_Delay;
        public float GetRadius(Args args) => (float) m_Radius.Get(args);

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}