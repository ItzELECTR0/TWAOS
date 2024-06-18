using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Tag")]
    [Description("Returns true if the game object is tagged with a concrete name")]

    [Category("Game Objects/Compare Tag")]
    
    [Parameter("Game Object", "The game object instance used in the condition")]
    [Parameter("Tag", "The Tag name checked against the game object")]

    [Keywords("Belong", "Has", "Is")]
    
    [Image(typeof(IconTag), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionGameObjectTag : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private TagValue m_Tag = new TagValue();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_GameObject} tagged as {this.m_Tag}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            return gameObject != null && gameObject.CompareTag(this.m_Tag.Value);
        }
    }
}
