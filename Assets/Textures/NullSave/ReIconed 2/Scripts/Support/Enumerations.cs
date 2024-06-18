namespace NullSave
{

    public enum ReIconedActionLookup : byte
    {
        Name = 0,
        Id = 1
    }

    public enum ReIconedModifiers : byte
    {
        None = 0,
        Positive = 1,
        Negative = 2,
        Dual = 3,
        All = 4
    }

    public enum ReIconedUpdateType : byte
    {
        ActiveInput = 0,
        PreferController = 1,
        PreferKeyboard = 2
    }

}