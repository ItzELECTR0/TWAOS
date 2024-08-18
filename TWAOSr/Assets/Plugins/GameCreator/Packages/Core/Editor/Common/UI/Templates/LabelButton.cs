using System;
using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class LabelButton : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/LabelButton";
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public LabelButton(string buttonText, Action callback) : this(" ", buttonText, callback)
        { }
        
        public LabelButton(string text, string buttonText, Action callback)
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);
            
            Label label = new Label(text);
            Button button = new Button(callback)
            {
                text = buttonText
            };

            this.Add(label);
            this.Add(button);

            AlignLabel.On(this);
        }
    }
}