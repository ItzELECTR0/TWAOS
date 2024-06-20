namespace Unity.Serialization.Json
{
    partial class JsonAdapter : IJsonAdapter<SerializedValueView>
    {
        void IJsonAdapter<SerializedValueView>.Serialize(in JsonSerializationContext<SerializedValueView> context, SerializedValueView value)
        {
            switch (value.Type)
            {
                case TokenType.String:
                {
                    context.SerializeValue(value.AsStringView().ToString());
                    break;
                }
                case TokenType.Object:
                {
                    context.SerializeValue(value.AsObjectView());
                    break;
                }
                case TokenType.Array:
                {
                    context.SerializeValue(value.AsArrayView());
                    break;
                }
                case TokenType.Primitive:
                {
                    var p = value.AsPrimitiveView();
                    
                    if (p.IsIntegral())
                    {
                        if (p.IsSigned())
                        {
                            context.SerializeValue(value.AsInt64());
                        }
                        else
                        {
                            context.SerializeValue(value.AsUInt64());
                        }
                    }
                    else if (p.IsDecimal() || p.IsInfinity() || p.IsNaN())
                    {
                        context.SerializeValue(value.AsDouble());
                    }
                    else if (p.IsBoolean())
                    {
                        context.SerializeValue(value.AsBoolean());
                    }
                    else
                    {
                        context.Writer.WriteNull();
                    }
                    break;
                }
                default:
                {
                    context.Writer.WriteNull();
                    break;
                }
            }
        }

        SerializedValueView IJsonAdapter<SerializedValueView>.Deserialize(in JsonDeserializationContext<SerializedValueView> context)
        {
            throw new System.InvalidOperationException();
        }
    }
}
