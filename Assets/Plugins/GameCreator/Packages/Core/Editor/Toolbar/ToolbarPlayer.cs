using GameCreator.Editor.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarPlayer : TToolbarButton
    {
        public const string ID = "Game Creator/Player";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Player";
        protected override string Tooltip => "Create a Player";
        protected override IIcon CreateIcon => new IconPlayer(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            CharacterEditor.CreatePlayer(null);
        }
    }
}
