namespace GameCreator.Runtime.VisualScripting
{
    public class BranchResult
    {
        public static readonly BranchResult True = new BranchResult(true);
        public static readonly BranchResult False = new BranchResult(false);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public bool Value { get; }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        private BranchResult(bool value)
        {
            this.Value = value;
        }
    }
}