using System;
using System.Collections.Generic;

namespace GameCreator.Editor.Search
{
    public class Index
    {
        // STATIC DATA: ---------------------------------------------------------------------------

        private static readonly Dictionary<Type, Index> Indices = new Dictionary<Type, Index>();
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly Domain m_Domain;
        private readonly Indexer m_Indexer;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        private Index(Type type)
        {
            this.m_Domain = new Domain(type);
            this.m_Indexer = new Indexer(this.m_Domain);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static Index RequireIndex(Type type)
        {
            if (!Indices.TryGetValue(type, out Index index))
            {
                index = new Index(type);
                Indices.Add(type, index);
            }

            return index;
        }

        public static List<Type> Get(Type type, string search)
        {
            Index index = RequireIndex(type);
            return Searcher.Get(search, index.m_Domain, index.m_Indexer);
        }
    }
}