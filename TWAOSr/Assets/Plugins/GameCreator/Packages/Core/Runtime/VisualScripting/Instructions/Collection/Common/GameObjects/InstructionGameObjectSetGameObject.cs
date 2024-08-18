using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Game Object")]
    [Description("Sets a game object value equal to another one")]

    [Category("Game Objects/Set Game Object")]

    [Parameter("Set", "Where the value is set")]
    [Parameter("From", "The value that is set")]

    [Keywords("Change", "Instance", "Variable", "Asset")]
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionGameObjectSetGameObject : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetGameObject m_Set = SetGameObjectNone.Create;
        
        [SerializeField]
        private PropertyGetGameObject m_From = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = {this.m_From}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject value = this.m_From.Get(args);
            this.m_Set.Set(value, args);

            return DefaultResult;
        }
    }
}