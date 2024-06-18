using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Sibling Of")]
    [Description("Returns true if the game object shares the same parent as the other one")]

    [Category("Transforms/Is Sibling Of")]
    
    [Parameter("Sibling A", "The game object instance compared")]
    [Parameter("Sibling B", "Another game object instance compared")]

    [Keywords("Transform", "Hierarchy", "Ancestor", "Brother", "Sister")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Serializable]
    public class ConditionTransformSiblings : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_SiblingA = new PropertyGetGameObject();
        [SerializeField] private PropertyGetGameObject m_SiblingB = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_SiblingB} is sibling of {this.m_SiblingA}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject siblingA = this.m_SiblingA.Get(args);
            GameObject siblingB = this.m_SiblingB.Get(args);

            if (siblingA == null) return false;
            if (siblingB == null) return false;

            return siblingA.transform.parent == siblingB.transform.parent;
        }
    }
}
