using GameCreator.Editor.Common;
using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemThirdPerson))]
    public class ShotSystemThirdPersonDrawer : TShotSystemDrawer
    {
        protected override string Name(SerializedProperty property) => "Third Person";
    }
    
    [CustomPropertyDrawer(typeof(ShotSystemThirdPerson.Align))]
    public class ShotSystemThirdPersonAlignDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Align";
    }
}