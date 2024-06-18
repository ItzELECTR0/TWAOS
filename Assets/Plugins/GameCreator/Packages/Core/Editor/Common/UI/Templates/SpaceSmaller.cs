using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class SpaceSmaller : VisualElement
    {
        public SpaceSmaller()
        {
            this.style.height = new StyleLength(5);
        }
    }
}