using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class Args
    {
        public static readonly Args EMPTY = new Args();
        
        [NonSerialized] private readonly Dictionary<int, Component> m_SelfComponents;
        [NonSerialized] private readonly Dictionary<int, Component> m_TargetComponents;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public GameObject Self   { get; private set; }
        [field: NonSerialized] public GameObject Target { get; private set; }
        
        public Args Clone => new Args(this.Self, this.Target);

        // CONSTRUCTORS: --------------------------------------------------------------------------

        private Args()
        {
            this.m_SelfComponents = new Dictionary<int, Component>();
            this.m_TargetComponents = new Dictionary<int, Component>();
        }

        public Args(Component target) : this(target, target)
        { }

        public Args(GameObject target) : this(target, target)
        { }

        public Args(Component self, Component target) : this()
        {
            this.Self = self == null ? null : self.gameObject;
            this.Target = target == null ? null : target.gameObject;
        }

        public Args(GameObject self, GameObject target) : this()
        {
            this.Self = self;
            this.Target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T ComponentFromSelf<T>(bool inChildren = false) where T : Component
        {
            return this.GetComponent<T>(this.m_SelfComponents, this.Self, inChildren);
        }

        public T ComponentFromTarget<T>(bool inChildren = false) where T : Component
        {
            return this.GetComponent<T>(this.m_TargetComponents, this.Target, inChildren);
        }

        public void ChangeSelf(GameObject self)
        {
            if (this.Self == self) return;

            this.Self = self;
            this.m_SelfComponents.Clear();
        }
        
        public void ChangeSelf<T>(T self) where T : Component
        {
            this.ChangeSelf(self != null ? self.gameObject : null);
        }

        public void ChangeTarget(GameObject target)
        {
            if (this.Target == target) return;

            this.Target = target;
            this.m_TargetComponents.Clear();
        }
        
        public void ChangeTarget<T>(T target) where T : Component
        {
            this.ChangeTarget(target != null ? target.gameObject : null);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private TComponent GetComponent<TComponent>(
            IDictionary<int, Component> dictionary, GameObject gameObject, bool inChildren)
            where TComponent : Component
        {
            if (gameObject == null) return null;
            
            int hash = typeof(TComponent).GetHashCode();
            if (!dictionary.TryGetValue(hash, out Component value) || value == null)
            {
                value = inChildren
                    ? gameObject.GetComponent<TComponent>()
                    : gameObject.GetComponentInChildren<TComponent>();

                if (value == null) return null;
                dictionary[hash] = value;
            }

            return value as TComponent;
        }
    }
}