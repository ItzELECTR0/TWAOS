using GameCreator.Editor.Variables;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarLocalNameVariables : TToolbarButton
    {
        public const string ID = "Game Creator/Local Name Variables";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Local Name Variables";
        protected override string Tooltip => "Create a Local Name Variables";
        protected override IIcon CreateIcon => new IconNameVariable(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            LocalNameVariablesEditor.CreateLocalNameVariables(null);
        }
    }
}
