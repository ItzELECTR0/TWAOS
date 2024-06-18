using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemFirstPerson))]
    public class ShotSystemFirstPersonDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "First Person";
    }
}