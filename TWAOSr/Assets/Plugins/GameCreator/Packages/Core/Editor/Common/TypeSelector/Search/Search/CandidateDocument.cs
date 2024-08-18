namespace GameCreator.Editor.Search
{
    public struct CandidateDocument
    {
        public int DocumentId { get; }
        public double Score { get; }

        public CandidateDocument(int documentId, double score)
        {
            this.DocumentId = documentId;
            this.Score = score;
        }
    }
}