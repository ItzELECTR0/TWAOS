using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Broadcast Message")]
    [Description("Invokes any method on any component found on the target game object")]

    [Category("Visual Scripting/Broadcast Message")]
    
    [Parameter("Game Object", "The target game object that receives the broadcast message")]
    [Parameter("Message", "The name of the method or methods that are called")]
    [Parameter("Send Upwards", "If true the message travels from the game object towards the root")]

    [Example(
        "By default all broadcast messages travel from the target game object and towards all " +
        "its children. Setting the Send Upwards field to true makes the message travel from the " +
        "game object towards the root parent"
    )]
    
    [Keywords("Execute", "Call", "Invoke", "Function")]
    [Image(typeof(IconMessage), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionLogicBroadcastMessage : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private PropertyGetString m_Message = new PropertyGetString();

        [SerializeField] private bool m_SendUpwards;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Broadcast {0} on {1} {2}", 
            this.m_Message, 
            this.m_GameObject,
            this.m_SendUpwards ? "upwards" : string.Empty
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;
            
            string message = this.m_Message.Get(args);
            switch (this.m_SendUpwards)
            {
                case true: 
                    gameObject.SendMessageUpwards(message, SendMessageOptions.DontRequireReceiver);
                    break;
                
                case false:
                    gameObject.SendMessage(message, SendMessageOptions.DontRequireReceiver);
                    break;
            }

            return DefaultResult;
        }
    }
}