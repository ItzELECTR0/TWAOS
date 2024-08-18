namespace GameCreator.Editor.Search
{
    internal class Pipelines
    {
        public static readonly Pipelines Indexing = new Pipelines(
            new PipelineTrimmer()
            // new PipelineStemmer() So much research and it works better off without stemming...
        );
        
        public static readonly Pipelines Searching = new Pipelines(
            new PipelineTrimmer()
        );
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        private IPipeline[] List { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private Pipelines(params IPipeline[] pipelines)
        {
            this.List = pipelines;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Process(string[] terms)
        {
            foreach (IPipeline pipeline in this.List)
            {
                for (int i = 0; i < terms.Length; ++i)
                {
                    terms[i] = pipeline.Run(terms[i]);
                }
            }
        }
    }
}