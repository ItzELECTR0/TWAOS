using System;
using System.Collections.Generic;
using System.Reflection;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TTypeSelector<T> : ITypeSelector where T : VisualElement
    {
        protected readonly Type m_Type;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action<Type, Type> EventChange;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TTypeSelector(Type type, T element)
        {
            this.m_Type = type;
            this.SetupActivator(element);
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void InvokeEventChange(Type previousType, Type newType)
        {
            this.EventChange?.Invoke(previousType, newType);
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract void SetupActivator(T element);

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        protected static List<Type> GetCandidateTypes(Type typeField)
        {
            Type[] allTypes = TypeUtils.GetTypesDerivedFrom(typeField);
            List<Type> types = new List<Type>();

            foreach (Type type in allTypes)
            {
                if (type.IsAbstract) continue;
                if (type.IsInterface) continue;
                if (type.GetCustomAttribute<HideInSelectorAttribute>(true) != null) continue;

                types.Add(type);
            }

            return types;
        }
    }
}