namespace GameCreator.Runtime.Common
{
    public static class Settings
    {
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static T From<T>() where T : class, IRepository, new()
        {
            return TRepository<T>.Get;
        }
    }
}