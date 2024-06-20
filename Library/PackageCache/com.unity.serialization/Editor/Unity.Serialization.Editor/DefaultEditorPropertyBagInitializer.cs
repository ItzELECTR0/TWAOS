using UnityEditor;

namespace Unity.Serialization.Editor
{
    static class DefaultEditorPropertyBags
    {
        [InitializeOnLoadMethod]
        internal static void Initialize()
        {
            DefaultPropertyBagInitializer.Initialize();
        }
    }
}