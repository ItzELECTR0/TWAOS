using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class SpaceCustom : VisualElement
    {
        public SpaceCustom(int value)
        {
            this.style.height = new StyleLength(value);
        }
    }
}