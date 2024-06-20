using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Serialization.Binary.Tests
{
    partial class BinarySerializationTests
    {
        [TestFixture]
        class Adapters
        {
            class TestObject : ScriptableObject
            {
    
            }

            class UnityObjectAdapter : IContravariantBinaryAdapter<UnityEngine.Object>
            {
                public int SerializeCount;
                public int DeserializeCount;
                
                public void Serialize(IBinarySerializationContext context, Object value)
                {
                    SerializeCount++;
                }

                public object Deserialize(IBinaryDeserializationContext context)
                {
                    DeserializeCount++;
                    return null;
                }
            }

            [Test]
            public unsafe void SerializeAndDeserialize_PolymorphicUnityEngineObject_WithUserDefinedAdapter()
            {
                var container = ScriptableObject.CreateInstance<TestObject>();
                var adapter = new UnityObjectAdapter();

                try
                {
                    var parameters = new BinarySerializationParameters
                    {
                        UserDefinedAdapters = new List<IBinaryAdapter> { adapter }
                    };

                    using (var stream = new UnsafeAppendBuffer(16, 8, Allocator.Temp))
                    {
                        BinarySerialization.ToBinary<object>(&stream, container, parameters);
                        
                        var reader = stream.AsReader();
                        BinarySerialization.FromBinary<object>(&reader, parameters);
                    }
                    
                    Assert.AreEqual(adapter.SerializeCount, 1);
                    Assert.AreEqual(adapter.DeserializeCount, 1);
                }
                finally
                {
                    Object.DestroyImmediate(container);
                }
            }
        }
    }
}