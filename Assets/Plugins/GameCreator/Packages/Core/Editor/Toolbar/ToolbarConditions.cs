using GameCreator.Editor.VisualScripting;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarConditions : TToolbarButton
    {
        public const string ID = "Game Creator/Conditions";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Conditions";
        protected override string Tooltip => "Create a Conditions game object";
        protected override IIcon CreateIcon => new IconConditions(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            ConditionsEditor.CreateElement(null);
        }
    }
}
