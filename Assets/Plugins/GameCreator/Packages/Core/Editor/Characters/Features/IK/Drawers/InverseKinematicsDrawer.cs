using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(InverseKinematics))]
    public class InverseKinematicsDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property) => "Inverse Kinematics";
    }
}