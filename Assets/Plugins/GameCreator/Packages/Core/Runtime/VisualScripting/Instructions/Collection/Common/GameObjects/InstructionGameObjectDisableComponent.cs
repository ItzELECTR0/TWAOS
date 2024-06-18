using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Disable Component")]
    [Description("Disables a component class from the game object")]

    [Category("Game Objects/Components/Disable Component")]

    [Keywords("Deactivate", "Turn", "Off", "MonoBehaviour", "Behaviour", "Script")]
    [Image(typeof(IconComponent), ColorTheme.Type.Red)]
    
    [Serializable]
    public class InstructionGameObjectDisableComponent : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TypeReferenceBehaviour m_Type = new TypeReferenceBehaviour();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Disable {this.m_Type} from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            Behaviour behaviour = gameObject.Get(this.m_Type.Type) as Behaviour;
            if (behaviour != null) behaviour.enabled = false;
            
            return DefaultResult;
        }
    }
}