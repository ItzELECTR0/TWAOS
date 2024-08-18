using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Component")]
    [Description("Adds a component class to the game object")]

    [Category("Game Objects/Components/Add Component")]

    [Keywords("Add", "Append", "MonoBehaviour", "Behaviour", "Script")]
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow, typeof(OverlayPlus))]
    
    [Serializable]
    public class InstructionGameObjectAddComponent : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TypeReferenceComponent m_Type = new TypeReferenceComponent();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Add {this.m_Type} to {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            gameObject.Add(this.m_Type.Type);
            return DefaultResult;
        }
    }
}