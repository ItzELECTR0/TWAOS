using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Active")]
    [Description("Changes the state of a game object to active or inactive")]

    [Category("Game Objects/Set Active")]

    [Keywords("Activate", "Deactivate", "Enable", "Disable")]
    [Keywords("MonoBehaviour", "Behaviour", "Script")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectSetActive : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetBool m_Active = GetBoolValue.Create(true);
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set Active {this.m_GameObject} to {this.m_Active}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            bool value = this.m_Active.Get(args);
            gameObject.SetActive(value);
            
            return DefaultResult;
        }
    }
}