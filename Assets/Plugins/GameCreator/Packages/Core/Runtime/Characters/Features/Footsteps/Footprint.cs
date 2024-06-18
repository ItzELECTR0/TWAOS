using System;

namespace GameCreator.Runtime.Characters
{
    public class Footprint
    {
        [field: NonSerialized] public bool WasGrounded { get; set; } = true;
    }
}