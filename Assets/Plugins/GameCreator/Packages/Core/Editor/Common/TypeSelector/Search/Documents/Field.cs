using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Search
{
    internal class Field
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public int FieldType { get; }
        
        public int FieldId { get; }
        public int DocumentId { get; }
        
        public string Text { get; }
        public int Boost { get; }
        
        public string[] Terms { get; }
        
        public Dictionary<string, int> TermsPositions { get; }
        public Dictionary<string, double> TermsFrequency { get; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private Field(int fieldType, int documentId, string text, int boost)
        {
            this.FieldType = fieldType;
            
            this.FieldId = IdProvider.FieldId;
            this.DocumentId = documentId;
            
            this.Text = text;
            this.Boost = boost;
            
            this.Terms = Tokenizer.Run(this.Text);
            Pipelines.Indexing.Process(this.Terms);

            this.TermsPositions = new Dictionary<string, int>(this.Terms.Length);
            this.TermsFrequency = new Dictionary<string, double>(this.Terms.Length);

            for (int i = 0; i < this.Terms.Length; i++)
            {
                string term = this.Terms[i];
                this.TermsPositions.TryAdd(term, i);
                
                if (this.TermsFrequency.ContainsKey(term))
                {
                    this.TermsFrequency[term] += 1;
                }
                else
                {
                    this.TermsFrequency[term] = 1;
                }
            }

            foreach (KeyValuePair<string, int> uniqueTerm in this.TermsPositions)
            {
                this.TermsFrequency[uniqueTerm.Key] /= this.Terms.Length;
            }
        }

        public static Field First(int fieldType, int documentId, IEnumerable<ISearchable> collection)
        {
            ISearchable entry = collection.FirstOrDefault();
            
            return entry != null 
                ? new Field(fieldType, documentId, entry.SearchText, entry.SearchPriority) 
                : new Field(fieldType, documentId, string.Empty, 1);
        }
        
        public static Field Joins(int fieldType, int documentId, IEnumerable<ISearchable> collection)
        {
            if (collection == null) return new Field(fieldType, documentId, string.Empty, 1);
            
            StringBuilder text = new StringBuilder();
            int boost = 1;
            
            foreach (ISearchable entry in collection)
            {
                if (entry == null) continue;
                
                if (text.Length != 0) text.Append(" "); 
                text.Append(entry.SearchText);
                
                boost = Math.Max(boost, entry.SearchPriority);
            }

            return new Field(fieldType, documentId, text.ToString(), boost);
        }
    }
}