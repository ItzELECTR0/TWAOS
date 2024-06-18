using System;

namespace GameCreator.Runtime.Characters
{
    public interface IMunition : ICloneable
    {
        int Id { get; }
        TMunitionValue Value { get; }
    }
}