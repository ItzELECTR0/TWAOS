using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Properties;
using Unity.Serialization.Tests;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    [TestFixture]
    partial class JsonAdapterTests
    {
        class DummyClass
        {
            
        }
        
        [GeneratePropertyBag]
        class ClassWithAdapters
        {
            public int A;
        }

        class DummyAdapter : IJsonAdapter<DummyClass>
        {
            public void Serialize(in JsonSerializationContext<DummyClass> context, DummyClass value)
            {
                
            }

            public DummyClass Deserialize(in JsonDeserializationContext<DummyClass> context)
            {
                return null;
            }
        }

        /// <summary>
        /// This class will intercept visitation for <see cref="ClassWithAdapters"/> types and write them out as a single integer value.
        /// </summary>
        class TestAdapter : IJsonAdapter<ClassWithAdapters>
        {
            void IJsonAdapter<ClassWithAdapters>.Serialize(in JsonSerializationContext<ClassWithAdapters> context, ClassWithAdapters value)
            {
                context.Writer.WriteValue(value.A);
            }

            ClassWithAdapters IJsonAdapter<ClassWithAdapters>.Deserialize(in JsonDeserializationContext<ClassWithAdapters> context)
            {
                return new ClassWithAdapters
                {
                    A = context.SerializedValue.AsInt32()
                };
            }
        }

        /// <summary>
        /// This class will intercept visitation for <see cref="ClassWithAdapters"/> types and write them out as a single integer value with an inverted sign.
        /// </summary>
        class TestInverter : IJsonAdapter<ClassWithAdapters>
        {
            void IJsonAdapter<ClassWithAdapters>.Serialize(in JsonSerializationContext<ClassWithAdapters> context, ClassWithAdapters value)
            {
                context.Writer.WriteValue(-value.A);
            }

            ClassWithAdapters IJsonAdapter<ClassWithAdapters>.Deserialize(in JsonDeserializationContext<ClassWithAdapters> context)
            {
                return new ClassWithAdapters
                {
                    A = -context.SerializedValue.AsInt32()
                };
            }
        }

        /// <summary>
        /// The <see cref="TestDecorator"/> shows an example of adding some additional structure to a type while letting the normal serialization flow happen.
        /// </summary>
        class TestDecorator : IJsonAdapter<ClassWithAdapters>
        {
            void IJsonAdapter<ClassWithAdapters>.Serialize(in JsonSerializationContext<ClassWithAdapters> context, ClassWithAdapters value)
            {
                using (context.Writer.WriteObjectScope())
                {
                    context.Writer.WriteKey("Decorated");
                    context.ContinueVisitation();
                }
            }

            ClassWithAdapters IJsonAdapter<ClassWithAdapters>.Deserialize(in JsonDeserializationContext<ClassWithAdapters> context)
            {
                return context.ContinueVisitation(context.SerializedValue["Decorated"]);
            }
        }

        /// <summary>
        /// The <see cref="TestGetInstanceAdapter"/> shows an example of accessing the existing instance when using override paths..
        /// </summary>
        class TestGetInstanceAdapter : IJsonAdapter<ClassWithAdapters>
        {
            public int ExpectedValue;
            
            void IJsonAdapter<ClassWithAdapters>.Serialize(in JsonSerializationContext<ClassWithAdapters> context, ClassWithAdapters value)
            {
                context.ContinueVisitation();
            }

            ClassWithAdapters IJsonAdapter<ClassWithAdapters>.Deserialize(in JsonDeserializationContext<ClassWithAdapters> context)
            {
                var instance = context.GetInstance();

                Assert.That(instance.A, Is.EqualTo(ExpectedValue));

                return instance;
            }
        }
        
        /// <summary>
        /// The <see cref="TestWriteValueAdapter"/> shows an example of writing out custom key-value pairs using the context API.
        /// </summary>
        class TestWriteValueAdapter : IJsonAdapter<ClassWithAdapters>
        {
            void IJsonAdapter<ClassWithAdapters>.Serialize(in JsonSerializationContext<ClassWithAdapters> context, ClassWithAdapters value)
            {
                using (context.Writer.WriteObjectScope())
                {
                    context.SerializeValue("a", 10);
                    context.SerializeValue("b", new[] {"hello", "world"});
                    context.SerializeValue("c", new Vector3(1, 2, 3));
                }
            }

            ClassWithAdapters IJsonAdapter<ClassWithAdapters>.Deserialize(in JsonDeserializationContext<ClassWithAdapters> context)
            {
                return context.GetInstance();
            }
        }
        
        [GeneratePropertyBag]
        class ClassWithAdaptedTypes
        {
            public ClassWithAdapters Value;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            PropertyBag.RegisterArray<string>();
        }

        [Test]
        public void SerializeAndDeserialize_WithNoUserDefinedAdapter_ValueIsSerializedNormally()
        {
            var src = new ClassWithAdaptedTypes
            {
                Value = new ClassWithAdapters {A = 42}
            };

            var json = JsonSerialization.ToJson(src);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":{""A"":42}}"));
            var dst = JsonSerialization.FromJson<ClassWithAdaptedTypes>(json);
            Assert.That(dst.Value.A, Is.EqualTo(src.Value.A));
        }

        [Test]
        public void SerializeAndDeserialize_WithUserDefinedAdapter_AdapterIsInvoked()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    new DummyAdapter(),
                    new TestAdapter()
                }
            };

            var src = new ClassWithAdaptedTypes
            {
                Value = new ClassWithAdapters {A = 42}
            };

            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":42}"));

            var dst = JsonSerialization.FromJson<ClassWithAdaptedTypes>(json, jsonSerializationParameters);

            Assert.That(dst.Value.A, Is.EqualTo(src.Value.A));
        }

        [Test]
        public void SerializeAndDeserialize_WithUserDefinedAdapter_AdapterCanContinue()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter> {new TestDecorator()}
            };

            var src = new ClassWithAdaptedTypes
            {
                Value = new ClassWithAdapters {A = 42}
            };

            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":{""Decorated"":{""A"":42}}}"));

            var dst = JsonSerialization.FromJson<ClassWithAdaptedTypes>(json, jsonSerializationParameters);

            Assert.That(dst.Value.A, Is.EqualTo(src.Value.A));
        }

        [Test]
        public void SerializeAndDeserialize_WithMultipleUserDefinedAdapters_AdaptersAreInvoked()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    // The order is important here.
                    new TestDecorator(),
                    new TestInverter()
                }
            };

            var src = new ClassWithAdaptedTypes
            {
                Value = new ClassWithAdapters {A = 42}
            };

            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":{""Decorated"":-42}}"));

            var dst = JsonSerialization.FromJson<ClassWithAdaptedTypes>(json, jsonSerializationParameters);

            Assert.That(dst.Value.A, Is.EqualTo(src.Value.A));
        }

        [Test]
        public void SerializeAndDeserialize_WithMultipleUserDefinedAdapters_OnlyTheFirstAdapterIsInvoked()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    // The order is important here.
                    new TestInverter(),
                    new TestAdapter(),
                    new TestDecorator()
                }
            };

            var src = new ClassWithAdaptedTypes
            {
                Value = new ClassWithAdapters {A = 42}
            };

            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":-42}"));

            var dst = JsonSerialization.FromJson<ClassWithAdaptedTypes>(json, jsonSerializationParameters);

            Assert.That(dst.Value.A, Is.EqualTo(src.Value.A));
        }

        [Test]
        public void SerializeAndDeserialize_WithUserDefinedAdapter_AdapterCanAccessExistingInstance()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    // The order is important here.
                    new TestGetInstanceAdapter { ExpectedValue = 42 }
                }
            };

            var dst = new ClassWithAdaptedTypes
            {
                Value = new ClassWithAdapters {A = 42}
            };

            JsonSerialization.FromJsonOverride(@"{""Value"":0}", ref dst, jsonSerializationParameters);

            Assert.That(dst.Value.A, Is.EqualTo(42));
        }
        
        [Test]
        public void SerializeAndDeserialize_WithUserDefinedAdapter_AdapterCanWriteKeyValuePairs()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    // The order is important here.
                    new TestWriteValueAdapter()
                }
            };

            var src = new ClassWithAdaptedTypes();
            
            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);

            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":{""a"":10,""b"":[""hello"",""world""],""c"":{""x"":1,""y"":2,""z"":3}}}"));
        }

        class ClassWithString
        {
            public string s;
        }

        class StringSuffixAdapter : IJsonAdapter<string>
        {
            public string Suffix;
            
            public void Serialize(in JsonSerializationContext<string> context, string value)
            {
                context.Writer.WriteValue(value + Suffix);
            }

            public string Deserialize(in JsonDeserializationContext<string> context)
            {
                var value = context.ContinueVisitation();
                return value[..^Suffix.Length];
            }
        }

        [Test]
        public void SerializeAndDeserialize_WithStringAdapter_AdapterIsCalled()
        {
            var jsonSerializationParameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    new StringSuffixAdapter { Suffix = " adapter"}
                }
            };

            var src = new ClassWithString
            {
                s = "hello"
            };

            var json = JsonSerialization.ToJson(src, jsonSerializationParameters);
            
            Assert.That(UnFormat(json), Is.EqualTo(@"{""s"":""hello adapter""}"));

            var dst = JsonSerialization.FromJson<ClassWithString>(json, jsonSerializationParameters);
            
            Assert.That(src.s, Is.EqualTo(dst.s));
        }

        static string UnFormat(string json)
        {
            return Regex.Replace(json, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
        }
    }
}