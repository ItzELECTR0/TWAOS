using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Take off Skin Mesh")]
    [Description("Removes an instance of a Skin Mesh from a Character")]

    [Category("Characters/Visuals/Take off Skin Mesh")]
    
    [Parameter("Prefab", "Game Object reference with a Skin Mesh Renderer that is removed")]
    [Parameter("From Character", "Target Character that uses its armature to wear the skin mesh")]
    
    [Image(typeof(IconSkinMesh), ColorTheme.Type.TextLight, typeof(OverlayArrowRight))]
    
    [Keywords("Renderer", "Game Object", "Armature")]
    [Serializable]
    public class InstructionCharacterTakeOffSkinMesh : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Prefab = GetGameObjectInstance.Create();
        
        [SerializeField] private PropertyGetGameObject m_FromCharacter = GetGameObjectPlayer.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Take {this.m_Prefab} off {this.m_FromCharacter}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Character character = this.m_FromCharacter.Get<Character>(args);
            GameObject prefab = this.m_Prefab.Get(args);

            if (character == null) return DefaultResult;
            if (prefab == null) return DefaultResult;

            character.Props.RemoveSkinMesh(prefab);
            return DefaultResult;
        }
    }
}