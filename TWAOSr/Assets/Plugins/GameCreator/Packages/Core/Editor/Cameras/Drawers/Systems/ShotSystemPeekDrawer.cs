using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemPeek))]
    public class ShotSystemPeekDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Peek";
    }
}