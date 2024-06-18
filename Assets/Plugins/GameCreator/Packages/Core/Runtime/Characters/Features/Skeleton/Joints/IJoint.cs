using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Joint Type")]
    
    public interface IJoint
    {
        Joint Setup(GameObject gameObject, Skeleton skeleton, Animator animator);
    }
}