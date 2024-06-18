using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Cameras
{
    [Serializable]
    public struct ShakeEffect
    {
        public const float COEF_SHAKE_POSITION = 01.0f;
        public const float COEF_SHAKE_ROTATION = 25.0f;
        
        private const int SEED_MIN = 000;
        private const int SEED_MAX = 999;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private bool m_ShakePosition;
        [SerializeField] private bool m_ShakeRotation;

        [SerializeField] private float m_Magnitude;
        [SerializeField] private float m_Roughness;
        
        [SerializeField] private Transform m_Transform;
        [SerializeField] private float m_Radius;
        
        // MEMBERS: -------------------------------------------------------------------------------

        private bool m_IsInitialized;
        
        private Vector3 m_Seed;
        private float m_NoiseSpeed;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Vector3 Value { get; private set; }
        
        public float PositionWeight => this.m_ShakePosition ? 1f : 0f;
        public float RotationWeight => this.m_ShakeRotation ? 1f : 0f;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private ShakeEffect(float magnitude, float roughness)
        {
            this.m_Magnitude = magnitude;
            this.m_Roughness = roughness;

            this.m_ShakePosition = true;
            this.m_ShakeRotation = true;
            
            this.m_Transform = null;
            this.m_Radius = 10f;

            this.m_IsInitialized = false;
            this.m_Seed = Vector3.zero;
            this.m_NoiseSpeed = 0f;

            this.Value = Vector3.zero;
        }

        public static ShakeEffect Create => new ShakeEffect(1f, 1f);

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update(TCamera camera)
        {
            if (!this.m_IsInitialized)
            {
                this.m_NoiseSpeed = 0.0f;
                this.m_Seed = new Vector3(
                    UnityEngine.Random.Range(SEED_MIN, SEED_MAX),
                    UnityEngine.Random.Range(SEED_MIN, SEED_MAX),
                    UnityEngine.Random.Range(SEED_MIN, SEED_MAX)
                );
                
                this.m_IsInitialized = true;
            }

            Vector3 noise = this.GetPerlinNoise();
            this.m_NoiseSpeed += Time.deltaTime * this.m_Roughness;

            float distance = Vector3.Distance(
                this.m_Transform != null ? this.m_Transform.position : camera.transform.position, 
                camera.transform.position
            );
            
            float distanceFalloffCoefficient = 1f - Mathf.Clamp01(distance/this.m_Radius);
            this.Value = noise * (this.m_Magnitude * distanceFalloffCoefficient);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector3 GetPerlinNoise()
        {
            return new Vector3(
                Mathf.PerlinNoise(this.m_NoiseSpeed, this.m_Seed.x) - 0.5f,
                Mathf.PerlinNoise(this.m_NoiseSpeed, this.m_Seed.y) - 0.5f,
                Mathf.PerlinNoise(this.m_NoiseSpeed, this.m_Seed.z) - 0.5f
            );
        }
    }
}