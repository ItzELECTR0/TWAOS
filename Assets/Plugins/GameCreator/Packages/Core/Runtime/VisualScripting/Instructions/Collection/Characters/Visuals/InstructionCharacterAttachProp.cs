using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Attach Prop")]
    [Description("Attaches a prefab or instance Prop onto a Character's bone")]

    [Category("Characters/Visuals/Attach Prop")]

    [Parameter("Character", "The character target")]
    [Parameter("Type", "Whether to attach the prop as a prefab or instance")]
    [Parameter("Prop", "The prefab or instance object that is attached to the character")]
    [Parameter("Bone", "Which bone the prop is attached to")]
    [Parameter("Position", "Local offset from which the prop is distanced from the bone")]
    [Parameter("Rotation", "Local offset from which the prop is rotated from the bone")]

    [Keywords("Characters", "Add", "Grab", "Draw", "Pull", "Take", "Object")]
    [Image(typeof(IconTennis), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterAttachProp : Instruction
    {
        private enum Type
        {
            Prefab,
            Instance
        }

        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private Type m_Type = Type.Prefab;
        [SerializeField] private PropertyGetGameObject m_Prop = new PropertyGetGameObject();

        [SerializeField] private HandleField m_Handle = new HandleField();

        public override string Title => $"Attach {this.m_Type} {this.m_Prop} on {this.m_Character} {this.m_Handle}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject prop = this.m_Prop.Get(args);
            if (prop == null) return DefaultResult;

            Args handleArgs = new Args(character.gameObject);
            HandleResult handle = this.m_Handle.Get(handleArgs);
            
            switch (this.m_Type)
            {
                case Type.Prefab:
                    character.Props.AttachPrefab(
                        handle.Bone, prop,
                        handle.LocalPosition, handle.LocalRotation
                    );
                    break;
                
                case Type.Instance:
                    character.Props.AttachInstance(
                        handle.Bone, prop,
                        handle.LocalPosition, handle.LocalRotation
                    );
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }

            return DefaultResult;
        }
    }
}