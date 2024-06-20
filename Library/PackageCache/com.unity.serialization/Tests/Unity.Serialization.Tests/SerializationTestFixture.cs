using JetBrains.Annotations;
using NUnit.Framework;

namespace Unity.Serialization.Tests
{
    struct CommonSerializationParameters
    {
        public bool DisableSerializedReferences { get; set; }
    }
    
    [TestFixture]
    abstract partial class SerializationTestFixture
    {
        protected abstract bool SupportsPolymorphicUnityObjectReferences { get; }
        
        protected abstract T SerializeAndDeserialize<T>([CanBeNull] T value, CommonSerializationParameters parameters = default);
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
#if !UNITY_2022_1_OR_NEWER 
            Properties.AOT.PropertyGenerator<PropertyWrapper<StructWithPrimitives?>, StructWithPrimitives?>.Preserve();
#endif
            Properties.PropertyBag.RegisterList<int>();
            Properties.PropertyBag.RegisterList<string>();
            Properties.PropertyBag.RegisterList<System.Collections.Generic.List<int>>();
            Properties.PropertyBag.RegisterList<System.Collections.Generic.List<object>>();
            Properties.PropertyBag.RegisterHashSet<int>();
        }
    }
}