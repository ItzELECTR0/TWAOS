using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconComponent), ColorTheme.Type.Green)]
    
    [Title("Component Enabled")]
    [Category("Game Object/Component Enabled")]
    
    [Description("Remembers if the specified component of the object is enabled")]

    [Serializable]
    public class MemoryComponent : Memory
    {
        [SerializeField] private TypeReferenceBehaviour m_Component = new TypeReferenceBehaviour();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Is {this.m_Component} Enabled";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override Token GetToken(GameObject target)
        {
            Behaviour behaviour = this.GetBehaviour(target);
            return new TokenComponent(behaviour);
        }

        public override void OnRemember(GameObject target, Token token)
        {
            if (token is not TokenComponent tokenEnabled) return;
            
            Behaviour behaviour = this.GetBehaviour(target);
            if (behaviour != null) behaviour.enabled = tokenEnabled.Enabled;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Behaviour GetBehaviour(GameObject target)
        {
            Component component = target.Get(this.m_Component.Type);
            return component is Behaviour behaviour ? behaviour : null;
        }
    }
}