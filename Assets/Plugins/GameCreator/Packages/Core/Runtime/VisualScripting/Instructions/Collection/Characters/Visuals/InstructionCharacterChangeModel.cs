using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Model")]
    [Description("Changes the Character current model")]

    [Category("Characters/Visuals/Change Model")]

    [Parameter("Character", "The character target")]
    [Parameter("Model", "The prefab object that replaces the current Character model")]
    [Parameter("Skeleton", "Optional parameter that replaces the configuration of volumes")]
    [Parameter("Footstep Sounds", "Optional parameter that replaces the current Footstep sounds")]
    [Parameter("Offset", "A local offset from the center of the Character")]

    [Keywords("Characters", "Model")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterChangeModel : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        [Space]
        [SerializeField] private PropertyGetGameObject m_Model = new PropertyGetGameObject();
        
        [SerializeField] private MaterialSoundsAsset m_MaterialSounds;
        [SerializeField] private Vector3 m_Offset = Vector3.zero;

        public override string Title => $"Change Model on {this.m_Character} to {this.m_Model}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject model = this.m_Model.Get(args);
            if (model == null) return DefaultResult;

            character.ChangeModel(model, new Character.ChangeOptions
            {
                materials = this.m_MaterialSounds,
                offset = this.m_Offset
            });
            
            return DefaultResult;
        }
    }
}