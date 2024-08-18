using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class PadBox : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/PadBox";

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PadBox()
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);
        }
    }
}
