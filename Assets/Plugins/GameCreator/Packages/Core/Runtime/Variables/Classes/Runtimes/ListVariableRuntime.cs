using System;
using System.Collections;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class ListVariableRuntime : TVariableRuntime<IndexVariable>
    {
        public enum Change
        {
            Set    = 0x0001,
            Insert = 0x0010,
            Remove = 0x0100,
            Move   = 0x1000
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeReference] private IndexList m_List = new IndexList();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        internal List<IndexVariable> Variables { get; private set; }

        public IdString TypeID => this.m_List.TypeID;
        
        public int Count => this.Variables.Count;
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Change, int> EventChange;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ListVariableRuntime()
        {
            this.Variables = new List<IndexVariable>();
        }

        public ListVariableRuntime(IndexList indexList) : this()
        {
            this.m_List = indexList;
        }
        
        public ListVariableRuntime(IdString typeID, params IndexVariable[] indexList) : this()
        {
            this.m_List = new IndexList(typeID, indexList);
        }
        
        // INITIALIZERS: --------------------------------------------------------------------------

        public override void OnStartup()
        {
            this.Variables = new List<IndexVariable>();
            
            for (int i = 0; i < this.m_List.Length; ++i)
            {
                IndexVariable variable = this.m_List.Get(i);
                if (variable == null) continue;
                
                this.Variables.Add(variable.Copy as IndexVariable);
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(int index)
        {
            if (index < 0) return null;
            return index < this.Count 
                ? this.Variables[index]?.Value 
                : null;
        }

        public void Set(int index, object value)
        {
            index = Mathf.Clamp(index, 0, this.Count);
            if (index >= this.Count) return;
            
            this.Variables[index].Value = value;
            this.EventChange?.Invoke(Change.Set, index);
        }

        public void Insert(int index, object value)
        {
            index = Mathf.Clamp(index, 0, this.Count);
            TValue content = TValue.CreateValue(this.TypeID, value);
            
            this.Variables.Insert(index, new IndexVariable(content));
            this.EventChange?.Invoke(Change.Insert, index);
        }

        public void Push(object value)
        {
            TValue content = TValue.CreateValue(this.TypeID, value);
            this.Variables.Add(new IndexVariable(content));
            this.EventChange?.Invoke(Change.Insert, this.Count - 1);
        }

        public void Remove(int index)
        {
            index = Mathf.Clamp(index, 0, this.Count);
            if (index >= this.Count) return;
            
            this.Variables.RemoveAt(index);
            this.EventChange?.Invoke(Change.Remove, index);
        }

        public void Move(int source, int destination)
        {
            source = Mathf.Clamp(source, 0, this.Count);
            destination = Mathf.Clamp(destination, 0, this.Count);
            
            if (source >= this.Count) return;
            if (destination >= this.Count) return;
            
            IndexVariable item = this.Variables[source];
            this.Variables.RemoveAt(source);
            this.Variables.Insert(destination, item);
            this.EventChange?.Invoke(Change.Move, destination);
        }
        
        public string Title(int index)
        {
            if (index < 0) return null;
            return index < this.Count ? this.Variables[index]?.Title : string.Empty;
        }
        
        public Texture Icon(int index)
        {
            if (index < 0) return null;
            return index < this.Count ? this.Variables[index]?.Icon : null;
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------
        
        public override IEnumerator<IndexVariable> GetEnumerator()
        {
            return this.Variables.GetEnumerator();
        }
    }
}