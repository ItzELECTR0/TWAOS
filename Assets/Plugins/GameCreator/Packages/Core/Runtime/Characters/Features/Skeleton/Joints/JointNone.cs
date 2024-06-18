using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("No Joint")]
    [Category("No Joint")]
    
    [Image(typeof(IconEmpty), ColorTheme.Type.TextNormal)]
    [Description("Do not use any Joint")]
    
    [Serializable]
    public class JointNone : IJoint
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public Joint Setup(GameObject gameObject, Skeleton skeleton, Animator animator)
        {
            return null;
        }

        public override string ToString()
        {
            return "no joint";
        }
    }
}