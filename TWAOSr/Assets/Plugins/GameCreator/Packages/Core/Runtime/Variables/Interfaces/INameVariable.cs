using System;

namespace GameCreator.Runtime.Variables
{
    public interface INameVariable
    {
        void Register(Action<string> callback);
        void Unregister(Action<string> callback);
    }
}