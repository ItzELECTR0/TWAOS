using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class LabelTitle : Label
    {
        public LabelTitle() : base()
        {
            this.style.unityFontStyleAndWeight = FontStyle.Bold;
            this.style.marginTop = new StyleLength(3);
            this.style.marginBottom = new StyleLength(3);
        }

        public LabelTitle(string text) : this()
        {
            this.text = text;
        }
    }
}