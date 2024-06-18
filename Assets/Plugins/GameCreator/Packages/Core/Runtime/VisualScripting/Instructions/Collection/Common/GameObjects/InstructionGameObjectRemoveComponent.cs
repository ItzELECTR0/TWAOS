using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Remove Component")]
    [Description("Removes an existing component from the game object")]

    [Category("Game Objects/Components/Remove Component")]

    [Keywords("Delete", "Destroy", "MonoBehaviour", "Behaviour", "Script")]
    [Image(typeof(IconComponent), ColorTheme.Type.Red, typeof(OverlayMinus))]
    
    [Serializable]
    public class InstructionGameObjectRemoveComponent : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TypeReferenceComponent m_Type = new TypeReferenceComponent();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Remove {this.m_Type} from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;
            
            Component component = gameObject.Get(this.m_Type.Type);
            if (component != null) UnityEngine.Object.Destroy(component);
            
            return DefaultResult;
        }
    }
}