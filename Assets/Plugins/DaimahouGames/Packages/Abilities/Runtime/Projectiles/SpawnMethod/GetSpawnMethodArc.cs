using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Arc Spawn")]
    [Category("Arc Spawn")]
    
    [Image(typeof(IconLocation), ColorTheme.Type.Pink)]
    [Description("Spawn projectile in a cone pattern")]

    [Keywords("Spawn", "Projectile", "Arc")]
    
    [Serializable]
    public class GetSpawnMethodArc : PropertyTypeGetSpawnMethod
    {
        [SerializeField] private PropertyGetLocation m_SpawnPoint = new();
        [SerializeField] private PropertyGetInteger m_SpawnCount = new(6);
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(.25f);
        [SerializeField] private PropertyGetInteger m_ArcAngle = new(60);
        
        [SerializeField] private float m_SpawnDelay;
        [SerializeField] private bool m_RandomSpawn;
        [SerializeField] private float m_MaxSpawnDuration = .5f;
        public override string String => "Arc Spawn";
        
        public override async void Spawn(ExtendedArgs args, Projectile projectile)
        {
            args = args.Clone;
            
            var location = m_SpawnPoint.Get(args);

            var spawnPoint = location.HasPosition(args.Self)
                ? location.GetPosition(args.Self)
                : args.Self != null ? args.Self.transform.position : Vector3.zero;
            
            var spawnCount = (int) m_SpawnCount.Get(args);
            var points = GetPointsAroundArc
            (
                spawnCount,
                spawnPoint,
                args.Self.transform.rotation,
                (float) m_Radius.Get(args),
                (float) m_ArcAngle.Get(args)
            );
            
            var spawnDuration = Mathf.Min(m_SpawnDelay * spawnCount, m_MaxSpawnDuration);

            if (m_RandomSpawn)
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var start = Random.Range(0, points.Length);
                    var end = Random.Range(0, points.Length);
                    var lerp = Random.Range(0, 1f);
                    var point = Vector3.Lerp(points[start], points[end], lerp);
                    
                    SpawnProjectile(args, projectile, point, point - spawnPoint);
                    await Awaiters.Seconds(spawnDuration / spawnCount);
                }
            }
            else
            {
                foreach (var point in points)
                {
                    SpawnProjectile(args, projectile, point, point - spawnPoint);
                    await Awaiters.Seconds(spawnDuration / spawnCount);
                }
            }
        }

        private Vector3[] GetPointsAroundArc(int count, Vector3 center, Quaternion rotation, float radius, float angle)
        {
            var points = new Vector3[count];
            for (var i = 0; i < count; i++)
            {
                var radians = Mathf.Deg2Rad * angle / (Mathf.Max(1, count-1)) * i;
                radians += Mathf.PI / 2 - Mathf.Deg2Rad * angle / 2;

                var vertical = Mathf.Sin(radians);
                var horizontal = Mathf.Cos(radians);

                var direction = new Vector3(horizontal, 0, vertical);

                points[i] = center + rotation * direction * radius;
            }

            return points;
        }
        
        private void SpawnProjectile(ExtendedArgs args, Projectile projectile, Vector3 spawnPoint, Vector3 direction)
        {
            var rotation = direction != Vector3.zero 
                ? Quaternion.LookRotation(direction, Vector3.up)
                : Quaternion.identity;
            
            var runtimeProjectile = projectile.Get
            (
                args,
                spawnPoint,
                rotation
            );
            
            args = args.Clone;
            args.ChangeSelf(runtimeProjectile);

            runtimeProjectile.Initialize(args, direction);
        }
    }
}