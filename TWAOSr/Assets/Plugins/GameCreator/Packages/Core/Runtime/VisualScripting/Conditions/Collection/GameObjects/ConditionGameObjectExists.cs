using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Does Game Object Exist")]
    [Description("Returns true if the game object reference is not null")]

    [Category("Game Objects/Does Game Object Exist")]
    
    [Parameter("Game Object", "The game object instance used in the condition")]

    [Keywords("Null", "Scene", "Lives")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionGameObjectExists : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_GameObject} Exist";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            return gameObject != null;
        }
    }
}
