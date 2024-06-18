using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public class ShotSystemNoise : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemNoise).GetHashCode();
        
        private const float SEED_MIN = -99f;
        private const float SEED_MAX = 99f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertyGetRotation m_Angle = GetRotationEuler.Create(Vector3.one);
        [SerializeField] private PropertyGetPosition m_Movement = GetPositionVector3.Create(Vector3.zero);

        [SerializeField] private PropertyGetDecimal m_AngularSpeed = GetDecimalDecimal.Create(0.25f);
        [SerializeField] private PropertyGetDecimal m_LinearSpeed = GetDecimalDecimal.Create(0.5f);
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private float m_SeedAngleX;
        [NonSerialized] private float m_SeedAngleY;
        [NonSerialized] private float m_SeedAngleZ;
        
        [NonSerialized] private float m_SeedMoveX;
        [NonSerialized] private float m_SeedMoveY;
        [NonSerialized] private float m_SeedMoveZ;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        // IMPLEMENTS: ----------------------------------------------------------------------------

        public override void OnAwake(TShotType shotType)
        {
            base.OnAwake(shotType);
            
            this.m_SeedAngleX = UnityEngine.Random.Range(SEED_MIN, SEED_MAX);
            this.m_SeedAngleY = UnityEngine.Random.Range(SEED_MIN, SEED_MAX);
            this.m_SeedAngleZ = UnityEngine.Random.Range(SEED_MIN, SEED_MAX);
            
            this.m_SeedMoveX = UnityEngine.Random.Range(SEED_MIN, SEED_MAX);
            this.m_SeedMoveY = UnityEngine.Random.Range(SEED_MIN, SEED_MAX);
            this.m_SeedMoveZ = UnityEngine.Random.Range(SEED_MIN, SEED_MAX);
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);

            float angularSpeed = (float) this.m_AngularSpeed.Get(shotType.Args);
            float linearSpeed = (float) this.m_LinearSpeed.Get(shotType.Args);
            
            float time = shotType.ShotCamera.TimeMode.Time;
            
            float noiseAngleX = this.GetNoise(this.m_SeedAngleX, angularSpeed, time);
            float noiseAngleY = this.GetNoise(this.m_SeedAngleY, angularSpeed, time);
            float noiseAngleZ = this.GetNoise(this.m_SeedAngleZ, angularSpeed, time);
            
            float noiseMoveX = this.GetNoise(this.m_SeedMoveX, linearSpeed, time);
            float noiseMoveY = this.GetNoise(this.m_SeedMoveY, linearSpeed, time);
            float noiseMoveZ = this.GetNoise(this.m_SeedMoveZ, linearSpeed, time);

            Vector3 angle = this.m_Angle.Get(shotType.Args).eulerAngles;
            Vector3 movement = this.m_Movement.Get(shotType.Args);
            
            Quaternion rotation = Quaternion.Euler(
                noiseAngleX * angle.x,
                noiseAngleY * angle.y,
                noiseAngleZ * angle.z
            );

            Vector3 translation = new Vector3(
                noiseMoveX * movement.x,
                noiseMoveY * movement.y,
                noiseMoveZ * movement.z
            );

            shotType.Rotation *= rotation;
            shotType.Position += translation;
        }

        private float GetNoise(float seed, float speed, float time)
        {
            float perlinNoise = Mathf.PerlinNoise(seed, (seed + time) * speed); 
            return Mathf.Clamp01(perlinNoise) * 2f - 1f;
        }
    }
}