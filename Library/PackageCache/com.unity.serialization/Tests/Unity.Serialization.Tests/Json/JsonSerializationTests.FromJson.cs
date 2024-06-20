using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Unity.Collections;
using Unity.Properties;
using Unity.Serialization.Tests;
using UnityEngine;

#pragma warning disable 649

namespace Unity.Serialization.Json.Tests
{
    [TestFixture]
    partial class JsonSerializationTests
    {
        [GeneratePropertyBag]
        internal class TestClassWithPrimitives
        {
            public int A;
            public float B;
            public string C;
        }

        [GeneratePropertyBag]
        internal class TestClassWithArray
        {
            public int[] ArrayInt;
        }

        [GeneratePropertyBag]
        internal class TestClassWithArrayArray
        {
            public int[][] ArrayArrayInt;
        }

        [GeneratePropertyBag]
        internal class TestClassWithList
        {
            public List<int> ListInt;
        }

        [GeneratePropertyBag]
        internal class TestClassWithDictionary
        {
            public Dictionary<string, int> DictionaryStringInt;
            public Dictionary<string, object> DictionaryStringObject;
        }

        static readonly Dictionary<string, object> s_ExpectedTestResults = new Dictionary<string, object>
        {
            {"ArrayArrayInt0x0", new int[][] { }},
            {"ArrayArrayInt1x0", new[] {new int[0]}},
            {"ArrayArrayInt2x3", new[] {new[] {1, 2, 3}, new[] {4, 5, 6}}},
            {"ListInt0", new List<int>()},
            {"ListInt3", new List<int> {1, 2, 3}},
            {"DictionaryStringInt0", new Dictionary<string, int>()},
            {"DictionaryStringInt3", new Dictionary<string, int> {{"a", 1}, {"b", 2}, {"c", 3}}}
        };

        [Test]
        [TestCase(@"{""A"":10}", 10)]
        public void FromJson_ToClassWithPrimitive_ReturnsClassInstanceWithCorrectValues(string json, int expected)
        {
            var obj = JsonSerialization.FromJson<TestClassWithPrimitives>(json);
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<TestClassWithPrimitives>());
            Assert.That(obj.A, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"{""ArrayInt"":[]}", "")]
        [TestCase(@"{""ArrayInt"":[1,2,3]}",  "1,2,3")]
        public void FromJson_ToClassWithArray_ReturnsClassInstanceWithCorrectValues(string json, string expected)
        {
            var obj = JsonSerialization.FromJson<TestClassWithArray>(json);
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<TestClassWithArray>());
            Assert.That(obj.ArrayInt, Is.Not.Null);
            Assert.That(obj.ArrayInt, Is.EquivalentTo(expected.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse)));
        }

        [Test]
        [TestCase(@"{""ArrayArrayInt"":[]}", "ArrayArrayInt0x0")]
        [TestCase(@"{""ArrayArrayInt"":[[]]}", "ArrayArrayInt1x0")]
        [TestCase(@"{""ArrayArrayInt"":[[1,2,3],[4,5,6]]}", "ArrayArrayInt2x3")]
        public void FromJson_ToClassWithArrayOfArray_ReturnsClassInstanceWithCorrectValues(string json, string id)
        {
            var expected = s_ExpectedTestResults[id] as int[][];
            Assert.That(expected, Is.Not.Null);
            
            var obj = JsonSerialization.FromJson<TestClassWithArrayArray>(json);
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<TestClassWithArrayArray>());
            Assert.That(obj.ArrayArrayInt, Is.Not.Null);
            Assert.That(obj.ArrayArrayInt, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase(@"{""ListInt"":[]}", "ListInt0")]
        [TestCase(@"{""ListInt"":[1,2,3]}", "ListInt3")]
        public void FromJson_ToClassWithList_ReturnsClassInstanceWithCorrectValues(string json, string id)
        {
            var expected = s_ExpectedTestResults[id] as List<int>;
            Assert.That(expected, Is.Not.Null);

            var obj = JsonSerialization.FromJson<TestClassWithList>(json);
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<TestClassWithList>());
            Assert.That(obj.ListInt, Is.Not.Null);
            Assert.That(obj.ListInt, Is.EquivalentTo(expected));
        }
        
        [Test]
        [TestCase(@"{""DictionaryStringInt"":{}}", "DictionaryStringInt0")]
        [TestCase(@"{""DictionaryStringInt"":[]}", "DictionaryStringInt0")]
        [TestCase(@"{""DictionaryStringInt"":{""a"":1,""b"":2,""c"":3}}", "DictionaryStringInt3")]
        [TestCase(@"{""DictionaryStringInt"":[{""Key"":""a"",""Value"":1},{""Key"":""b"",""Value"":2},{""Key"":""c"",""Value"":3}]}", "DictionaryStringInt3")]
        public void FromJson_ToClassWithDictionary_ReturnsClassInstanceWithCorrectValues(string json, string id)
        {
            var expected = s_ExpectedTestResults[id] as Dictionary<string, int>;
            Assert.That(expected, Is.Not.Null);

            var obj = JsonSerialization.FromJson<TestClassWithDictionary>(json);
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<TestClassWithDictionary>());
            Assert.That(obj.DictionaryStringInt, Is.Not.Null);
            Assert.That(obj.DictionaryStringInt, Is.EquivalentTo(expected));
        }

        [Test]
        public void FromJson_StreamWithPartialTokens()
        {
            const string kJson = 
@"{
    ""Dependencies"": [
        ""GlobalObjectId_V1-1-4040109c1e5014e8abe57249dc3a759b-93214019566545601-0""
    ],
    ""Components"": [
        {
            ""$type"": ""NetCodeConversionSettings, Unity.NetCode"",
            ""Target"": 2
        }
    ]
}";
            using (var stream = new MemoryStream(UTF8Encoding.Default.GetBytes(kJson)))
            {
                var configuration = SerializedObjectReaderConfiguration.Default;

                configuration.UseReadAsync = stream.Length > 512 << 10;
                configuration.ValidationType = JsonValidationType.Standard;
                configuration.BlockBufferSize = (int) stream.Length; // 512 kb max
                configuration.OutputBufferSize = (int) stream.Length; // 1 mb max
                
                using (var reader = new SerializedObjectReader(stream, configuration))
                {
                    var obj = reader.ReadObject();
                    
                    Assert.That(obj.Count(), Is.EqualTo(2));
                }
            }
        }

        [Test]
        public void FromJson_ToIntList_WhenInputIsInvalid()
        {
            Assert.Throws<InvalidJsonException>(() =>
            {
                JsonSerialization.FromJson<List<int>>("[");
            });
            
            Assert.Throws<InvalidJsonException>(() =>
            {
                JsonSerialization.FromJson<Dictionary<string, int>>("{");
            });
        }
        
        [Test]
        public void FromJson_StreamWithExactBlockSize()
        {
            const string kJson = "{\"a\":12345678,\"b\":[";
            
            using (var stream = new MemoryStream(UTF8Encoding.Default.GetBytes(kJson)))
            {
                var configuration = SerializedObjectReaderConfiguration.Default;

                configuration.UseReadAsync = false;
                configuration.ValidationType = JsonValidationType.Standard;
                configuration.BlockBufferSize = (int) stream.Length * 2;
                configuration.OutputBufferSize = (int) stream.Length * 2;
                
                using (var reader = new SerializedObjectReader(stream, configuration))
                {
                    Assert.Throws<InvalidJsonException>(() =>
                    {
                        reader.ReadObject();
                    });
                }
            }
        }

        [Test]
        public void FromJson_ToClassWithPrimitves_WhenInputIsInvalid_ThrowsDetailedException()
        {
            var ex = Assert.Throws<InvalidJsonException>(() =>
            {
                JsonSerialization.FromJson<TestClassWithPrimitives>(@"{""A"": asd}");
            });
            
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Character, Is.EqualTo(7));
        }
        
        [Test]
        public void FromJson_ToClassWithPrimitves_WhenInputIsInvalidWithDisabledValidation_OnlyThrowsStructuralException()
        {
            Assert.DoesNotThrow(() =>
            {
                JsonSerialization.FromJson<TestClassWithPrimitives>(@"{""A"": asd}", new JsonSerializationParameters {DisableValidation = true});
            });
            
            var ex = Assert.Throws<InvalidJsonException>(() =>
            {
                JsonSerialization.FromJson<TestClassWithPrimitives>(@"{""A"": 1}}", new JsonSerializationParameters {DisableValidation = true});
            });
            
            Assert.That(ex.Message, Is.EqualTo("Input json was structurally invalid. Try with JsonValidationType=[Standard or Simple]"));
        }

        [Test]
        public void TryFromJson_ToTestClassWithPrimitives_WhenInputIsInvalid_DoesNotThrow()
        {
            var success = false;
            DeserializationResult result = default;
            
            Assert.DoesNotThrow(() =>
            {
                success = JsonSerialization.TryFromJson<TestClassWithPrimitives>(@"{""A"": asd}", out var container, out result);
            });
            
            Assert.That(success, Is.False);
            Assert.That(result.Exceptions.Count(), Is.EqualTo(1));
            Assert.That(result.Exceptions.First().Payload, Is.TypeOf<InvalidJsonException>());
        }
        
        [Test]
        [TestCase("a\\\\")]
        [TestCase("b\\\\\\\"")]
        [TestCase("c \\\\ \\\"def\\\"")]
        public void FromJson_StringEscapeHandlingTrue_RemovesEscapeCharacters(string content)
        {
            var json = "{\"C\": \"" + content + "\"}";
            var container = JsonSerialization.FromJson<TestClassWithPrimitives>(json, new JsonSerializationParameters { StringEscapeHandling = true });
            var unescaped = content.Replace("\\\"", "\"").Replace("\\\\", "\\");
            Assert.That(container.C, Is.EqualTo(unescaped));
        }

        [Test]
        [TestCase("hello\\\\")]
        [TestCase("hello\\\\\\\\")]
        public void FromJson_StringEscapeHandlingFalse_DoesRemoveEscapeCharacters(string content)
        {
            var json = "{\"C\": \"" + content + "\"}";
            var container = JsonSerialization.FromJson<TestClassWithPrimitives>(json, new JsonSerializationParameters { StringEscapeHandling = false });
            Assert.That(container.C, Is.EqualTo(content));
        }

        [Test]
        [TestCase('a')]
        [TestCase('0')]
        [TestCase('/')]
        [TestCase('\\')]
        [TestCase('\0')]
        [TestCase('\t')]
        [TestCase('\t')]
        [TestCase('\b')]
        [TestCase('\"')]
        [TestCase('\'')]
        public void FromJson_ClassWithCharValue_CanBeConvertedFromStringOrInt(char value)
        {
            ClassWithPrimitives dst;

            // Use the actual writer to apply escaping.
            using var writer = new JsonWriter(Allocator.Temp);
            writer.WriteValue(value);
            
            var jsonWithCharInString = $"{{\"CharValue\":{writer.ToString()}}}";
            dst = JsonSerialization.FromJson<ClassWithPrimitives>(jsonWithCharInString);
            Assert.That(dst.CharValue, Is.EqualTo(value));
            
            var jsonWithCharAsInt = $"{{\"CharValue\":{(int) value}}}";
            dst = JsonSerialization.FromJson<ClassWithPrimitives>(jsonWithCharAsInt);
            Assert.That(dst.CharValue, Is.EqualTo(value));
        }
    }
}