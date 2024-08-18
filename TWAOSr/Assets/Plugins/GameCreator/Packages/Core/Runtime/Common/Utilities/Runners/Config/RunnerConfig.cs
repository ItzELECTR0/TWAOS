using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Runtime.Common
{
    public struct RunnerConfig : IRunnerConfig
    {
        public static readonly RunnerConfig Default = new RunnerConfig();
        
        // MEMBERS: -------------------------------------------------------------------------------

        private string m_Name;
        
        private IRunnerLocation m_Location;
        private ICancellable m_Cancellable;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Name
        {
            get => !string.IsNullOrEmpty(this.m_Name) ? this.m_Name : "Runner";
            set => this.m_Name = value;
        }

        public IRunnerLocation Location
        {
            get => this.m_Location ?? RunnerLocationNone.Create;
            set => this.m_Location = value;
        }

        public ICancellable Cancellable
        {
            get => this.m_Cancellable ?? null;
            set => this.m_Cancellable = value;
        }
    }
}