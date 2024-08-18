using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Game Objects")]
    [Description("Returns true if the game object is the same as another one")]

    [Category("Game Objects/Compare Game Objects")]
    
    [Parameter("Game Object", "The game object instance used in the comparison")]
    [Parameter("Compare To", "The game object instance that is compared against")]

    [Keywords("Same", "Equal", "Exact", "Instance")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionGameObjectCompare : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private PropertyGetGameObject m_CompareTo = new PropertyGetGameObject();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_GameObject} = {this.m_CompareTo}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject a = this.m_GameObject.Get(args);
            GameObject b = this.m_CompareTo.Get(args);

            return a == b;
        }
    }
}
