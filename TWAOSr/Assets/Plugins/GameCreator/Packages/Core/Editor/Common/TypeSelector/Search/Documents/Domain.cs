using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Search
{
    internal class Domain
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public Dictionary<int, Document> Documents { get; }
        public Dictionary<int, Field> Fields { get; }
        
        public Dictionary<string, HashSet<int>> Terms { get; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public Domain(Type type)
        {
            this.Documents = new Dictionary<int, Document>();
            this.Fields = new Dictionary<int, Field>();
            this.Terms = new Dictionary<string, HashSet<int>>();
            
            Type[] collection = TypeUtils.GetTypesDerivedFrom(type).ToArray();

            foreach (Type entry in collection)
            {
                Document document = new Document(entry);
                this.Documents.Add(document.DocumentId, document);
                
                TitleAttribute[] title = entry.GetCustomAttributes<TitleAttribute>().ToArray();
                CategoryAttribute[] category = entry.GetCustomAttributes<CategoryAttribute>().ToArray();
                DescriptionAttribute[] description = entry.GetCustomAttributes<DescriptionAttribute>().ToArray();
                ParameterAttribute[] parameters = entry.GetCustomAttributes<ParameterAttribute>().ToArray();
                KeywordsAttribute[] keywords = entry.GetCustomAttributes<KeywordsAttribute>().ToArray();

                if (title.Length == 0 || title[0] == null)
                {
                    title = new[] 
                    {
                        new TitleAttribute(TypeUtils.GetNiceName(entry))
                    };
                }
                
                Field[] fields = 
                {
                    Field.First(0, document.DocumentId, title),
                    Field.First(1, document.DocumentId, category),
                    Field.First(2, document.DocumentId, description),
                    Field.Joins(3, document.DocumentId, parameters),
                    Field.Joins(4, document.DocumentId, keywords)
                };

                foreach (Field field in fields)
                {
                    this.Fields[field.FieldId] = field;
                    
                    foreach (string uniqueTerm in field.TermsPositions.Keys)
                    {
                        if (!this.Terms.ContainsKey(uniqueTerm))
                        {
                            this.Terms.Add(uniqueTerm, new HashSet<int>());
                        }
                        
                        this.Terms[uniqueTerm].Add(field.FieldId);
                    }
                }
            }
        }
    }
}