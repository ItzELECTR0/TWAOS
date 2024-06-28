using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Version(1, 0, 0)]
    
    [Title("Projectile")]
    [Category("Projectile")]
    
    [Description("Spawn an projectile")]

    [Keywords("Projectile", "Missile")]

    [Image(typeof(IconProjectile), ColorTheme.Type.Red)]
    [Serializable]
    public class AbilityEffectProjectile : AbilityEffect
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private Projectile m_Projectile;
        [SerializeField] private PropertyGetSpawnMethod m_SpawnMethod = GetSpawnMethodSingle.Create();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => string.Format("{0} : {1}",
            m_Projectile == null ? "(none)" : m_Projectile.name,
            m_SpawnMethod.String
        );
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override void Apply_Internal(ExtendedArgs args)
        {
            m_SpawnMethod.Spawn(args, m_Projectile);
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        
        //============================================================================================================||
    }
}