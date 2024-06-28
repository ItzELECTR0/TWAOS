using System;
using System.Linq;
using DaimahouGames.Runtime.Characters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrototypeCreator.Core.Editor.Characters
{
    public class ReactiveGesturePlayer : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        
        private readonly SerializedObject m_SerializedObject;
        private readonly Animator m_PreviewAnimator;
        
        private bool m_ShouldUpdateProgress;
        
        private Image m_HeadImage;
        private Label m_HeadLabel;
        private Button m_HeadButton;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public ReactiveGesturePlayer(GameObject previewObject, SerializedObject serializedObject)
        {
            if (previewObject == null) return;
            
            m_SerializedObject = serializedObject;
            m_PreviewAnimator = previewObject.GetComponentInChildren<Animator>();
        }
        
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public void SetProgress(float progress)
        {
            if (EditorGUI.EndChangeCheck()) AnimationMode.StartAnimationMode();

            if (!AnimationMode.InAnimationMode()) return;
            if (EditorApplication.isPlaying) return;

            var clips = GetClips();
            
            if (clips.Length <= 0) return;
            if (m_PreviewAnimator == null) return;

            var length = clips.Select(x => x.length).Sum();
            var sampleTime = progress * length;
            var cumulatedClipLength = 0f;

            foreach (var clip in clips)
            {
                var currentClipTime = sampleTime - cumulatedClipLength;
                
                if(currentClipTime >= clip.length)
                {
                    cumulatedClipLength += clip.length;
                    continue;
                }
                
                AnimationMode.BeginSampling();
                {
                    AnimationMode.SampleAnimationClip(
                        m_PreviewAnimator.gameObject,
                        clip,
                        currentClipTime
                    );
                }
                AnimationMode.EndSampling();
                
                break;
            }
        }

        private AnimationClip[] GetClips()
        {
            var reactiveGesture = m_SerializedObject.targetObject as ReactiveGesture;
            if (reactiveGesture != null && reactiveGesture.Clip != null) return  new[] {reactiveGesture.Clip};

            var reactiveState = m_SerializedObject.targetObject as ReactiveState;
            var stateClip = m_SerializedObject.FindProperty("m_StateClip").objectReferenceValue as AnimationClip;
            
            if (reactiveState != null) return new[] {reactiveState.EntryClip, stateClip, reactiveState.ExitClip}
                .Where(x => x != null)
                .ToArray();
            
            return Array.Empty<AnimationClip>();
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}