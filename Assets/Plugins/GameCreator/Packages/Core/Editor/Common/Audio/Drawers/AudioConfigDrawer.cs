using GameCreator.Runtime.Common.Audio;
using UnityEditor;

namespace GameCreator.Editor.Common.Audio
{
    [CustomPropertyDrawer(typeof(TAudioConfig), true)]
    public class AudioConfigDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property)
        {
            return "Audio Config";
        }
    }
}
