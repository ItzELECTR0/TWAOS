using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public abstract class StateOverrideAnimator : State
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected AnimatorOverrideController m_Controller;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override RuntimeAnimatorController StateController => this.m_Controller;
    }
}
