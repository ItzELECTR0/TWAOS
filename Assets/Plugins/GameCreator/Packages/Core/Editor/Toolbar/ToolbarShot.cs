using GameCreator.Editor.Cameras;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarShot : TToolbarButton
    {
        public const string ID = "Game Creator/Camera Shot";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Camera Shot";
        protected override string Tooltip => "Create a Camera Shot";
        protected override IIcon CreateIcon => new IconCameraShot(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            ShotCameraEditor.CreateElement(null);
        }
    }
}
