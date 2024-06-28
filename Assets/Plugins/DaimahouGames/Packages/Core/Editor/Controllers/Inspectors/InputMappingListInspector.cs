using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Core;
using UnityEditor;

namespace DaimahouGames.Editor.Core
{
    public class InputMappingListInspector : ListInspector<InputMapping.Mapping>
    {
        public InputMappingListInspector(SerializedProperty property) : base(property) {}

        protected override GenericInspector MakeItemInspector(int index)
        {
            return new InputMappingInspector(this, index);
        }
    }
}