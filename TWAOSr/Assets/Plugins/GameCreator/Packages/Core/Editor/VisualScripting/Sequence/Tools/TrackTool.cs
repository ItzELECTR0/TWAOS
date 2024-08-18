using System;
using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public abstract class TrackTool : VisualElement
    {
        private const string NAME_ROOT = "GC-Sequence-Track-Root";
        
        private const string NAME_LANE = "GC-Sequence-Track-Lane";
        private const string NAME_HEAD = "GC-Sequence-Track-Head";
        private const string NAME_TRACK = "GC-Sequence-Track-Track";
        
        private const string NAME_LANE_BUTTON_L = "GC-Sequence-Track-Lane-Button";
        private const string NAME_LANE_BUTTON_R = "GC-Sequence-Track-Lane-Button";

        private static readonly IIcon ICON_TRACK = new IconSequenceTrack(ColorTheme.Type.Background);
        private static readonly IIcon ICON_RMV = new IconSequenceClipRemove(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_ADD = new IconSequenceClipAdd(ColorTheme.Type.TextLight);

        private static readonly Color COLOR_HEAD_ON = ColorTheme.Get(ColorTheme.Type.TextNormal);
        private static readonly Color COLOR_HEAD_OFF = ColorTheme.Get(ColorTheme.Type.TextLight);

        private const int PLAYHEAD_SIZE = 1;

        private const string HINT_ADD = "Add a new Clip";
        private const string HINT_RMV = "Remove selected Clip";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly SerializedProperty m_PropertyTrack;

        private readonly VisualElement m_Lane;
        private readonly VisualElement m_Head;
        private readonly VisualElement m_Track;

        private readonly Button m_ButtonAdd;
        private readonly Button m_ButtonRemove;

        // PROPERTIES: ----------------------------------------------------------------------------

        public SequenceTool SequenceTool { get; }
        public List<ClipTool> ClipTools { get; }

        public SerializedProperty PropertyClips => this.SequenceTool.Tracks
            .GetArrayElementAtIndex(this.TrackIndex)
            .FindPropertyRelative("m_Clips");

        public int TrackIndex { get; }
        public int SelectedClip { get; private set; } = -1;
        
        public Track Track
        {
            get
            {
                SerializedObject serializedObject = this.SequenceTool.SerializedObject;
                SerializationUtils.ApplyUnregisteredSerialization(serializedObject);
                
                return this.m_PropertyTrack.managedReferenceValue as Track;
            }
        }

        public VisualElement Head => this.m_Head;

        public float Width => this.m_Track.resolvedStyle.width;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int, int> EventSelectClip;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public TrackTool(SequenceTool sequenceTool, int trackIndex)
        {
            this.name = NAME_ROOT;
            
            this.SequenceTool = sequenceTool;
            this.TrackIndex = trackIndex;

            this.m_PropertyTrack = this.SequenceTool.Tracks.GetArrayElementAtIndex(trackIndex);

            this.m_Lane = new VisualElement
            {
                name = NAME_LANE,
                pickingMode = PickingMode.Ignore,
                style = { backgroundImage = new StyleBackground(ICON_TRACK.Texture) }
            };

            this.m_Head = new VisualElement
            {
                name = NAME_HEAD,
                pickingMode = PickingMode.Ignore,
                style =
                {
                    backgroundColor = COLOR_HEAD_OFF,
                    width = PLAYHEAD_SIZE
                }
            };

            this.m_Track = new VisualElement { name = NAME_TRACK };
            
            SerializedProperty clips = this.PropertyClips;
            int clipsSize = clips.arraySize;
            this.ClipTools = new List<ClipTool>();
            
            for (int i = 0; i < clipsSize; ++i)
            {
                ClipTool clipTool = new ClipTool(this, i);
                clipTool.EventSelect += this.OnChangeTrackSelection;
                
                this.ClipTools.Add(clipTool);;
                this.m_Track.Add(clipTool);
            }

            this.SequenceTool.PlaybackTool.EventChange += this.OnChangeHeadPosition;
            this.SequenceTool.PlaybackTool.EventDragStart += this.OnDragHeadStart;
            this.SequenceTool.PlaybackTool.EventDragFinish += this.OnDragHeadFinish;

            this.m_Lane.RegisterCallback<GeometryChangedEvent>(this.OnChangeGeometry);

            this.m_Lane.Add(this.m_Track);
            this.m_Track.Add(this.m_Head);

            if (this.Track.AllowRemove != TrackRemoveType.Deny)
            {
                this.m_ButtonRemove = new Button(this.RemoveClip)
                {
                    name = NAME_LANE_BUTTON_L,
                    tooltip = HINT_RMV,
                    style = { width = sequenceTool.TracksOffsetL }
                };
                
                this.m_ButtonRemove.Add(new Image { image = ICON_RMV.Texture });
                this.Add(this.m_ButtonRemove);
            }
            else
            {
                this.style.paddingLeft = sequenceTool.TracksOffsetL;
            }
            
            this.Add(this.m_Lane);
            
            if (this.Track.AllowAdd != TrackAddType.Deny)
            {
                this.m_ButtonAdd = new Button(this.AddClip)
                {
                    name = NAME_LANE_BUTTON_R,
                    tooltip = HINT_ADD,
                    style = { width = sequenceTool.TracksOffsetR }
                };
                
                this.m_ButtonAdd.Add(new Image { image = ICON_ADD.Texture });
                this.Add(this.m_ButtonAdd);
            }
            else
            {
                this.style.paddingRight = sequenceTool.TracksOffsetR;
            }
            
            this.SequenceTool.EventSelect += this.OnChangeSelection;
            
            this.OnChangeHeadPosition();
            this.RefreshButtons();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Select(int clipIndex)
        {
            this.SelectedClip = clipIndex;
            this.Refresh();
        }
        
        public void Refresh()
        {
            this.RefreshClips();
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------
        
        private void OnChangeHeadPosition()
        {
            float value = this.SequenceTool.PlaybackTool.Value;
            
            float x = Mathf.Lerp(0f, this.m_Track.resolvedStyle.width, value);
            float y = this.m_Head.transform.position.y;
            
            Vector3 position = new Vector3(x - PLAYHEAD_SIZE * 0.5f, y, 0);
            this.m_Head.transform.position = position;
        }
        
        private void OnDragHeadFinish()
        {
            this.m_Head.style.backgroundColor = COLOR_HEAD_OFF;
        }

        private void OnDragHeadStart()
        {
            this.m_Head.style.backgroundColor = COLOR_HEAD_ON;
        }
        
        private void OnChangeGeometry(GeometryChangedEvent eventGeometry)
        {
            if (eventGeometry.oldRect == eventGeometry.newRect) return;
            this.OnChangeHeadPosition();
            this.RefreshClips();
        }

        private void OnChangeTrackSelection(int trackIndex, int clipIndex)
        {
            this.EventSelectClip?.Invoke(trackIndex, clipIndex);
        }

        private void OnChangeSelection(int trackIndex, int clipIndex)
        {
            this.RefreshButtons();
            this.RefreshClips();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshClips()
        {
            foreach (ClipTool clip in this.ClipTools)
            {
                clip.Refresh();
            }
        }
        
        private void RemoveClip()
        {
            if (this.Track.AllowRemove == TrackRemoveType.Deny) return;

            int removeIndex = this.Track.AllowAdd switch
            {
                TrackAddType.Allow => this.SelectedClip,
                TrackAddType.Deny => this.SelectedClip,
                TrackAddType.OnlyOne => 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (removeIndex < 0 || removeIndex >= this.ClipTools.Count) return;
            
            SerializedObject serializedObject = this.m_PropertyTrack.serializedObject;
            serializedObject.Update();
            
            this.PropertyClips.DeleteArrayElementAtIndex(removeIndex);
            SerializationUtils.ApplyUnregisteredSerialization(serializedObject);

            ClipTool removedClipTool = this.ClipTools[removeIndex];
            this.ClipTools.RemoveAt(removeIndex);
            this.m_Track.Remove(removedClipTool);
            
            for (int i = this.ClipTools.Count - 1; i >= 0; --i)
            {
                this.ClipTools[i].ClipIndex = i;
                this.ClipTools[i].Refresh();
            }

            this.SelectedClip = -1;
            this.SequenceTool.DetailsTool.Refresh();
            this.RefreshButtons();
        }

        private void AddClip()
        {
            if (this.Track.AllowAdd == TrackAddType.Deny) return;

            SerializedObject serializedObject = this.m_PropertyTrack.serializedObject;
            serializedObject.Update();

            if (this.Track.AllowAdd == TrackAddType.OnlyOne && this.ClipTools.Count > 0)
            {
                return;
            }
            
            float time = this.SequenceTool.PlaybackTool.Value;
            float duration = this.Track.TrackType == TrackType.Range
                ? (1f - time) * 0.5f
                : 0f;
            
            int newIndex = this.PropertyClips.arraySize;
            
            this.PropertyClips.InsertArrayElementAtIndex(newIndex);
            SerializationUtils.ApplyUnregisteredSerialization(serializedObject);
            
            SerializedProperty newClip = this.PropertyClips.GetArrayElementAtIndex(newIndex);
            
            this.OnCreateClip(newClip, time, duration);
            SerializationUtils.ApplyUnregisteredSerialization(serializedObject);
            
            ClipTool clipTool = new ClipTool(this, newIndex);
            clipTool.EventSelect += this.OnChangeTrackSelection;
            this.ClipTools.Add(clipTool);
            this.m_Track.Add(clipTool);
            
            this.SequenceTool.SelectClip(this.TrackIndex, newIndex);
            
            this.m_Head.BringToFront();
            this.RefreshButtons();
        }

        // private void CreateNewClip(SerializedProperty newClip, float time, float duration)
        // {
        //     SerializedProperty instructions = newClip.FindPropertyRelative(ClipDefault.NAME_INSTRUCTIONS);
        //     
        //     if (instructions != null) 
        //     {
        //         instructions
        //             .FindPropertyRelative(InstructionListDrawer.NAME_INSTRUCTIONS)
        //             .arraySize = 0;
        //     }
        // }

        private void RefreshButtons()
        {
            this.m_ButtonRemove?.SetEnabled(
                this.Track.AllowAdd switch
                {
                    TrackAddType.Allow => this.SequenceTool.SelectedTrack == this.TrackIndex && 
                                          this.SelectedClip != -1,
                    TrackAddType.Deny => false,
                    TrackAddType.OnlyOne => this.ClipTools.Count >= 1,
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
            
            this.m_ButtonAdd?.SetEnabled(
                this.Track.AllowAdd switch
                {
                    TrackAddType.Allow => true,
                    TrackAddType.Deny => false,
                    TrackAddType.OnlyOne => this.ClipTools.Count == 0,
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
        }
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected abstract void OnCreateClip(SerializedProperty clip, float time, float duration);
    }
}