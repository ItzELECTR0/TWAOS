namespace GameCreator.Runtime.Common
{
    public static class IntegerCounter
    {
        private static int Counter;

        public static int Generate() => Counter++;
    }
}