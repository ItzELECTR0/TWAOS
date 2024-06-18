using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public interface IState
    {
        RuntimeAnimatorController StateController { get; }
        AvatarMask StateMask { get; }
        bool HasStateMask { get; }

        AnimationClip EntryClip { get; }
        bool HasEntryClip { get; }
        AvatarMask EntryMask { get; }
        
        AnimationClip ExitClip { get; }
        bool HasExitClip { get; }
        AvatarMask ExitMask { get; }
    }
}