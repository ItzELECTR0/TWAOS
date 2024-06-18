using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCreator.Runtime.Characters.Animim
{
    public class GesturesOutput : TAnimimOutput
    {
        [NonSerialized] private readonly List<GesturePlayableBehaviour> m_ActiveList;

        // PROPERTIES: ----------------------------------------------------------------------------

        internal override float RootMotion
        {
            get
            {
                float motion = 0f;
                foreach (GesturePlayableBehaviour gesture in this.m_ActiveList)
                {
                    motion = Math.Max(motion, gesture.RootMotion);
                }

                return motion;
            }
        }

        public bool IsPlaying => this.m_ActiveList.Count > 0;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public GesturesOutput(AnimimGraph animimGraph) : base(animimGraph)
        {
            this.m_ActiveList = new List<GesturePlayableBehaviour>();
        }

        public GesturesOutput() : base(null)
        {
            this.m_ActiveList = new List<GesturePlayableBehaviour>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        /// <summary>
        /// Plays an Animation Clip as a Gesture
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="avatarMask"></param>
        /// <param name="blendMode"></param>
        /// <param name="config"></param>
        /// <param name="stopPreviousGestures"></param>
        public async Task CrossFade(AnimationClip animationClip, AvatarMask avatarMask,
            BlendMode blendMode, ConfigGesture config, bool stopPreviousGestures)
        {
            GesturePlayableBehaviour template = new GesturePlayableBehaviour(
                animationClip, avatarMask,
                blendMode, this.m_AnimimGraph, config
            );

            var gesturePlayable = ScriptPlayable<GesturePlayableBehaviour>.Create(
                this.m_AnimimGraph.Graph, template, 1
            );

            GesturePlayableBehaviour behavior = this.Play(
                ref gesturePlayable, config,
                stopPreviousGestures
            );

            while (!behavior.IsComplete && !ApplicationManager.IsExiting)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Smoothly stops all Gestures
        /// </summary>
        /// <param name="delay">Amount of seconds to wait</param>
        /// <param name="transitionOut">The seconds it takes to fade them out</param>
        public void Stop(float delay, float transitionOut)
        {
            foreach (GesturePlayableBehaviour gesture in this.m_ActiveList)
            {
                gesture.Stop(delay, transitionOut);
            }
        }

        /// <summary>
        /// Stops a Gesture that is currently playing
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="delay"></param>
        /// <param name="transitionOut"></param>
        public void Stop(AnimationClip animationClip, float delay, float transitionOut)
        {
            if (animationClip == null) return;
            int hash = animationClip.GetHashCode();

            foreach (GesturePlayableBehaviour gesture in this.m_ActiveList)
            {
                if (gesture.AnimationClipHash != hash) continue;
                gesture.Stop(delay, transitionOut);
            }
        }
        
        /// <summary>
        /// Changes the speed coefficient of a playing Animation Clip
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="speed"></param>
        public void SetSpeed(AnimationClip animationClip, float speed)
        {
            if (animationClip == null) return;
            int hash = animationClip.GetHashCode();

            foreach (GesturePlayableBehaviour gesture in this.m_ActiveList)
            {
                if (gesture.AnimationClipHash != hash) continue;
                gesture.Speed = speed;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private GesturePlayableBehaviour Play(
            ref ScriptPlayable<GesturePlayableBehaviour> gesturePlayable,
            ConfigGesture config, bool stopPreviousGestures)
        {
            if (stopPreviousGestures)
            {
                this.Stop(config.DelayIn + config.TransitionIn + SAFE_TIME_EPSILON, 0f);
            }

            Playable input = this.ScriptPlayable.GetInput(0);
            this.ScriptPlayable.DisconnectInput(0);

            gesturePlayable.ConnectInput(0, input, 0);
            gesturePlayable.SetInputWeight(0, 1f);

            this.ScriptPlayable.ConnectInput(0, gesturePlayable, 0);
            this.ScriptPlayable.SetInputWeight(0, 1f);

            GesturePlayableBehaviour behaviour = gesturePlayable.GetBehaviour();

            this.m_ActiveList.Add(behaviour);
            behaviour.Create(this);

            return behaviour;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        internal override void OnDeleteChild(TAnimimPlayableBehaviour playableBehaviour)
        {
            GesturePlayableBehaviour gesture = playableBehaviour as GesturePlayableBehaviour;
            this.m_ActiveList.Remove(gesture);
        }
    }
}
