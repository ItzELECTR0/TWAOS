using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Does Component Exist")]
    [Description("Returns true if the game object has the component attached")]

    [Category("Game Objects/Does Component Exist")]
    
    [Parameter("Game Object", "The game object instance used in the condition")]
    [Parameter("Component", "The component type that is searched")]

    [Keywords("Null", "Scene", "Lives")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionGameObjectComponentExists : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private TypeReferenceComponent m_Component = new TypeReferenceComponent();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_GameObject} has {this.m_Component}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return false;
            
            return gameObject.Get(this.m_Component.Type) != null;
        }
    }
}
