using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public class ClipTool : VisualElement
    {
        private const string NAME_CLIP_CONNECTION_L = "GC-Sequence-Clip-Connection-L";
        private const string NAME_CLIP_CONNECTION_M = "GC-Sequence-Clip-Connection-M";
        private const string NAME_CLIP_CONNECTION_R = "GC-Sequence-Clip-Connection-R";
        
        private const string NAME_CLIP_A = "GC-Sequence-Clip-A";
        private const string NAME_CLIP_B = "GC-Sequence-Clip-B";

        private const string CLASS_CONNECTION_NORMAL = "gc-sequence-clip-connection-normal";
        private const string CLASS_CONNECTION_THIN = "gc-sequence-clip-connection-thin";

        private const int CLICK_THRESHOLD = 5;
        private const float CLIP_WIDTH = 14f;

        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly VisualElement m_ClipConnectionL;
        private readonly VisualElement m_ClipConnectionM;
        private readonly VisualElement m_ClipConnectionR;
        
        private readonly Image m_ClipA;
        private readonly Image m_ClipB;
        
        private readonly HandleDragManipulator m_Manipulator;
        
        private float m_DragOffset;

        private readonly Texture m_IconClipNormal;
        private readonly Texture m_IconClipSelect;

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty Property => this.TrackTool.PropertyClips
            .GetArrayElementAtIndex(this.ClipIndex);
        
        private TrackTool TrackTool { get; }
        public int ClipIndex { get; internal set; }

        public SerializedProperty PropertyTime
        {
            get
            {
                SerializedObject serializedObject = this.Property.serializedObject;
                SerializationUtils.ApplyUnregisteredSerialization(serializedObject);
                return this.Property.FindPropertyRelative("m_Time");
            }
        }

        public SerializedProperty PropertyDuration
        {
            get
            {
                SerializedObject serializedObject = this.Property.serializedObject;
                SerializationUtils.ApplyUnregisteredSerialization(serializedObject);
                return this.Property.FindPropertyRelative("m_Duration");
            }
        }

        protected virtual bool RoundTimelineClip => true; 

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int, int> EventSelect;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ClipTool(TrackTool trackTool, int clipIndex)
        {
            this.TrackTool = trackTool;
            this.ClipIndex = clipIndex;
            this.pickingMode = PickingMode.Ignore;

            this.m_IconClipNormal = this.TrackTool.Track.CustomClipIconNormal;
            this.m_IconClipSelect = this.TrackTool.Track.CustomClipIconSelect;

            if (this.m_IconClipNormal == null)
            {
                this.m_IconClipNormal = new IconSequenceClip(trackTool.Track.ColorClipNormal).Texture;
            }
            
            if (this.m_IconClipSelect == null)
            {
                this.m_IconClipSelect = new IconSequenceClip(trackTool.Track.ColorClipSelect).Texture;
            }

            this.m_Manipulator = new HandleDragManipulator(
                this.OnChangeStart,
                this.OnChangeFinish,
                this.OnChangeMove
            );
            
            this.AddManipulator(this.m_Manipulator);
            
            Color connectionL = trackTool.Track.ColorConnectionLeftNormal;
            Color connectionM = trackTool.Track.ColorConnectionMiddleNormal;
            Color connectionR = trackTool.Track.ColorConnectionRightNormal;
            
            this.m_ClipConnectionL = new VisualElement
            {
                name = NAME_CLIP_CONNECTION_L,
                pickingMode = connectionL == default ? PickingMode.Ignore : PickingMode.Position
            };
            
            this.m_ClipConnectionM = new VisualElement
            {
                name = NAME_CLIP_CONNECTION_M,
                pickingMode = connectionM == default ? PickingMode.Ignore : PickingMode.Position
            };
            
            this.m_ClipConnectionR = new VisualElement
            {
                name = NAME_CLIP_CONNECTION_R,
                pickingMode = connectionR == default ? PickingMode.Ignore : PickingMode.Position
            };

            if (connectionL != default) this.m_ClipConnectionL.style.backgroundColor = connectionL;
            if (connectionM != default) this.m_ClipConnectionM.style.backgroundColor = connectionM;
            if (connectionR != default) this.m_ClipConnectionR.style.backgroundColor = connectionR;
            
            this.m_ClipConnectionL.AddToClassList(trackTool.Track.IsConnectionLeftThin
                ? CLASS_CONNECTION_THIN
                : CLASS_CONNECTION_NORMAL
            );
            
            this.m_ClipConnectionM.AddToClassList(trackTool.Track.IsConnectionMiddleThin
                ? CLASS_CONNECTION_THIN
                : CLASS_CONNECTION_NORMAL
            );
            
            this.m_ClipConnectionR.AddToClassList(trackTool.Track.IsConnectionRightThin
                ? CLASS_CONNECTION_THIN
                : CLASS_CONNECTION_NORMAL
            );

            this.m_ClipA = new Image
            {
                name = NAME_CLIP_A,
                focusable = true,
                pickingMode = PickingMode.Position,
                image = this.m_IconClipNormal
            };
            
            this.m_ClipB = new Image
            {
                name = NAME_CLIP_B,
                focusable = true,
                pickingMode = PickingMode.Position,
                image = this.m_IconClipNormal
            };
            
            this.m_ClipA.AddManipulator(new ContextualMenuManipulator(this.OnContextClipA));
            this.m_ClipB.AddManipulator(new ContextualMenuManipulator(this.OnContextClipB));

            this.Add(this.m_ClipConnectionL);
            this.Add(this.m_ClipConnectionM);
            this.Add(this.m_ClipConnectionR);
            this.Add(this.m_ClipB);
            this.Add(this.m_ClipA);

            this.Refresh();
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnChangeStart()
        {
            this.BringToFront();
            this.TrackTool.Head.BringToFront();

            bool changeTime = 
                this.m_Manipulator.Target == this.m_ClipA ||
                this.m_Manipulator.Target == this.m_ClipConnectionL ||
                this.m_Manipulator.Target == this.m_ClipConnectionM ||
                this.m_Manipulator.Target == this.m_ClipConnectionR;

            bool changeDuration = this.m_Manipulator.Target == this.m_ClipB;

            if (changeTime)
            {
                float clip = this.m_ClipA.transform.position.x;
                this.m_DragOffset = this.m_Manipulator.StartPosition.x - clip;
            }
            
            if (changeDuration)
            {
                float clip = this.m_ClipB.transform.position.x;
                this.m_DragOffset = this.m_Manipulator.StartPosition.x - clip;
            }
            
            this.OnChange();
        }
        
        private void OnChangeFinish()
        {
            if (Mathf.Abs(this.m_Manipulator.Difference.x) < CLICK_THRESHOLD)
            {
                this.Select();
            }
            
            this.OnChange();
        }

        private void OnChangeMove()
        {
            this.OnChange();
        }
        
        private void OnChange()
        {
            this.OnChangeA();
            this.OnChangeB();

            SerializedObject serializedObject = this.Property.serializedObject;
            SerializationUtils.ApplyUnregisteredSerialization(serializedObject);
            
            this.Refresh();
            this.TrackTool.SequenceTool.DetailsTool.RefreshSign();
        }

        private void OnChangeA()
        {
            float offset = CLIP_WIDTH * 0.5f - this.m_DragOffset;
            float position = this.m_Manipulator.Target == this.m_ClipA ||
                          this.m_Manipulator.Target == this.m_ClipConnectionL ||
                          this.m_Manipulator.Target == this.m_ClipConnectionM ||
                          this.m_Manipulator.Target == this.m_ClipConnectionR 
                ? this.m_Manipulator.FinishPosition.x + offset
                : 0f;
            
            if (position == 0f) return;
            float ratio = Mathf.Clamp01(position / this.resolvedStyle.width);
            
            float maxFrame = this.TrackTool.SequenceTool.PlaybackTool.MaxFrame;
            if (this.RoundTimelineClip && maxFrame > 0f)
            {
                float frame = Mathf.Round(Mathf.Clamp01(ratio) * maxFrame);
                ratio = frame / maxFrame;
            }

            if (maxFrame > 0f && this.TrackTool.Track.TrackType == TrackType.Range)
            {
                ratio = Math.Min(ratio, 1f - 1f / maxFrame);
            }

            this.PropertyTime.floatValue = ratio;
        }
        
        private void OnChangeB()
        {
            float offset = CLIP_WIDTH * 0.5f - this.m_DragOffset;
            float position = this.m_Manipulator.Target == this.m_ClipB 
                ? this.m_Manipulator.FinishPosition.x + offset
                : 0f;
            
            if (position == 0f) return;

            float time = this.PropertyTime.floatValue;
            float ratio = Mathf.Clamp01(position / this.resolvedStyle.width) - time;
            
            float maxFrame = this.TrackTool.SequenceTool.PlaybackTool.MaxFrame;
            if (this.RoundTimelineClip && maxFrame > 0f)
            {
                float frame = Mathf.Round(Mathf.Clamp01(ratio) * maxFrame);
                ratio = frame / maxFrame;
            }

            if (maxFrame > 0f)
            {
                ratio = Math.Max(ratio, 1f / maxFrame);
            }
            
            this.PropertyDuration.floatValue = ratio;
        }
        
        private void OnContextClipA(ContextualMenuPopulateEvent eventContext)
        {
            InputDropdownFloat.Open("Time", this.m_ClipA, result =>
            {
                SerializedObject so = this.PropertyTime.serializedObject; 
                so.Update();
                this.PropertyTime.floatValue = result;
                SerializationUtils.ApplyUnregisteredSerialization(so);
                this.TrackTool.SequenceTool.Refresh();
            }, this.PropertyTime.floatValue);
            
            eventContext.StopPropagation();
        }
        
        private void OnContextClipB(ContextualMenuPopulateEvent eventContext)
        {
            InputDropdownFloat.Open("Duration", this.m_ClipB, result =>
            {
                SerializedObject so = this.PropertyTime.serializedObject; 
                so.Update();
                this.PropertyDuration.floatValue = result;
                SerializationUtils.ApplyUnregisteredSerialization(so);
                this.TrackTool.SequenceTool.Refresh();
            }, this.PropertyDuration.floatValue);
            
            eventContext.StopPropagation();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Refresh()
        {
            this.Property.serializedObject.Update();

            float time = this.PropertyTime.floatValue;
            float duration = this.PropertyDuration.floatValue;
            
            bool aDragged = this.m_Manipulator.Target == this.m_ClipA &&
                            this.m_Manipulator.IsDragging;
            
            bool bDragged = this.m_Manipulator.Target == this.m_ClipB &&
                            this.m_Manipulator.IsDragging;
            
            this.SetClip(time, this.m_ClipA, aDragged);
            this.SetClip(time + duration, this.m_ClipB, bDragged);

            this.m_ClipB.style.display = this.TrackTool.Track.TrackType switch
            {
                TrackType.Single => DisplayStyle.None,
                TrackType.Range => DisplayStyle.Flex,
                _ => throw new ArgumentOutOfRangeException()
            };

            this.m_ClipConnectionL.style.display = DisplayStyle.Flex;
            this.m_ClipConnectionM.style.display = DisplayStyle.Flex;
            this.m_ClipConnectionR.style.display = DisplayStyle.Flex;
            
            this.RefreshConnections();
        }

        public void Select()
        {
            this.EventSelect?.Invoke(
                this.TrackTool.TrackIndex,
                this.ClipIndex
            );
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SetClip(float value, Image clip, bool isDragging)
        {
            const float offset = CLIP_WIDTH * 0.5f;

            float x = Mathf.Lerp(0f, this.TrackTool.Width, value);
            float y = !float.IsNaN(clip.transform.position.y) ? clip.transform.position.y : 0f;
            
            Vector3 position = new Vector3(x - offset, y, 0);
            clip.transform.position = position;

            bool isTrack = this.TrackTool.SequenceTool.SelectedTrack == this.TrackTool.TrackIndex;
            
            if (isDragging) clip.image = this.m_IconClipSelect;
            else clip.image = isTrack && this.TrackTool.SelectedClip == this.ClipIndex
                ? this.m_IconClipSelect
                : this.m_IconClipNormal;
        }

        private void RefreshConnections()
        {
            const float offset = CLIP_WIDTH * 0.5f;
            
            Vector3 clipA = this.m_ClipA.transform.position;
            Vector3 clipB = this.m_ClipB.transform.position;

            this.m_ClipConnectionL.transform.position = Vector3.zero;
            this.m_ClipConnectionL.style.width = clipA.x + offset;

            this.m_ClipConnectionM.transform.position = clipA + Vector3.right * offset;
            this.m_ClipConnectionM.style.width = clipB.x - clipA.x;
            
            this.m_ClipConnectionR.transform.position = clipB + Vector3.right * offset;
            this.m_ClipConnectionR.style.width = this.TrackTool.Width - clipB.x - offset;
            
            bool isTrack = this.TrackTool.SequenceTool.SelectedTrack == this.TrackTool.TrackIndex;
            bool isSelected = isTrack && this.TrackTool.SelectedClip == this.ClipIndex;
            
            Color connectionNormalL = this.TrackTool.Track.ColorConnectionLeftNormal;
            Color connectionNormalM = this.TrackTool.Track.ColorConnectionMiddleNormal;
            Color connectionNormalR = this.TrackTool.Track.ColorConnectionRightNormal;
            
            Color connectionSelectL = this.TrackTool.Track.ColorConnectionLeftSelect;
            Color connectionSelectM = this.TrackTool.Track.ColorConnectionMiddleSelect;
            Color connectionSelectR = this.TrackTool.Track.ColorConnectionRightSelect;

            if (connectionSelectL != default) this.m_ClipConnectionL.style.backgroundColor = isSelected
                ? connectionSelectL
                : connectionNormalL;

            if (connectionSelectM != default) this.m_ClipConnectionM.style.backgroundColor = isSelected
                ? connectionSelectM
                : connectionNormalM;
            
            if (connectionSelectR != default) this.m_ClipConnectionR.style.backgroundColor = isSelected
                ? connectionSelectR
                : connectionNormalR;
        }
    }
}