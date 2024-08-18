using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class CompareGameObjectOrAny
    {
        private enum Option
        {
            Any = 0,
            Specific = 1
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Option m_Option = Option.Any;
        [SerializeField] private PropertyGetGameObject m_GameObject = GetGameObjectPlayer.Create();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Any => this.m_Option == Option.Any;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CompareGameObjectOrAny()
        { }

        public CompareGameObjectOrAny(PropertyGetGameObject gameObject) : this(false, gameObject)
        { }
        
        public CompareGameObjectOrAny(bool defaultAny, PropertyGetGameObject gameObject) : this()
        {
            this.m_Option = defaultAny ? Option.Any : Option.Specific;
            this.m_GameObject = gameObject;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Match(GameObject compareTo, Args args)
        {
            if (this.Any) return true;
            return compareTo == this.Get(args);
        }
        
        public bool Match(GameObject compareTo, GameObject args)
        {
            if (this.Any) return true;
            return compareTo == this.Get(args);
        }
        
        public GameObject Get(Args args)
        {
            return this.m_GameObject.Get(args);
        }

        public GameObject Get(GameObject target)
        {
            return this.m_GameObject.Get(target);
        }

        public T Get<T>(Args args) where T : Component
        {
            return this.m_GameObject.Get<T>(args);
        }
        
        public T Get<T>(GameObject target) where T : Component
        {
            return this.m_GameObject.Get<T>(target);
        }
    }
}
