using System.Collections.Generic;
using NUnit.Framework;
using Unity.Properties;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonAdapterTests
    {
        [GeneratePropertyBag]
        class ClassWithObjectReferences
        {
            public object ObjectReference1;
            public Foo Foo;
            public object ObjectReference2;
        }

        [GeneratePropertyBag]
        class Foo
        {
            public int A;
            public object B;
        }

        [GeneratePropertyBag]
        class Dummy
        {
        }
        
        /// <summary>
        /// The <see cref="FooAdapter"/> shows an example of re-entrance and serialize a nested object as an inlined json string.
        /// </summary>
        class FooAdapter : IJsonAdapter<Foo>
        {
            public void Serialize(in JsonSerializationContext<Foo> context, Foo value)
            {
                context.Writer.WriteValue(JsonSerialization.ToJson(value).Replace('"', '\"').Replace("\n", "").Replace(" ", ""));
            }

            public Foo Deserialize(in JsonDeserializationContext<Foo> context)
            {
                return JsonSerialization.FromJson<Foo>(context.SerializedValue.AsStringView().ToString().Replace("\\\"", "\""));
            }
        }

        [Test]
        public void SerializeAndDeserialize_WithReEntranceAdapter_SerializedReferencesAreNotCleared()
        {
            var parameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    new FooAdapter()
                }
            };

            var reference = new Dummy();

            var src = new ClassWithObjectReferences
            {
                ObjectReference1 = reference,
                ObjectReference2 = reference,
                Foo = new Foo
                {
                    A = 42,
                
                    // Despite being the same reference, the adapter triggers a re-entrance and handles serialization in it's own tree.
                    B = reference
                }
            };

            var json = JsonSerialization.ToJson(src, parameters);
            
            var dst = JsonSerialization.FromJson<ClassWithObjectReferences>(json, parameters);

            Assert.That(dst, Is.Not.SameAs(src));
            Assert.That(dst.ObjectReference1, Is.SameAs(dst.ObjectReference2));
            Assert.That(dst.Foo.A, Is.EqualTo(42));
            
            // Despite being the same reference in src, the adapter triggers a re-entrance and handles which should create a new instance.
            Assert.That(dst.Foo.B, Is.Not.SameAs(dst.ObjectReference1));
        }
    }
}