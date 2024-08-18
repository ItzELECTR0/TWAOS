namespace GameCreator.Editor.Search
{
    internal static class IdProvider
    {
        private static int DocumentCounter;
        private static int FieldCounter;

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static int DocumentId => ++DocumentCounter;
        public static int FieldId => ++FieldCounter;
    }
}