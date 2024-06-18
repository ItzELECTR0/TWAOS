using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Layer")]
    [Description("Changes the layer value of a game object")]

    [Parameter(
        "Layer", 
        "The layer where the game object belongs to"
    )]
    
    [Parameter(
        "Children Too", 
        "Whether to also change the layer of the game object's children or not"
    )]
    
    [Category("Game Objects/Change Layer")]
    [Keywords("MonoBehaviour", "Behaviour", "Script")]
    
    [Image(typeof(IconLayers), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectLayer : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private LayerMaskValue m_Layer = new LayerMaskValue();
        [SerializeField] private bool m_ChildrenToo = false;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Change Layer to {0} on {1} {2}",
            this.m_Layer,
            this.m_GameObject,
            this.m_ChildrenToo ? "and children" : string.Empty
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            gameObject.layer = this.m_Layer.Value;

            if (this.m_ChildrenToo)
            {
                Transform[] children = gameObject.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    child.gameObject.layer = this.m_Layer.Value;
                }
            }
            
            return DefaultResult;
        }
    }
}