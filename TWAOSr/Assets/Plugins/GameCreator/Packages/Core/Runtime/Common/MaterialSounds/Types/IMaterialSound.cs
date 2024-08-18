using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface IMaterialSound
    {
        public float Volume { get; }
        public AudioClip Audio { get; }
        
        public PoolField Impact { get; }
    }
}