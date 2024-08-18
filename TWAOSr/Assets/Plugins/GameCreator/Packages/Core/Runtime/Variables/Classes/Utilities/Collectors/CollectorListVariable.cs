using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class CollectorListVariable
    {
        private enum Type
        {
            LocalList,
            GlobalList
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Type m_ListVariable = Type.LocalList;

        [SerializeField] private PropertyGetGameObject m_LocalList = new PropertyGetGameObject();
        [SerializeField] private GlobalListVariables m_GlobalList;

        // GETTERS: -------------------------------------------------------------------------------
        
        public List<object> Get(Args args)
        {
            List<object> list = new List<object>();
            
            switch (this.m_ListVariable)
            {
                case Type.LocalList:
                    LocalListVariables localList = this.m_LocalList.Get<LocalListVariables>(args);
                    if (localList != null)
                    {
                        
                        for (int i = 0; i < localList.Count; ++i)
                        {
                            list.Add(localList.Get(i));
                        }   
                    }
                    break;
                
                case Type.GlobalList:
                    if (this.m_GlobalList != null)
                    {
                        for (int i = 0; i < this.m_GlobalList.Count; ++i)
                        {
                            list.Add(this.m_GlobalList.Get(i));
                        }   
                    }
                    break;
                    
                default: throw new ArgumentOutOfRangeException();
            }

            return list;
        }
        
        public int GetCount(Args args)
        {
            switch (this.m_ListVariable)
            {
                case Type.LocalList:
                    LocalListVariables localList = this.m_LocalList.Get<LocalListVariables>(args);
                    return localList != null ? localList.Count : 0;
                    
                case Type.GlobalList:
                    return this.m_GlobalList != null ? this.m_GlobalList.Count : 0;
                    
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public IdString GetTypeId(Args args)
        {
            switch (this.m_ListVariable)
            {
                case Type.LocalList:
                    LocalListVariables localList = this.m_LocalList.Get<LocalListVariables>(args);
                    return localList != null ? localList.TypeID : ValueNull.TYPE_ID;

                case Type.GlobalList:
                    return this.m_GlobalList != null
                        ? this.m_GlobalList.TypeID
                        : ValueNull.TYPE_ID;
                
                default: return ValueNull.TYPE_ID;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Fill(GameObject[] gameObjects, Args args)
        {
            object[] array = new object[gameObjects.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = gameObjects[i];
            }

            this.Fill(array, args);
        }
        
        public void Fill(object[] values, Args args)
        {
            switch (this.m_ListVariable)
            {
                case Type.LocalList:
                    LocalListVariables localList = this.m_LocalList.Get<LocalListVariables>(args);
                    if (localList == null) return;
                    localList.Clear();
                    
                    foreach (object value in values)
                    {
                        if (value == null) continue;
                        localList.Push(value);
                    }
                    break;
                
                case Type.GlobalList:
                    if (this.m_GlobalList == null) return;
                    this.m_GlobalList.Clear();
                    foreach (object value in values)
                    {
                        if (value == null) continue;
                        this.m_GlobalList.Push(value);
                    }
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void Clear(Args args)
        {
            switch (this.m_ListVariable)
            {
                case Type.LocalList:
                    LocalListVariables localList = this.m_LocalList.Get<LocalListVariables>(args);
                    if (localList == null) return;
                    localList.Clear();
                    break;
                
                case Type.GlobalList:
                    if (this.m_GlobalList == null) return;
                    this.m_GlobalList.Clear();
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void Remove(IListGetPick pick, Args args)
        {
            switch (this.m_ListVariable)
            {
                case Type.LocalList:
                    LocalListVariables localList = this.m_LocalList.Get<LocalListVariables>(args);
                    if (localList == null) return;
                    localList.Remove(pick, args);
                    break;
                
                case Type.GlobalList:
                    if (this.m_GlobalList == null) return;
                    this.m_GlobalList.Remove(pick, args);
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_ListVariable switch
            {
                Type.LocalList => this.m_LocalList.ToString(),
                Type.GlobalList => this.m_GlobalList != null 
                    ? this.m_GlobalList.name 
                    : "(none)",
                
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}