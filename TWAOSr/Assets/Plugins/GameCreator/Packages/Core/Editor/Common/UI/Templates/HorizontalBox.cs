using System;
using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class HorizontalBox : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/HorizontalBox";
        
        public enum FlexMode
        {
            None,
            FirstGrows,
            SecondGrows
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public HorizontalBox(FlexMode flexMode = FlexMode.None, params VisualElement[] children)
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);

            foreach (VisualElement child in children)
            {
                this.Add(child);
            }

            switch (flexMode)
            {
                case FlexMode.None: 
                    break;
                
                case FlexMode.FirstGrows:
                    if (this.childCount >= 1) this[0].style.flexGrow = 1;
                    break;
                
                case FlexMode.SecondGrows:
                    if (this.childCount >= 2) this[1].style.flexGrow = 1;
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(flexMode), flexMode, null);
            }
        }
    }
}