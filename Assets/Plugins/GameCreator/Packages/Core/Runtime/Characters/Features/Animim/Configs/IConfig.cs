namespace GameCreator.Runtime.Characters.Animim
{
    public interface IConfig
    {
        float DelayIn { get; set; }
        float Duration { get; set; }
        float Speed { get; set; }
        float Weight { get; set; }
        
        bool RootMotion { get; set; }

        float TransitionIn { get; set; }
        float TransitionOut { get; set; }
    }
}