using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Game Object Active")]
    [Description("Returns true if the game object reference exists and is active")]

    [Category("Game Objects/Is Game Object Active")]
    
    [Parameter("Game Object", "The game object instance used in the condition")]

    [Keywords("Null", "Scene", "Enabled")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionGameObjectActive : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Active {this.m_GameObject}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            return gameObject != null && gameObject.activeInHierarchy;
        }
    }
}
