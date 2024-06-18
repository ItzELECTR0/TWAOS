using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/characters/animation/states")]
    [Icon(RuntimePaths.GIZMOS + "GizmoStateAnimation.png")]
    public class StateAnimation : StateOverrideAnimator
    {
        [SerializeField] private AnimationClip m_StateClip;
        
        // SERIALIZATION CALLBACKS: ---------------------------------------------------------------

        protected sealed override void BeforeSerialize()
        {
            if (this.m_Controller == null) return;
            this.m_Controller["Human@Action"] = this.m_StateClip;
        }

        protected sealed override void AfterSerialize()
        { }
    }
}