using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Name")]
    [Description("Changes the name of a game object")]

    [Parameter(
        "Name", 
        "The new name assigned to the game object"
    )]

    [Category("Game Objects/Change Name")]
    [Keywords("MonoBehaviour", "Behaviour", "Script")]
    
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectName : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetString m_Name = GetStringString.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Name of {this.m_GameObject} = {this.m_Name}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            gameObject.name = this.m_Name.Get(args);
            return DefaultResult;
        }
    }
}