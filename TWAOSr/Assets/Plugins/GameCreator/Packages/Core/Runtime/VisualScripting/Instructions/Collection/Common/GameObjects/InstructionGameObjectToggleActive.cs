using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Toggle Active")]
    [Description("Toggles the state of a game object to active or to inactive")]

    [Category("Game Objects/Toggle Active")]

    [Keywords("Activate", "Deactivate", "Enable", "Disable", "Switch", "Swap")]
    [Keywords("MonoBehaviour", "Behaviour", "Script")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectToggleActive : TInstructionGameObject
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Toggle Active {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            bool value = gameObject.activeSelf;
            gameObject.SetActive(!value);
            
            return DefaultResult;
        }
    }
}