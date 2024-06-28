using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace DaimahouGames.Runtime.Characters
{
    [CreateAssetMenu(
        fileName = "My Reactive Gesture",
        menuName = "Game Creator/Characters/Reactive Gesture",
        order = -20
    )]
    
    public class ReactiveGesture : ScriptableObject
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [FormerlySerializedAs("m_AbilityClip")]
        [SerializeField] private AnimationClip m_Animation;

        [SerializeField] private AvatarMask m_AvatarMask = null;
        [SerializeField] private BlendMode m_BlendMode = BlendMode.Blend;
        
        [SerializeField] private float m_Delay;
        [SerializeField] private PropertyGetDecimal m_Speed = GetDecimalDecimal.Create(1);
        [SerializeField] private bool m_UseRootMotion;
        [SerializeField] private float m_TransitionIn = 0.1f;
        [SerializeField] private float m_TransitionOut = 0.1f;
        
        [SerializeReference] private INotify[] m_Notifies;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public AnimationClip Clip => m_Animation;
        public AvatarMask AvatarMask => m_AvatarMask;
        public BlendMode BlendMode => m_BlendMode;
        public float Delay => m_Delay;
        public bool UseRootMotion => m_UseRootMotion;
        public float TransitionIn => m_TransitionIn;
        public float TransitionOut => m_TransitionOut;
        public float GetSpeed(Args args) => (float) m_Speed.Get(args);
        public float GetDuration(Args args) => Clip.length / GetSpeed(args);

        public int NotifyCount => m_Notifies.Length;

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public INotify GetNotify(int i) => m_Notifies[i];
        public IEnumerable<INotify> GetNotifies() => m_Notifies;
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}