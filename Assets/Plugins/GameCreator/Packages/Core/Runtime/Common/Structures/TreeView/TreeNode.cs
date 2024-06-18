using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TreeNode
    {
        public const int INVALID = -1;

        public const string NAME_CHILDREN = nameof(m_Children);
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private int m_Id;
        
        [SerializeField] private int m_Parent;
        [SerializeField] private List<int> m_Children;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public int Id => this.m_Id;
        
        public int Parent
        {
            get => this.m_Parent;
            set => this.m_Parent = value;
        }

        public List<int> Children
        {
            get => this.m_Children;
            set => this.m_Children = value;
        }
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public TreeNode(int id, int parent)
        {
            this.m_Id = id;
            this.m_Parent = parent;
            this.m_Children = new List<int>();
        }

        // INTERNAL METHODS: ----------------------------------------------------------------------

        // internal void Set(int id, int parent)
        // {
        //     this.m_Id = id;
        //     this.m_Parent = parent;
        // }
    }
}