using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters.Animim;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(AnimimGraph))]
    public class AnimimGraphDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property)
        {
            return "States & Gestures";
        }
    }
}