using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Sleeping")]
    [Description("Returns true if the game object's Rigidbody or Rigidbody2D is sleeping")]

    [Category("Physics/Is Sleeping")]
    
    [Parameter("Game Object", "The game object instance with a Rigidbody or Rigidbody2D")]

    [Keywords("Affect", "Physics", "Force", "Rigidbody", "Awake")]
    
    [Image(typeof(IconPhysics), ColorTheme.Type.Green, typeof(OverlayZ))]
    [Serializable]
    public class ConditionPhysicsIsSleeping : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_GameObject} is Sleeping";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return false;

            Rigidbody rigidbody3D = gameObject.Get<Rigidbody>();
            if (rigidbody3D != null) return rigidbody3D.IsSleeping();

            Rigidbody2D rigidbody2D = gameObject.Get<Rigidbody2D>();
            if (rigidbody2D != null) return rigidbody2D.IsSleeping();

            return false;
        }
    }
}
