using NUnit.Framework;
using Unity.Collections;

namespace Unity.Serialization.Json.Tests
{
    [TestFixture]
    class PackedBinaryWriterTests
    {
        [Test]
        [TestCase(@"{}")]
        [TestCase(@"{""foo"": {}, ""bar"": ""hello world""}")]
        public unsafe void PackedBinaryWriter_Write(string json)
        {
            using (var tokenStream = new JsonTokenStream(Allocator.TempJob))
            using (var tokenizer = new JsonTokenizer(Allocator.TempJob))
            using (var binaryStream = new PackedBinaryStream(Allocator.TempJob))
            using (var writer = new PackedBinaryWriter(tokenStream, binaryStream, Allocator.TempJob))
            {
                fixed (char* ptr = json)
                {
                    var buffer = new UnsafeBuffer<char>(ptr, json.Length);
                    tokenizer.Write(tokenStream, buffer, 0, json.Length);
                    writer.Write(buffer, tokenStream.TokenNextIndex);
                }
            }
        }

        [Test]
        [TestCase(@"{""t|e|st""|:""a|b|c""}")]
        public unsafe void PackedBinaryWriter_Write_PartialKey(string input)
        {
            var parts = input.Split('|');
            
            using (var tokenStream = new JsonTokenStream(Allocator.TempJob))
            using (var tokenizer = new JsonTokenizer(Allocator.TempJob))
            using (var binaryStream = new PackedBinaryStream(Allocator.TempJob))
            using (var writer = new PackedBinaryWriter(tokenStream, binaryStream, Allocator.TempJob))
            {
                foreach (var json in parts)
                {
                    fixed (char* ptr = json)
                    {
                        var buffer = new UnsafeBuffer<char>(ptr, json.Length);
                        tokenizer.Write(tokenStream, buffer, 0, json.Length);
                        writer.Write(buffer, tokenStream.TokenNextIndex);
                    }
                }

                binaryStream.DiscardCompleted();
            }
        }
    }
}
