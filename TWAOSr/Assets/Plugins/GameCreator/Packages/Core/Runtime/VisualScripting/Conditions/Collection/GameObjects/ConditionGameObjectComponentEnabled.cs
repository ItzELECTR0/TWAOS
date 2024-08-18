using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Component Enabled")]
    [Description("Returns true if the game object has the component enabled")]

    [Category("Game Objects/Is Component Enabled")]
    
    [Parameter("Game Object", "The game object instance used in the condition")]
    [Parameter("Component", "The component type checked")]

    [Keywords("Null", "Active")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionGameObjectComponentEnabled : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private TypeReferenceComponent m_Component = new TypeReferenceComponent();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Component} on {this.m_GameObject} Enabled";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return false;
            
            Component component = gameObject.Get(this.m_Component.Type);
            if (component == null) return false;
            
            Behaviour behaviour = component as Behaviour;
            return behaviour == null || behaviour.isActiveAndEnabled;
        }
    }
}
