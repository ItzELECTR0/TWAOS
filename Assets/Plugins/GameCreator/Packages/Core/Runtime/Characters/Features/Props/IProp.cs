using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    internal interface IProp
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        Transform Bone { get; }
        GameObject Instance { get; }
        
        // METHODS: -------------------------------------------------------------------------------

        void Create(Animator animator);
        void Destroy();
        void Drop();
    }
}