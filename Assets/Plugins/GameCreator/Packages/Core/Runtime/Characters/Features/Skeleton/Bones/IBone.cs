using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Bone Type")]
    
    public interface IBone
    {
        Transform GetTransform(Animator animator);
    }
}