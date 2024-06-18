using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Layer")]
    [Description("Returns true if the game object belongs to any of the layer mask values")]

    [Category("Game Objects/Compare Layer")]
    
    [Parameter("Game Object", "The game object instance used in the condition")]
    [Parameter("Layer Mask", "A bitmask of Layer values")]

    [Keywords("Mask", "Physics", "Belong", "Has")]
    
    [Image(typeof(IconLayers), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionGameObjectLayerMask : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => string.Format(
            "{0} belongs to {1}", 
            this.m_GameObject,
            LayerMaskValue.GetLayerMaskName(this.m_LayerMask)
        );
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return false;

            int bitmask = this.m_LayerMask.value & (1 << gameObject.layer); 
            return bitmask > 0;
        }
    }
}
