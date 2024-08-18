using GameCreator.Editor.VisualScripting;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarHotspot : TToolbarButton
    {
        public const string ID = "Game Creator/Hotspot";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Hotspot";
        protected override string Tooltip => "Create a Hotspot game object";
        protected override IIcon CreateIcon => new IconHotspot(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            HotspotEditor.CreateElement(null);
        }
    }
}
