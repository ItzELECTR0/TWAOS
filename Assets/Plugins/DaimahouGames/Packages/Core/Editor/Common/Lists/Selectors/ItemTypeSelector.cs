using GameCreator.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class ItemTypeSelector<T> : TypeSelectorListFancy
    {
        public ItemTypeSelector(SerializedProperty propertyList, Button element) 
            : base(propertyList, typeof(T), element) {}
    }
}