using System;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Circle Spawn")]
    [Category("Circle Spawn")]
    
    [Image(typeof(IconLocation), ColorTheme.Type.Pink)]
    [Description("Spawn projectile in a circle pattern")]

    [Keywords("Spawn", "Projectile", "Circle")]
    
    [Serializable]
    public class GetSpawnMethodCircle : PropertyTypeGetSpawnMethod
    {
        [SerializeField] private PropertyGetLocation m_SpawnPoint = new();
        [SerializeField] private PropertyGetInteger m_SpawnCount = new(6);
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(.45f);
        
        public override string String => "Circle Spawn";
        
        public override void Spawn(ExtendedArgs args, Projectile projectile)
        {
            var location = m_SpawnPoint.Get(args);

            var spawnPoint = location.HasPosition(args.Self)
                ? location.GetPosition(args.Self)
                : args.Self != null ? args.Self.transform.position : Vector3.zero;
            
            var points = GetPointsAroundCircle
            (
                (int) m_SpawnCount.Get(args),
                spawnPoint,
                args.Self.transform.rotation,
                (float) m_Radius.Get(args)
            );

            foreach (var point in points)
            {
                SpawnProjectile(args, projectile, point, point - spawnPoint);
            }
        }
        
        private Vector3[] GetPointsAroundCircle(int count, Vector3 center, Quaternion rotation, float radius)
        {
            var points = new Vector3[count];
            for (var i = 0; i < count; i++)
            {
                var radians = 2 * Mathf.PI / count * i;

                var vertical = Mathf.Sin(radians);
                var horizontal = Mathf.Cos(radians); 
         
                var direction = new Vector3 (horizontal, 0, vertical);

                points[i] = center + rotation * direction * radius;
            }

            return points;
        }
        
        private void SpawnProjectile(ExtendedArgs args, Projectile projectile, Vector3 point, Vector3 direction)
        {
            var rotation = direction != Vector3.zero 
                ? Quaternion.LookRotation(direction, Vector3.up)
                : Quaternion.identity;
            
            var runtimeProjectile = projectile.Get
            (
                args,
                point,
                rotation
            );
            
            args = args.Clone;
            args.ChangeSelf(runtimeProjectile);
            
            runtimeProjectile.Initialize(args, direction);
        }
    }
}