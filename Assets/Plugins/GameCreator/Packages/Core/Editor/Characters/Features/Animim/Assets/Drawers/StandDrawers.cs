using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(StandSingle))]
    public class StandSingleDrawers : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Locomotion: Single";
    }
    
    [CustomPropertyDrawer(typeof(Stand8Points))]
    public class Stand8PointsDrawers : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Locomotion: Circular 8 Points";
    }
    
    [CustomPropertyDrawer(typeof(Stand16Points))]
    public class Stand16PointsDrawers : TBoxDrawer
    {
        protected override string Name(SerializedProperty property)=> "Locomotion: Circular 16 Points";
    }
}