using GameCreator.Editor.Variables;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarLocalListVariables : TToolbarButton
    {
        public const string ID = "Game Creator/Local List Variables";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Local List Variables";
        protected override string Tooltip => "Create a Local List Variables";
        protected override IIcon CreateIcon => new IconListVariable(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            LocalListVariablesEditor.CreateLocalListVariables(null);
        }
    }
}
