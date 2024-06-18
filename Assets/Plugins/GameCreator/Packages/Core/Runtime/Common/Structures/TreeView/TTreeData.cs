using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TTreeData<TValue> : TSerializableDictionary<int, TTreeDataItem<TValue>> 
        where TValue : class
    { }
}