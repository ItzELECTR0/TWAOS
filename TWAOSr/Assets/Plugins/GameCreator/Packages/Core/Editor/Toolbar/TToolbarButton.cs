using GameCreator.Runtime.Common;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    internal abstract class TToolbarButton : EditorToolbarButton
    {
        protected const ColorTheme.Type COLOR = ColorTheme.Type.TextLight;

        // MEMBERS: -------------------------------------------------------------------------------
        
        private IIcon m_Icon;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        private IIcon Icon => this.m_Icon ??= this.CreateIcon;

        protected abstract string Text { get; }
        protected abstract string Tooltip { get; }
        
        protected abstract IIcon CreateIcon { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected TToolbarButton() => this.Setup();

        private void Setup()
        {
            this.text = this.Text;
            this.icon = this.Icon.Texture;
            this.tooltip = this.Tooltip;
            this.clicked += Run;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        protected abstract void Run();
    }
}