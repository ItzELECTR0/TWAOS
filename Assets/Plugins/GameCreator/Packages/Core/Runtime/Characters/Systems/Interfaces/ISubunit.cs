namespace GameCreator.Runtime.Characters
{
    public interface ISubunit<in T> where T : IUnitCommon
    {
        void OnStartup(T unit, Character character);
        void OnDispose(T unit, Character character);

        void OnEnable(T unit);
        void OnDisable(T unit);

        void OnUpdate(T unit);
    }
}