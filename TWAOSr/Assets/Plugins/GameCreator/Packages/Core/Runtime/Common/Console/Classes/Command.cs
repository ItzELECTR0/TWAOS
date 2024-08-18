using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public abstract class Command
    {
        private const string ERR_PARAM_NOT_FOUND = "Unable to find parameter '{0}'";
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly Dictionary<PropertyName, IAction> m_Actions;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual bool IsHidden => false;

        internal IEnumerable<IAction> Actions => this.m_Actions.Values;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected Command()
        {
            this.m_Actions = new Dictionary<PropertyName, IAction>();
        }
        
        protected Command(IEnumerable<IAction> actions) : this()
        {
            foreach (IAction action in actions)
            {
                this.m_Actions[action.Name] = action;
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual Output[] Run(Input input) => this.RunDefault(input, null);

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected Output[] RunDefault(Input input, Func<GameObject, Output> function)
        {
            List<Output> outputs = new List<Output>();
            foreach (Parameter parameter in input.Parameters)
            {
                if (this.m_Actions.TryGetValue(parameter.Name, out IAction action))
                {
                    Output output;
                    switch (action)
                    {
                        case ActionOutput actionOutput:
                            output = actionOutput.Run(parameter.Value);
                            break;
                        
                        case ActionGameObject actionGameObject:
                            GameObject gameObject = actionGameObject.Run(parameter.Value);
                            output = function?.Invoke(gameObject);
                            break;

                        default: 
                            output = Output.Error("Undefined Action type"); 
                            break;
                    } 

                    outputs.Add(output);

                    if (output.IsError) return outputs.ToArray();
                }
                else
                {
                    Output output = Output.Error(string.Format(ERR_PARAM_NOT_FOUND, parameter.Name), true);
                    outputs.Add(output);
                    
                    return outputs.ToArray();
                }
            }

            return outputs.ToArray();
        }
    }
}