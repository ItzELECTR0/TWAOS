using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarMarker : TToolbarButton
    {
        public const string ID = "Game Creator/Marker";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Marker";
        protected override string Tooltip => "Create a navigation Marker";
        protected override IIcon CreateIcon => new IconMarker(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            MarkerEditor.CreateElement(null);
        }
    }
}
