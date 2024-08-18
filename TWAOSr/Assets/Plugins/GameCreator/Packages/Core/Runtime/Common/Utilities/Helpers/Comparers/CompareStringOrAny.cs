using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class CompareStringOrAny
    {
        private enum Option
        {
            Any = 0,
            Specific = 1
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Option m_Option = Option.Any;
        [SerializeField] private PropertyGetString m_Text = GetStringString.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Any => this.m_Option == Option.Any;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CompareStringOrAny()
        { }
        
        public CompareStringOrAny(PropertyGetString text) : this(false, text)
        { }
        
        public CompareStringOrAny(bool defaultAny, PropertyGetString text) : this()
        {
            this.m_Option = defaultAny ? Option.Any : Option.Specific;
            this.m_Text = text;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Match(string compareTo, Args args)
        {
            if (this.Any) return true;
            return compareTo == this.Get(args);
        }
        
        public bool Match(string compareTo, GameObject args)
        {
            if (this.Any) return true;
            return compareTo == this.Get(args);
        }
        
        public string Get(Args args)
        {
            return this.m_Text.Get(args);
        }

        public string Get(GameObject target)
        {
            return this.m_Text.Get(target);
        }
    }
}
