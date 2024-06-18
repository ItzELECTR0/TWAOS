using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class FlexibleSpace : VisualElement
    {
        public FlexibleSpace()
        {
            this.style.flexGrow = 1;
        }
    }
}
