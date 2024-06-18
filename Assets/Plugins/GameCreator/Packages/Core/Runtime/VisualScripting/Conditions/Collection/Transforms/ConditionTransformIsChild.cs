using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Child Of")]
    [Description("Returns true if the game object is the parent of the other one")]

    [Category("Transforms/Is Child Of")]
    
    [Parameter("Child", "The game object instance further down in the hierarchy of the parent")]
    [Parameter("Parent", "The game object instance that is higher in the hierarchy")]

    [Keywords("Transform", "Hierarchy", "Descendant", "Ancestor", "Parent", "Father", "Mother")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Green, typeof(OverlayArrowUp))]
    [Serializable]
    public class ConditionTransformIsChild : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Child = new PropertyGetGameObject();
        [SerializeField] private PropertyGetGameObject m_Parent = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Parent} is child of {this.m_Child}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject child = this.m_Child.Get(args);
            GameObject parent = this.m_Parent.Get(args);

            if (child == null) return false;
            if (parent == null) return false;

            return child.transform.IsChildOf(parent.transform);
        }
    }
}
