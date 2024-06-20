using System.IO;

namespace Unity.Serialization.Json
{
    partial class JsonAdapter :
        IJsonAdapter<DirectoryInfo>,
        IJsonAdapter<FileInfo>
    {
        void IJsonAdapter<DirectoryInfo>.Serialize(in JsonSerializationContext<DirectoryInfo> context, DirectoryInfo value)
        {
            if (null == value) 
                context.Writer.WriteNull();
            else 
                context.Writer.WriteValue(value.GetRelativePath());
        }

        DirectoryInfo IJsonAdapter<DirectoryInfo>.Deserialize(in JsonDeserializationContext<DirectoryInfo> context)
        {
            return context.SerializedValue.AsStringView().Equals("null") ? null : new DirectoryInfo(context.SerializedValue.ToString());
        }

        void IJsonAdapter<FileInfo>.Serialize(in JsonSerializationContext<FileInfo> context, FileInfo value)
        {
            if (null == value) 
                context.Writer.WriteNull();
            else 
                context.Writer.WriteValue(value.GetRelativePath());
        }

        FileInfo IJsonAdapter<FileInfo>.Deserialize(in JsonDeserializationContext<FileInfo> context)
        {
            return context.SerializedValue.AsStringView().Equals("null") ? null : new FileInfo(context.SerializedValue.ToString());
        }
    }
}
