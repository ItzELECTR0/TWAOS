using GameCreator.Editor.VisualScripting;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarTrigger : TToolbarButton
    {
        public const string ID = "Game Creator/Trigger";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Trigger";
        protected override string Tooltip => "Create a Trigger game object";
        protected override IIcon CreateIcon => new IconTriggers(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            TriggerEditor.CreateElement(null);
        }
    }
}
