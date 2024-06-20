using System.IO;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;

namespace Unity.Serialization.Binary
{
    unsafe partial class BinaryAdapter :
        IBinaryAdapter<DirectoryInfo>,
        IBinaryAdapter<FileInfo>
    {
        void IBinaryAdapter<DirectoryInfo>.Serialize(in BinarySerializationContext<DirectoryInfo> context, DirectoryInfo value)
        {
            if (null == value)
                context.Writer->AddNBC("null");
            else
                context.Writer->AddNBC(value.GetRelativePath());
        }

        DirectoryInfo IBinaryAdapter<DirectoryInfo>.Deserialize(in BinaryDeserializationContext<DirectoryInfo> context)
        {
            context.Reader->ReadNextNBC(out var str);
            return str.Equals("null") ? null : new DirectoryInfo(str);
        }

        void IBinaryAdapter<FileInfo>.Serialize(in BinarySerializationContext<FileInfo> context, FileInfo value)
        {
            if (null == value)
                context.Writer->AddNBC("null");
            else
                context.Writer->AddNBC(value.GetRelativePath());
        }

        FileInfo IBinaryAdapter<FileInfo>.Deserialize(in BinaryDeserializationContext<FileInfo> context)
        {
            context.Reader->ReadNextNBC(out var str);
            return str.Equals("null") ? null : new FileInfo(str);
        }
    }
}
