using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class NameVariableRuntime : TVariableRuntime<NameVariable>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeReference] private NameList m_List = new NameList();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        internal Dictionary<string, NameVariable> Variables { get; private set; }

        public NameList TemplateList => this.m_List;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<string> EventChange;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public NameVariableRuntime()
        {
            this.Variables = new Dictionary<string, NameVariable>();
        }
        
        public NameVariableRuntime(NameList nameList) : this()
        {
            this.m_List = nameList;
        }

        public NameVariableRuntime(params NameVariable[] nameList) : this()
        {
            this.m_List = new NameList(nameList);
        }
        
        // INITIALIZERS: --------------------------------------------------------------------------

        public override void OnStartup()
        {
            this.Variables = new Dictionary<string, NameVariable>();
            
            for (int i = 0; i < this.m_List.Length; ++i)
            {
                NameVariable variable = this.m_List.Get(i);
                if (variable == null) continue;
                
                if (this.Variables.ContainsKey(variable.Name)) continue;
                this.Variables.Add(variable.Name, variable.Copy as NameVariable);
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool Exists(string name)
        {
            return this.Variables.ContainsKey(name);
        }

        public object Get(string name)
        {
            NameVariable variable = this.AccessRuntimeVariable(name);
            return variable?.Value;
        }

        public string Title(string name)
        {
            NameVariable variable = this.AccessRuntimeVariable(name);
            return variable?.Title;
        }
        
        public Texture Icon(string name)
        {
            NameVariable variable = this.AccessRuntimeVariable(name);
            return variable?.Icon;
        }

        public void Set(string name, object value)
        {
            NameVariable variable = this.AccessRuntimeVariable(name);
            if (variable == null) return;
            
            variable.Value = value;
            this.EventChange?.Invoke(name);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private NameVariable AccessRuntimeVariable(string name)
        {
            string[] keys = name.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
            string firstKey = keys.Length > 0 ? keys[0] : string.Empty;
            
            NameVariable variable = this.Variables.TryGetValue(firstKey, out NameVariable entry)
                ? entry
                : null;
        
            if (keys.Length <= 1) return variable;
            if (variable?.Value is not GameObject gameObject) return null;
            
            LocalNameVariables variables = gameObject.Get<LocalNameVariables>();
            return variables != null ? variables.Runtime.AccessRuntimeVariable(keys[1]) : null;
        }

        // IMPLEMENTATIONS: -----------------------------------------------------------------------

        public override IEnumerator<NameVariable> GetEnumerator()
        {
            return this.Variables.Values.GetEnumerator();
        }
    }
}