using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Put on Skin Mesh")]
    [Description("Creates a new instance of a skin mesh renderer and puts it on a Character")]

    [Category("Characters/Visuals/Put on Skin Mesh")]
    
    [Parameter("Prefab", "Game Object reference with a Skin Mesh Renderer that is instantiated")]
    [Parameter("On Character", "Target Character that uses its armature to wear the skin mesh")]
    
    [Image(typeof(IconSkinMesh), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    
    [Keywords("Renderer", "New", "Game Object", "Armature")]
    [Serializable]
    public class InstructionCharacterPutOnSkinMesh : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Prefab = GetGameObjectInstance.Create();
        
        [SerializeField] private PropertyGetGameObject m_OnCharacter = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Put {this.m_Prefab} on {this.m_OnCharacter}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Character character = this.m_OnCharacter.Get<Character>(args);
            GameObject prefab = this.m_Prefab.Get(args);

            if (character == null) return DefaultResult;
            if (prefab == null) return DefaultResult;

            character.Props.AttachSkinMesh(prefab);
            return DefaultResult;
        }
    }
}