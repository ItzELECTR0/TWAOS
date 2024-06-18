using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TSerializableTree<TValue> where TValue : class
    {
        public const string NAME_DATA = nameof(m_Data);
        public const string NAME_NODES = nameof(m_Nodes);
        public const string NAME_ROOTS = nameof(m_Roots);

        public const string NAME_DATA_KEYS = TTreeData<TValue>.NAME_KEYS;
        public const string NAME_DATA_VALUES = TTreeData<TValue>.NAME_VALUES;
        
        public const string NAME_NODES_KEYS = TreeNodes.NAME_KEYS;
        public const string NAME_NODES_VALUES = TreeNodes.NAME_VALUES;
        
        public const string NAME_NODE_CHILDREN = TreeNode.NAME_CHILDREN;

        public const int NODE_INVALID = TreeNode.INVALID;
        
        // EDITOR MEMBERS: ------------------------------------------------------------------------
        
        /// <summary>
        /// [01/03/2023] HACK: This value is used as a work-around to an ongoing Unity issue where
        /// modifying a serialized object is not marked as 'dirty' and attempting to flush
        /// changes to disk fails, since it detects no changes at all.
        /// </summary>
        
        #pragma warning disable 414
        [SerializeField] internal int m_Dirty;
        #pragma warning restore 414

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected TTreeData<TValue> m_Data;
        [SerializeField] protected TreeNodes m_Nodes;
        [SerializeField] protected List<int> m_Roots;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int[] RootIds
        {
            get => this.m_Roots.ToArray();
            set => this.m_Roots = new List<int>(value);
        }
        
        public int FirstRootId => this.m_Roots.Count > 0 
            ? this.m_Roots[0] 
            : NODE_INVALID;

        public TreeNodes Nodes => this.m_Nodes;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventChange;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public TSerializableTree()
        {
            this.m_Data = new TTreeData<TValue>();
            this.m_Nodes = new TreeNodes();
            this.m_Roots = new List<int>();
        }
        
        // GETTERS: -------------------------------------------------------------------------------

        public bool Contains(int id)
        {
            return this.m_Data.ContainsKey(id);
        }

        public int SiblingIndex(int id)
        {
            int parent = this.Parent(id);
            
            int[] children = parent != NODE_INVALID
                ? this.Children(parent)?.ToArray() ?? Array.Empty<int>()
                : this.RootIds;

            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] == id) return i;
            }

            return NODE_INVALID;
        }
        
        public TValue Get(int id)
        {
            return this.m_Data.TryGetValue(id, out TTreeDataItem<TValue> value) 
                ? value.Value 
                : null;
        }

        public int Parent(int id)
        {
            return this.m_Nodes.TryGetValue(id, out TreeNode node) 
                ? node.Parent 
                : TreeNode.INVALID;
        }

        public int PreviousSibling(int id)
        {
            int index = this.SiblingIndex(id);
            
            int parent = this.Parent(id);
            int[] children = parent != NODE_INVALID
                ? this.Children(parent)?.ToArray() ?? Array.Empty<int>()
                : this.RootIds;

            int previousIndex = index - 1;
            return previousIndex >= 0 ? children[previousIndex] : NODE_INVALID;
        }
        
        public int NextSibling(int id)
        {
            int index = this.SiblingIndex(id);
            
            int parent = this.Parent(id);
            int[] children = parent != NODE_INVALID
                ? this.Children(parent)?.ToArray() ?? Array.Empty<int>()
                : this.RootIds;

            int nextIndex = index + 1;
            return nextIndex < children.Length ? children[nextIndex] : NODE_INVALID;
        }
        
        public List<int> Children(int id)
        {
            return this.m_Nodes.TryGetValue(id, out TreeNode node) 
                ? new List<int>(node.Children) 
                : new List<int>();
        }

        public List<int> Siblings(int id)
        {
            int parent = this.Parent(id);
            
            return parent != NODE_INVALID
                ? this.Children(parent)
                : new List<int>(this.RootIds);
        }
        
        // SETTERS: -------------------------------------------------------------------------------

        public int AddToRoot(TValue value)
        {
            return this.Add(value, TreeNode.INVALID, this.m_Roots.Count);
        }
        
        public int AddToRoot(TValue value, int index)
        {
            return this.Add(value, TreeNode.INVALID, index);
        }

        public int AddBeforeSibling(TValue value, int sibling)
        {
            if (!this.Contains(sibling)) return TreeNode.INVALID;
            int parent = this.Parent(sibling);

            int index = parent != TreeNode.INVALID 
                ? this.m_Nodes[parent].Children.IndexOf(sibling) 
                : this.m_Roots.IndexOf(sibling);
            
            return this.Add(value, parent, index);
        }
        
        public int AddAfterSibling(TValue value, int sibling)
        {
            if (!this.Contains(sibling)) return TreeNode.INVALID;
            int parent = this.Parent(sibling);

            int index = parent != TreeNode.INVALID 
                ? this.m_Nodes[parent].Children.IndexOf(sibling) 
                : this.m_Roots.IndexOf(sibling);
            
            return this.Add(value, parent, index + 1);
        }

        public int AddChild(TValue value, int parent)
        {
            if (!this.Contains(parent)) return TreeNode.INVALID;
            List<int> children = this.m_Nodes[parent].Children;
            
            return this.Add(value, parent, children.Count);
        }
        
        public int AddChild(TValue value, int parent, int index)
        {
            return this.Add(value, parent, index);
        }

        public bool Remove(int node)
        {
            if (node == NODE_INVALID) return false;
            
            int parent = this.Parent(node);
            List<int> children = this.Children(node);
            
            for (int i = children.Count - 1; i >= 0; --i)
            {
                this.Remove(children[i]);
            }
            
            if (parent == NODE_INVALID)
            {
                for (int i = this.m_Roots.Count - 1; i >= 0; i--)
                {
                    if (this.m_Roots[i] == node) this.m_Roots.RemoveAt(i);
                }
            }
            else
            {
                if (this.m_Nodes.TryGetValue(parent, out TreeNode parentNode))
                {
                    parentNode.Children.Remove(node);
                }
            }
            
            this.m_Data.Remove(node);
            this.m_Nodes.Remove(node);

            return true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private int NewId()
		{
			int id = Guid.NewGuid().GetHashCode();
			while (this.m_Data.ContainsKey(id) || id == TreeNode.INVALID)
			{
				id = Guid.NewGuid().GetHashCode();
			}

			return id;
		}
        
        private int Add(TValue value, int parent, int index)
        {
            int newId = this.NewId();
            this.m_Data.Add(newId, new TTreeDataItem<TValue>(newId, value));

            if (parent != TreeNode.INVALID)
            {
                List<int> children = this.m_Nodes[parent].Children;
                children.Insert(index, newId);
            }
            else
            {
                this.m_Roots.Insert(index, newId);
            }

            TreeNode newNode = new TreeNode(newId, parent);
            
            this.m_Nodes.Add(newId, newNode);
            this.EventChange?.Invoke();
            
            return newId;
        }
    }
}