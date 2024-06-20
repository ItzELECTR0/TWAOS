using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;

namespace Unity.Serialization.Binary
{
    unsafe partial class BinaryAdapter :
        IContravariantBinaryAdapter<UnityEngine.Object>
    {
        void IContravariantBinaryAdapter<UnityEngine.Object>.Serialize(IBinarySerializationContext context, UnityEngine.Object value)
        {
#if UNITY_EDITOR
            var id = UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(value).ToString();
            context.Writer->AddNBC(id);
#endif
        }

        object IContravariantBinaryAdapter<UnityEngine.Object>.Deserialize(IBinaryDeserializationContext context)
        {
#if UNITY_EDITOR
            context.Reader->ReadNextNBC(out var value);

            if (UnityEditor.GlobalObjectId.TryParse(value, out var id))
            {
                return UnityEditor.GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            }
#endif
            return null;
        }
    }
}
