using System;

namespace GameCreator.Editor.Common
{
    public interface ITypeSelector
    {
        event Action<Type, Type> EventChange;
    }
}