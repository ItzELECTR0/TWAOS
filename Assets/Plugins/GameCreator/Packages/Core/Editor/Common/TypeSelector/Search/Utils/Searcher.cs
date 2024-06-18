using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Editor.Search
{
    internal static class Searcher
    {
        private const int NUM_RESULTS = 15;
        private const int INIT_CAPACITY = 5;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        private static List<CandidateField> ListCandidateFields { get; }
        private static List<CandidateDocument> ListCandidateDocuments { get; }
        
        private static Dictionary<int, CandidateDocument> DocumentsCycle { get; }
        private static Dictionary<int, CandidateDocument> DocumentsCollection { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        static Searcher()
        {
            ListCandidateFields = new List<CandidateField>(INIT_CAPACITY);
            ListCandidateDocuments = new List<CandidateDocument>(INIT_CAPACITY);

            DocumentsCycle = new Dictionary<int, CandidateDocument>();
            DocumentsCollection = new Dictionary<int, CandidateDocument>(INIT_CAPACITY);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static List<Type> Get(string search, Domain domain, Indexer indexer)
        {
            DocumentsCollection.Clear();
            
            string[] terms = Tokenizer.Run(search);
            Pipelines.Searching.Process(terms);
            
            foreach (string term in terms)
            {
                if (string.IsNullOrEmpty(term)) continue;
                ListCandidateFields.Clear();
                
                foreach (KeyValuePair<string, List<int>> inverseEntry in indexer.TermFieldIndex)
                {
                    if (string.IsNullOrEmpty(inverseEntry.Key)) continue;
                    
                    int editDistance = Levenshtein.Get(term, inverseEntry.Key);
                    if (editDistance > Levenshtein.MAX_EDITS) continue;
                    
                    foreach (int fieldId in inverseEntry.Value)
                    {
                        CandidateField candidateField = new CandidateField(
                            inverseEntry.Key,
                            fieldId,
                            editDistance,
                            domain
                        );
                        
                        ListCandidateFields.Add(candidateField);
                    }
                }
                
                DocumentsCycle.Clear();

                foreach (CandidateField candidateField in ListCandidateFields)
                {
                    int documentId = domain.Fields[candidateField.FieldId].DocumentId;
                    DocumentsCycle.TryGetValue(documentId, out CandidateDocument termDocument); 
                    
                    if (termDocument.Score >= candidateField.Score) continue;
                    
                    CandidateDocument betterDocument = new CandidateDocument(
                        documentId,
                        candidateField.Score
                    );
                    
                    DocumentsCycle[documentId] = betterDocument;
                }
                
                foreach (KeyValuePair<int, CandidateDocument> termDocumentEntry in DocumentsCycle)
                {
                    DocumentsCollection.TryGetValue(
                        termDocumentEntry.Key,
                        out CandidateDocument currentDocument
                    );
                    
                    DocumentsCollection[termDocumentEntry.Key] = new CandidateDocument(
                        termDocumentEntry.Key,
                        termDocumentEntry.Value.Score + currentDocument.Score
                    );
                }
            }

            ListCandidateDocuments.Clear();
            ListCandidateDocuments.AddRange(DocumentsCollection.Values);
            ListCandidateDocuments.Sort(SortCandidateDocuments);

            int numResults = Math.Min(NUM_RESULTS, ListCandidateDocuments.Count);
            List<Type> result = new List<Type>(numResults);

            for (int i = 0; i < numResults; ++i)
            {
                int documentId = ListCandidateDocuments[i].DocumentId;
                Document document = domain.Documents[documentId];
                
                result.Add(document.Type);
            }
            
            return result;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static int SortCandidateDocuments(CandidateDocument x, CandidateDocument y)
        {
            return y.Score.CompareTo(x.Score);
        }
    }
}