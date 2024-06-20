#if UNITY_EDITOR
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;

namespace Unity.Serialization.Binary
{
    unsafe partial class BinaryAdapter : IBinaryAdapter
        , IBinaryAdapter<UnityEditor.GUID>
        , IBinaryAdapter<UnityEditor.GlobalObjectId>
    {
        void IBinaryAdapter<UnityEditor.GUID>.Serialize(in BinarySerializationContext<UnityEditor.GUID> context, UnityEditor.GUID value)
        {
            context.Writer->AddNBC(value.ToString());
        }

        UnityEditor.GUID IBinaryAdapter<UnityEditor.GUID>.Deserialize(in BinaryDeserializationContext<UnityEditor.GUID> context)
        {
            context.Reader->ReadNextNBC(out var str);
            return UnityEditor.GUID.TryParse(str, out var value) ? value : default;
        }

        void IBinaryAdapter<UnityEditor.GlobalObjectId>.Serialize(in BinarySerializationContext<UnityEditor.GlobalObjectId> context, UnityEditor.GlobalObjectId value)
        {
            context.Writer->AddNBC(value.ToString());
        }

        UnityEditor.GlobalObjectId IBinaryAdapter<UnityEditor.GlobalObjectId>.Deserialize(in BinaryDeserializationContext<UnityEditor.GlobalObjectId> context)
        {
            context.Reader->ReadNextNBC(out var str);
            return UnityEditor.GlobalObjectId.TryParse(str, out var value) ? value : default;
        }
    }
}
#endif