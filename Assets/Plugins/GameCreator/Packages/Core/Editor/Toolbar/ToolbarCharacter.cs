using GameCreator.Editor.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Toolbars;

namespace GameCreator.Editor.Overlays
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    internal sealed class ToolbarCharacter : TToolbarButton
    {
        public const string ID = "Game Creator/Characters";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Text => "Character";
        protected override string Tooltip => "Create a Character";
        protected override IIcon CreateIcon => new IconCharacter(COLOR);
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override void Run()
        {
            CharacterEditor.CreateCharacter(null);
        }
    }
}
