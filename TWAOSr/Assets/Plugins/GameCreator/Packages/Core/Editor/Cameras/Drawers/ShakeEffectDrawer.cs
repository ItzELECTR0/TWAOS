using GameCreator.Editor.Common;
using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShakeEffect))]
    public class ShakeEffectDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property)
        {
            return "Shake Effect";
        }
    }
}