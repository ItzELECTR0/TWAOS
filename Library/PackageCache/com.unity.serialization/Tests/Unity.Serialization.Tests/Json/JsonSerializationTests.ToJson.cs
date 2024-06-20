using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Unity.Properties;
using Unity.Serialization.Tests;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    [TestFixture]
    partial class JsonSerializationTests
    {
        MemoryStream m_Stream;
        SerializedObjectReaderConfiguration m_ConfigWithNoValidation;

        [SetUp]
        public void SetUp()
        {
            m_Stream = new MemoryStream();
            m_ConfigWithNoValidation = SerializedObjectReaderConfiguration.Default;
            m_ConfigWithNoValidation.ValidationType = JsonValidationType.None;
        }

        [TearDown]
        public void TearDown()
        {
            m_Stream.Dispose();
            m_Stream = null;
        }

        SerializedObjectReader CreateReader(string json)
        {
            m_Stream.Seek(0, SeekOrigin.Begin);

            using (var writer = new StreamWriter(m_Stream, Encoding.UTF8, 1024, true))
            {
                writer.Write(json);
            }

            m_Stream.Seek(0, SeekOrigin.Begin);

            return new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation);
        }
        
        [GeneratePropertyBag]
        internal struct TestStruct
        {
            public int A;
            public int B;
        }

        interface ITestInterface
        {
            
        }

        [GeneratePropertyBag]
        internal class TestConcreteA : ITestInterface
        {
            public int A;
        }

        [GeneratePropertyBag]
        internal class TestConcreteB : ITestInterface
        {
            public int B;
        }

        [GeneratePropertyBag]
        internal class ClassWithObjectField
        {
            public object Value;
        }

        [Test]
        public void ToJson_Null_ReturnsAStringThatSaysNull()
        {
            var json = JsonSerialization.ToJson<object>(default);
            Assert.That(json, Is.EqualTo("null"));
        }

        [Test]
        public void ToJson_StructWithPrimitives_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new TestStruct {A = 10, B = 32});
            Assert.That(UnFormat(json), Is.EqualTo(@"{""A"":10,""B"":32}"));
        }

        [Test]
        public void ToJson_ArrayInt_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new[] {1, 2, 3});
            Assert.That(UnFormat(json), Is.EqualTo(@"[1,2,3]"));
        }

        [Test]
        public void ToJson_ListInt_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new List<int> {1, 2, 3});
            Assert.That(UnFormat(json), Is.EqualTo(@"[1,2,3]"));
        }

        [Test]
        public void ToJson_HashSetInt_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new HashSet<int> {1, 2, 3});
            Assert.That(UnFormat(json), Is.EqualTo(@"[1,2,3]"));
        }

        [Test]
        public void ToJson_Interface_ReturnsValidJsonStringWithTypeInfo()
        {
            var json = JsonSerialization.ToJson<ITestInterface>(new TestConcreteA { A = 42 });
            Debug.Log(UnFormat(json));
            Assert.That(UnFormat(json), Is.EqualTo(@"{""$type"":""Unity.Serialization.Json.Tests.JsonSerializationTests+TestConcreteA, Unity.Serialization.Tests"",""A"":42}"));
        }
        
        [Test]
        public void ToJson_ListInterface_ReturnsValidJsonStringWithTypeInfo()
        {
            PropertyBag.RegisterList<ITestInterface>();
            
            var json = JsonSerialization.ToJson(new List<ITestInterface>
            {
                new TestConcreteA { A = 5 },
                new TestConcreteB { B = 6 }
            });
            Assert.That(UnFormat(json), Is.EqualTo(@"[{""$type"":""Unity.Serialization.Json.Tests.JsonSerializationTests+TestConcreteA, Unity.Serialization.Tests"",""A"":5},{""$type"":""Unity.Serialization.Json.Tests.JsonSerializationTests+TestConcreteB, Unity.Serialization.Tests"",""B"":6}]"));
        }

        [Test]
        public void ToJson_ObjectWithBoolValue_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new ClassWithObjectField { Value = true });
            Debug.Log(UnFormat(json));
            Assert.That(UnFormat(json), Is.EqualTo(@"{""Value"":true}"));
        }

        [Test]
        public void ToJson_SerializedObjectView_WithPrimitiveValues_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new ClassWithPrimitives
            {
                Int32Value = 55
            });

            using (var reader = CreateReader(json))
            {
                var view = reader.ReadObject();
                Assert.That(json, Is.EqualTo(JsonSerialization.ToJson(view)));
                Assert.That("55", Is.EqualTo(JsonSerialization.ToJson(view["Int32Value"])));
            }
        }

        [Test]
        public void ToJson_SerializedObjectView_WithNestedObject_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new ClassWithNestedClass
            {
                Container = new ClassWithPrimitives
                {
                    Float64Value = 1.2345
                }
            });

            using (var reader = CreateReader(json))
            {
                var view = reader.ReadObject();
                Assert.That(json, Is.EqualTo(JsonSerialization.ToJson(view)));
                Assert.That("1.2345", Is.EqualTo(UnFormat(JsonSerialization.ToJson(view["Container"]["Float64Value"]))));
            }
        }

        [Test]
        public void ToJson_SerializedObjectView_WithNestedArray_ReturnsValidJsonString()
        {
            var json = JsonSerialization.ToJson(new ClassWithLists
            {
                Int32List = new List<int> { 1,2,3,4,5 }
            });

            using (var reader = CreateReader(json))
            {
                var view = reader.ReadObject();
                Assert.That(json, Is.EqualTo(JsonSerialization.ToJson(view)));
                Assert.That("[1,2,3,4,5]", Is.EqualTo(UnFormat(JsonSerialization.ToJson(view["Int32List"]))));
            }
        }

        [Test]
        public void ToJson_ClassWithObjectField_WithCustomIndent_ReturnsCorrectlyFormattedString()
        {
            var json = JsonSerialization.ToJson(new ClassWithObjectField { Value = new TestStruct {A = 10, B = 32} }, new JsonSerializationParameters {Minified = false, Indent = 2});
            Assert.That(json, Is.EqualTo(
@"{
  ""Value"": {
    ""$type"": ""Unity.Serialization.Json.Tests.JsonSerializationTests+TestStruct, Unity.Serialization.Tests"",
    ""A"": 10,
    ""B"": 32
  }
}".Replace("\r\n", "\n")));
        }
        
        [Test]
        [TestCase("a\\")]
        [TestCase("b\\\"")]
        [TestCase("c \\ \"def\"")]
        public void ToJson_StringEscapeHandlingTrue_AddsEscapeCharacters(string content)
        {
            var container = new TestClassWithPrimitives { C = content };
            var json = JsonSerialization.ToJson(container, new JsonSerializationParameters { StringEscapeHandling = true });
            var escaped = content.Replace("\\", "\\\\").Replace("\"", "\\\"");
            Assert.That(UnFormat(json), Is.EqualTo($"{{\"A\":0,\"B\":0,\"C\":\"{escaped}\"}}"));
        }
        
        [Test]
        [TestCase("hello\\\\")]
        [TestCase("hello\\\\\\\\")]
        public void ToJson_StringEscapeHandlingFalse_DoesNotAddEscapeCharacters(string content)
        {
            var container = new TestClassWithPrimitives { C = content };
            var json = JsonSerialization.ToJson(container, new JsonSerializationParameters { StringEscapeHandling = false });
            Assert.That(UnFormat(json), Is.EqualTo($"{{\"A\":0,\"B\":0,\"C\":\"{content}\"}}"));
        }
    }
}