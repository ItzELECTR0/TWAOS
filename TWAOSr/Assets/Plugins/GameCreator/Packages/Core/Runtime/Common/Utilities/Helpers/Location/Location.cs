using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct Location : ILocation
    {
        public static readonly Location None = new Location(
            new PositionNone(),
            new RotationNone()
        );

        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] private IPosition Position { get; }
        [field: NonSerialized] private IRotation Rotation { get; }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasPosition(GameObject source) => this.Position?.HasPosition(source) ?? false;
        public bool HasRotation(GameObject source) => this.Rotation?.HasRotation(source) ?? false;

        public Vector3 GetPosition(GameObject source) => this.Position?.GetPosition(source) ?? default;
        public Quaternion GetRotation(GameObject source) => this.Rotation?.GetRotation(source) ?? default;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public Location(Vector3 position)
        {
            this.Position = new PositionConstant(position);
            this.Rotation = new RotationNone();
        }
        
        public Location(Quaternion rotation)
        {
            this.Position = new PositionNone();
            this.Rotation = new RotationConstant(rotation);
        }
        
        public Location(Vector3 position, Quaternion rotation)
        {
            this.Position = new PositionConstant(position);
            this.Rotation = new RotationConstant(rotation);
        }

        public Location(Marker marker)
        {
            this.Position = new PositionMarker(marker);
            this.Rotation = new RotationMarker(marker);
        }
        
        public Location(IPosition position, IRotation rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}