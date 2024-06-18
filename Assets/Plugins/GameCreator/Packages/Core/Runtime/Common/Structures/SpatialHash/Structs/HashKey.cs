using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    internal readonly struct HashKey : IEquatable<HashKey>
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] [ReadOnly] private readonly int m_Dimension;
        [NonSerialized] [ReadOnly] private readonly int3 m_Position;
        [NonSerialized] [ReadOnly] private readonly int m_HashCode;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public HashKey(int dimension, int3 position)
        {
            this.m_Dimension = dimension;
            this.m_Position = position;
            this.m_HashCode = HashCode.Combine(dimension, position);
        }
        
        // EQUALITY: ------------------------------------------------------------------------------
        
        public bool Equals(HashKey other)
        {
            return this.m_Position.x == other.m_Position.x &&
                   this.m_Position.y == other.m_Position.y &&
                   this.m_Position.z == other.m_Position.z &&
                   this.m_Dimension == other.m_Dimension;
        }

        public override int GetHashCode()
        {
            return this.m_HashCode;
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static int3 Hash(int dimension, int3 position)
        {
            return new int3(
                (int) math.floor(position.x / (float) dimension),
                (int) math.floor(position.y / (float) dimension),
                (int) math.floor(position.z / (float) dimension)
            );
        }
        
        public static int3 Hash(int clusterSize, Vector3 position)
        {
            return new int3(
                (int) Math.Floor(position.x / clusterSize),
                (int) Math.Floor(position.y / clusterSize),
                (int) Math.Floor(position.z / clusterSize)
            );
        }
    }
}