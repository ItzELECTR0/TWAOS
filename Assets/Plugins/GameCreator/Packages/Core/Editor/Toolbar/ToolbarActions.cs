using GameCreator.Editor.VisualScripting;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarActions : TToolbarButton
    {
        public const string ID = "Game Creator/Actions";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Actions";
        protected override string Tooltip => "Create an Actions game object";
        protected override IIcon CreateIcon => new IconInstructions(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            ActionsEditor.CreateElement(null);
        }
    }
}
