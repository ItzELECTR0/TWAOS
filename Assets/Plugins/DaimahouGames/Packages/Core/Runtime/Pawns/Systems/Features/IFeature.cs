namespace DaimahouGames.Runtime.Pawns
{
    public interface IFeature
    {
        void Awake();
        void Start();
        void Update();
        void Enable();
        void Disable();
    }
}