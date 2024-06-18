using System;

namespace GameCreator.Editor.Search
{
    internal readonly struct CandidateField
    {
        private const double DISTANCE_PENALTY = 3;
        private const double POSITION_SCORE = 0.1;
        
        private const double ENTROPY_SATURATION = 5;
        private const double ENTROPY_SMOOTH = 0.75;
        private const double ENTROPY_BOOST = 1;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public string Term { get; }
        public int FieldId { get; }
        
        public double Score { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CandidateField(string term, int fieldId, int editDistance, Domain domain)
        {
            this.Term = term;
            this.FieldId = fieldId;
            this.Score = GetScore(this.Term, this.FieldId, editDistance, domain);
        }

        private static double GetScore(string term, int fieldId, int editDistance, Domain domain)
        {
            Field field = domain.Fields[fieldId];
            Document document = domain.Documents[field.DocumentId];
            
            double normalDistance = editDistance / (double) Levenshtein.MAX_EDITS * DISTANCE_PENALTY;
            double scoreDistance = (1 - normalDistance) * DISTANCE_PENALTY;
            
            double entropy = GetEntropy(term, fieldId, domain);
            
            double positionRatio = field.TermsPositions[term] / (double) field.Terms.Length;
            double positionScore = (1 - positionRatio) * POSITION_SCORE;
            
            return document.Boost * field.Boost * (scoreDistance + entropy + positionScore);
        }
        
        private static double GetEntropy(string term, int fieldId, Domain domain)
        {
            Field field = domain.Fields[fieldId];
            field.TermsFrequency.TryGetValue(term, out double termFrequency);
            
            int fieldsWithTerm = domain.Terms[term].Count;
            double inverseFieldFrequency = Math.Log(
                domain.Fields.Count / (fieldsWithTerm + ENTROPY_SMOOTH),
                10 * ENTROPY_SATURATION
            );

            return termFrequency * inverseFieldFrequency * ENTROPY_BOOST;
        }
    }
}