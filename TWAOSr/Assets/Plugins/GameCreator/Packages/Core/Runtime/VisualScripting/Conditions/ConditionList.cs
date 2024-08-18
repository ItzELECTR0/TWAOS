using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class ConditionList : TPolymorphicList<Condition>
    {
        [SerializeReference]
        private Condition[] m_Conditions = Array.Empty<Condition>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Conditions.Length;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventStartCheck;
        public event Action EventEndCheck;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ConditionList()
        { }

        public ConditionList(params Condition[] conditions) : this()
        {
            this.m_Conditions = conditions;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool Check(Args args, CheckMode mode)
        {
            this.EventStartCheck?.Invoke();

            foreach (Condition condition in this.m_Conditions)
            {
                if (condition == null) continue;
                bool check = condition.Check(args);

                switch (mode)
                {
                    case CheckMode.And:
                        if (check == false)
                        {
                            this.EventEndCheck?.Invoke();
                            return false;
                        }
                        break;
                    
                    case CheckMode.Or:
                        if (check)
                        {
                            this.EventEndCheck?.Invoke();
                            return true;
                        }
                        break;
                    
                    default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }
            
            this.EventEndCheck?.Invoke();
            return mode == CheckMode.And;
        }

        public Condition Get(int index)
        {
            index = Mathf.Clamp(index, 0, this.Length - 1);
            return this.m_Conditions[index];
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Conditions.Length switch
            {
                0 => string.Empty,
                1 => this.m_Conditions[0]?.Title,
                _ => $"{this.m_Conditions[0]?.Title} +{this.m_Conditions.Length - 1}"
            };
        }

        public string ToString(string join)
        {
            return this.m_Conditions.Length switch
            {
                0 => string.Empty,
                1 => this.m_Conditions[0]?.Title,
                2 => $"{this.m_Conditions[0]?.Title} {join} {this.m_Conditions[1]?.Title}",
                _ => $"{this.m_Conditions[0]?.Title} {join} {this.m_Conditions[1]?.Title} +{this.m_Conditions.Length - 2}"
            };
        }
    }
}
