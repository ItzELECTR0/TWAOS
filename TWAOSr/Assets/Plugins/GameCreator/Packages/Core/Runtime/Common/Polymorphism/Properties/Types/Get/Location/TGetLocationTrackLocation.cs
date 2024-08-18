using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TGetLocationTrackLocation : PropertyTypeGetLocation
    {
        [Flags]
        private enum Axis
        {
            X = 0b001,
            Y = 0b010,
            Z = 0b100
        }

        private enum Rotation
        {
            None,
            TowardsTarget,
            AwayFromTarget
        }
        
        [SerializeField]
        private PropertyGetDecimal m_Distance = GetDecimalDecimal.Create(1f);

        [SerializeField] private Axis m_Axis = Axis.X | Axis.Z;

        [SerializeField] private bool m_TrackTarget;
        [SerializeField] private Rotation m_Rotation = Rotation.None;
        
        public override Location Get(Args args)
        {
            GameObject from = this.GetFrom(args);
            GameObject to = this.GetTo(args);
            
            return this.m_Rotation switch
            {
                Rotation.None => this.GetLocation(from, to, args, Vector3.zero),
                Rotation.TowardsTarget => this.GetLocationTowards(from, to, args, Vector3.zero),
                Rotation.AwayFromTarget => this.GetLocationAway(from, to, args, Vector3.zero),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        protected abstract GameObject GetFrom(Args args);
        protected abstract GameObject GetTo(Args args);
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Location GetLocation(GameObject from, GameObject to, Args args, Vector3 offset)
        {
            return new Location(
                this.GetPosition(from, to, args, offset),
                new RotationNone()
            );
        }
        
        private Location GetLocationTowards(GameObject from, GameObject to, Args args, Vector3 offset)
        {
            return new Location(
                this.GetPosition(from, to, args, offset),
                this.GetRotation(from, to , args, true)
            );
        }
        
        private Location GetLocationAway(GameObject from, GameObject to, Args args, Vector3 offset)
        {
            return new Location(
                this.GetPosition(from, to, args, offset),
                this.GetRotation(from, to , args, false)
            );
        }

        private IRotation GetRotation(GameObject from, GameObject to, Args args, bool towards)
        {
            if (from == null) return new RotationNone();
            if (to == null) return new RotationNone();

            switch (this.m_TrackTarget)
            {
                case true:
                    return towards
                        ? new RotationTowards(to.transform)
                        : new RotationAway(to.transform);
                    
                case false:
                    Vector3 direction = to.transform.position - from.transform.position;
                    if (from.Get<Character>() != null)
                    {
                        direction = Vector3.Scale(direction, Vector3Plane.NormalUp);
                    }
                    
                    return towards
                        ? new RotationConstant(Quaternion.LookRotation(direction))
                        : new RotationConstant(Quaternion.LookRotation(-direction));
            }
        }
        
        private IPosition GetPosition(GameObject from, GameObject to, Args args, Vector3 offset)
        {
            if (from == null) return new PositionNone();
            if (to == null) return new PositionNone();
            
            float distance = (float) this.m_Distance.Get(args);

            Vector3 axis = new Vector3(
                this.m_Axis.HasFlag(Axis.X) ? 1f : 0f,
                this.m_Axis.HasFlag(Axis.Y) ? 1f : 0f,
                this.m_Axis.HasFlag(Axis.Z) ? 1f : 0f
            );
            
            switch (this.m_TrackTarget)
            {
                case true:
                    return new PositionTowards(
                        to.transform,
                        axis,
                        offset,
                        distance
                    );
                
                case false:
                    Vector3 position = to.transform.TransformPoint(offset);
                    
                    if (distance > 0f)
                    {
                        Vector3 direction = (position - from.transform.position).normalized;
                        position -= direction * distance;
                    }
                    
                    return new PositionConstant(
                        new Vector3(
                            axis.x >= 0.5f ? position.x : from.transform.position.x,
                            axis.y >= 0.5f ? position.y : from.transform.position.y,
                            axis.z >= 0.5f ? position.z : from.transform.position.z
                        )
                    );
            }
        }
    }
}