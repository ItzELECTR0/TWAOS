using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Drop Prop")]
    [Description("Drops a prefab or instance Prop (if any) from a Character")]

    [Category("Characters/Visuals/Drop Prop")]

    [Parameter("Character", "The character target")]
    [Parameter("Type", "Whether to drop the prop form a prefab or as its instance")]
    [Parameter("Prop", "The prefab or instance object prop that is dropped from the character")]

    [Keywords("Characters", "Detach", "Let", "Sheathe", "Put", "Holster", "Object")]
    [Image(typeof(IconTennis), ColorTheme.Type.TextLight, typeof(OverlayArrowDown))]

    [Serializable]
    public class InstructionCharacterDropProp : Instruction
    {
        private enum Type
        {
            Prefab,
            Instance
        }
        
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private Type m_Type = Type.Prefab;
        [SerializeField] private PropertyGetGameObject m_Prop = new PropertyGetGameObject();

        public override string Title => $"Drop {this.m_Type} {this.m_Prop} from {this.m_Character}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject prop = this.m_Prop.Get(args);
            if (prop == null) return DefaultResult;

            switch (this.m_Type)
            {
                case Type.Prefab:
                    character.Props.DropPrefab(prop);
                    break;
                
                case Type.Instance:
                    character.Props.DropInstance(prop);
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
            
            return DefaultResult;
        }
    }
}