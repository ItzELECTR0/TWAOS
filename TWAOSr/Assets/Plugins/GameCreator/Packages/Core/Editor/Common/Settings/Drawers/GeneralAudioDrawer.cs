using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(GeneralAudio))]
    public class GeneralAudioDrawer : TTitleDrawer
    {
        protected override string Title => "Audio";
    }
}