using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Enable Component")]
    [Description("Enables a component class from the game object")]

    [Category("Game Objects/Components/Enable Component")]

    [Keywords("Active", "Turn", "On", "MonoBehaviour", "Behaviour", "Script")]
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectEnableComponent : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TypeReferenceBehaviour m_Type = new TypeReferenceBehaviour();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Enable {this.m_Type} from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            Behaviour behaviour = gameObject.Get(this.m_Type.Type) as Behaviour;
            if (behaviour != null) behaviour.enabled = true;
            
            return DefaultResult;
        }
    }
}