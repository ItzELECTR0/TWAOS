using  System;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.VisualScripting.Direction;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Single Spawn")]
    [Category("Single Spawn")]
    
    [Image(typeof(IconLocation), ColorTheme.Type.Pink)]
    [Description("Spawn a single projectile")]

    [Keywords("Spawn", "Projectile")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetSpawnMethodSingle : PropertyTypeGetSpawnMethod
    {
        public override string String => "Single Spawn";
        
        [SerializeField] private PropertyGetLocation m_SpawnPoint = new();
        [SerializeField] private PropertyGetDirection m_Direction = GetDirectionTowardsTarget.Create;
        
        public override void Spawn(ExtendedArgs args, Projectile projectile)
        {
            var location = m_SpawnPoint.Get(args);

            var spawnLocation = location.HasPosition(args.Self)
                ? location.GetPosition(args.Self)
                : args.Self != null ? args.Self.transform.position : Vector3.zero;
            
            var runtimeProjectile = projectile.Get
            (
                args,
                spawnLocation,
                args.Self.transform.rotation
            );

            args = args.Clone;
            args.ChangeSelf(runtimeProjectile);
 
            var direction = m_Direction.Get(args);
            var rotation = direction != Vector3.zero
                ? Quaternion.LookRotation(direction, Vector3.up)
                : Quaternion.identity;

            runtimeProjectile.transform.rotation = rotation;
            runtimeProjectile.Initialize(args, direction);
        }

        public static PropertyGetSpawnMethod Create() => new(new GetSpawnMethodSingle());
    }
}