using System;

namespace Unity.Serialization.Json
{
    partial class JsonAdapter :
        IJsonAdapter<sbyte>,
        IJsonAdapter<short>,
        IJsonAdapter<int>,
        IJsonAdapter<long>,
        IJsonAdapter<byte>,
        IJsonAdapter<ushort>,
        IJsonAdapter<uint>,
        IJsonAdapter<ulong>,
        IJsonAdapter<float>,
        IJsonAdapter<double>,
        IJsonAdapter<bool>,
        IJsonAdapter<char>,
        IJsonAdapter<string>
    {
        void IJsonAdapter<sbyte>.Serialize(in JsonSerializationContext<sbyte> context, sbyte value) => context.Writer.WriteValue(value);
        void IJsonAdapter<short>.Serialize(in JsonSerializationContext<short> context, short value) => context.Writer.WriteValue(value);
        void IJsonAdapter<int>.Serialize(in JsonSerializationContext<int> context, int value) => context.Writer.WriteValue(value);
        void IJsonAdapter<long>.Serialize(in JsonSerializationContext<long> context, long value) => context.Writer.WriteValue(value);
        void IJsonAdapter<byte>.Serialize(in JsonSerializationContext<byte> context, byte value) => context.Writer.WriteValue(value);
        void IJsonAdapter<ushort>.Serialize(in JsonSerializationContext<ushort> context, ushort value) => context.Writer.WriteValue(value);
        void IJsonAdapter<uint>.Serialize(in JsonSerializationContext<uint> context, uint value) => context.Writer.WriteValue(value);
        void IJsonAdapter<ulong>.Serialize(in JsonSerializationContext<ulong> context, ulong value) => context.Writer.WriteValue(value);
        void IJsonAdapter<float>.Serialize(in JsonSerializationContext<float> context, float value) => context.Writer.WriteValue(value);
        void IJsonAdapter<double>.Serialize(in JsonSerializationContext<double> context, double value) => context.Writer.WriteValue(value);
        void IJsonAdapter<bool>.Serialize(in JsonSerializationContext<bool> context, bool value) => context.Writer.WriteValueLiteral(value ? "true" : "false");
        void IJsonAdapter<char>.Serialize(in JsonSerializationContext<char> context, char value) => context.Writer.WriteValue((int) value);
        void IJsonAdapter<string>.Serialize(in JsonSerializationContext<string> context, string value) => context.Writer.WriteValue(value);
        
        sbyte IJsonAdapter<sbyte>.Deserialize(in JsonDeserializationContext<sbyte> context) 
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        short IJsonAdapter<short>.Deserialize(in JsonDeserializationContext<short> context) 
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        int IJsonAdapter<int>.Deserialize(in JsonDeserializationContext<int> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");
        
        long IJsonAdapter<long>.Deserialize(in JsonDeserializationContext<long> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        byte IJsonAdapter<byte>.Deserialize(in JsonDeserializationContext<byte> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        ushort IJsonAdapter<ushort>.Deserialize(in JsonDeserializationContext<ushort> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");
        
        uint IJsonAdapter<uint>.Deserialize(in JsonDeserializationContext<uint> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");
        
        ulong IJsonAdapter<ulong>.Deserialize(in JsonDeserializationContext<ulong> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");
        
        float IJsonAdapter<float>.Deserialize(in JsonDeserializationContext<float> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        double IJsonAdapter<double>.Deserialize(in JsonDeserializationContext<double> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        bool IJsonAdapter<bool>.Deserialize(in JsonDeserializationContext<bool> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");

        char IJsonAdapter<char>.Deserialize(in JsonDeserializationContext<char> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");
        
        string IJsonAdapter<string>.Deserialize(in JsonDeserializationContext<string> context)
            => throw new NotImplementedException($"This code should never be executed. {nameof(JsonPropertyReader)} should handle primitives in a specialized way.");
    }
}
