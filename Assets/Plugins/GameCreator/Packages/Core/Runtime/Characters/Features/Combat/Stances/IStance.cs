namespace GameCreator.Runtime.Characters
{
    public interface IStance
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        int Id { get; }
        Character Character { get; set; }

        // METHODS: -------------------------------------------------------------------------------

        void OnEnable(Character character);
        void OnDisable(Character character);
        void OnUpdate();
    }
}