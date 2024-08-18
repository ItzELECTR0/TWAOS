using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Runtime.Common
{
    public interface IRunnerConfig
    {
        string Name { get; }
        
        IRunnerLocation Location { get; }
        ICancellable Cancellable { get; }
    }
}