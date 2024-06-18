using System;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TUnitFacing : TUnit, IUnitFacing
    {
        private const float MAX_ANGLE_ERROR = 1f;
        private const float EPSILON_SPEED = 0.1f;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        protected Vector3 m_FaceDirection;

        protected float m_PivotSpeed;
        
        private Quaternion m_RotationVelocity = Quaternion.identity;

        private Dictionary<int, FacingLayer> m_LayersData;
        private List<int> m_LayersQueue;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public Vector3 WorldFaceDirection => this.Transform.TransformDirection(Vector3.forward);
        public Vector3 LocalFaceDirection => Vector3.forward;

        public Vector3 WorldTargetFaceDirection => this.m_FaceDirection;
        public Vector3 LocalTargetFaceDirection => this.Transform.InverseTransformDirection(this.m_FaceDirection);

        public float PivotSpeed => this.m_PivotSpeed;

        public abstract Axonometry Axonometry { get; set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        public virtual void OnStartup(Character character)
        {
            this.Character = character;

            this.m_FaceDirection = character.transform.TransformDirection(Vector3.forward);
            this.m_PivotSpeed = 0f;

            this.m_LayersData = new Dictionary<int, FacingLayer>();
            this.m_LayersQueue = new List<int>();
        }

        public virtual void AfterStartup(Character character)
        {
            this.Character = character;
        }
        
        public virtual void OnDispose(Character character)
        {
            this.Character = character;
        }

        public virtual void OnEnable()
        { }

        public virtual void OnDisable()
        { }

        // UPDATE METHOD: -------------------------------------------------------------------------

        public virtual void OnUpdate()
        {
            if (this.Character.IsDead) return;
            
            for (int i = this.m_LayersQueue.Count - 1; i >= 0; --i)
            {
                int key = this.m_LayersQueue[i];
                bool delete = true;

                if (this.m_LayersData.TryGetValue(key, out FacingLayer layer))
                {
                    delete = layer.Update(this.Character);
                }

                if (delete)
                {
                    this.m_LayersData.Remove(key);
                    this.m_LayersQueue.RemoveAt(i);
                }
            }

            if (this.m_LayersQueue.Count > 0)
            {
                int key = this.m_LayersQueue[0];
                this.m_FaceDirection = this.m_LayersData[key].Direction;
            }
            else
            {
                this.m_FaceDirection = this.GetDefaultDirection();
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(this.m_FaceDirection);

            Quaternion srcRotation = this.Transform.rotation;
            Quaternion dstRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

            Quaternion rotation = QuaternionUtils.SmoothDamp(
                srcRotation, dstRotation,
                ref this.m_RotationVelocity,
                1f / (this.Character.Motion.AngularSpeed / 360f), 
                this.Character.Time.DeltaTime
            );

            this.m_PivotSpeed = Vector3.SignedAngle(
                srcRotation * Vector3.forward,
                dstRotation * Vector3.forward,
                Vector3.up
            );

            this.Transform.rotation = Quaternion.Lerp(
                rotation,
                srcRotation * this.Character.Animim.RootMotionDeltaRotation,
                this.Character.RootMotionRotation
            );
        }

        public virtual void OnFixedUpdate()
        { }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract Vector3 GetDefaultDirection();
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected Vector3 DecideDirection(Vector3 driverDirection)
        {
            Vector3 currentDirection = this.Transform.TransformDirection(Vector3.forward);
            return driverDirection.magnitude > EPSILON_SPEED ? driverDirection : currentDirection;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public virtual void OnDrawGizmos(Character character)
        { }

        // LAYER SYSTEM: --------------------------------------------------------------------------

        private int CreateLayer(bool autoDestroyOnReach)
        {
            int key = IntegerCounter.Generate();

            this.m_LayersQueue.Add(key);
            this.m_LayersData.Add(key, new FacingLayer(this.Character, autoDestroyOnReach));

            return key;
        }
        
        private int CreateLayer(float autoDestroyOnTimeout)
        {
            int key = IntegerCounter.Generate();

            this.m_LayersQueue.Add(key);
            this.m_LayersData.Add(key, new FacingLayer(this.Character, autoDestroyOnTimeout));

            return key;
        }

        public void DeleteLayer(int key)
        {
            this.m_LayersData.Remove(key);
            this.m_LayersQueue.Remove(key);
        }

        public int SetLayerDirection(int key, Vector3 direction, bool autoDestroyOnReach)
        {
            if (this.m_LayersData.TryGetValue(key, out FacingLayer layer))
            {
                layer.SetDirection(direction);
            }
            else
            {
                float angle = Vector3.Angle(direction, this.WorldFaceDirection);
                if (!autoDestroyOnReach || angle > MAX_ANGLE_ERROR)
                {
                    key = this.CreateLayer(autoDestroyOnReach);
                    this.m_LayersData[key].SetDirection(direction);
                }
            }

            return key;
        }
        
        public int SetLayerDirection(int key, Vector3 direction, float autoDestroyOnTimeout)
        {
            if (this.m_LayersData.TryGetValue(key, out FacingLayer layer))
            {
                layer.SetDirection(direction);
            }
            else
            {
                key = this.CreateLayer(autoDestroyOnTimeout);
                this.m_LayersData[key].SetDirection(direction);
            }

            return key;
        }

        public int SetLayerTarget(int key, Transform target)
        {
            if (this.m_LayersData.TryGetValue(key, out FacingLayer layer))
            {
                layer.SetTarget(target);
            }
            else
            {
                key = this.CreateLayer(false);
                this.m_LayersData[key].SetTarget(target);
            }

            return key;
        }
    }
}