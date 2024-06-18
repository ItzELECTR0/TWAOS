using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace GameCreator.Editor.Overlays
{
    [Overlay(typeof(SceneView), "Game Creator")]
    [Icon(RuntimePaths.GIZMOS + "GizmoGameCreator.png")]
    
    public class EditorToolbarGameCreator : ToolbarOverlay
    {
        private EditorToolbarGameCreator() : base(
            ToolbarTrigger.ID,
            ToolbarConditions.ID,
            ToolbarActions.ID,
            ToolbarCharacter.ID,
            ToolbarPlayer.ID,
            ToolbarMarker.ID,
            ToolbarHotspot.ID,
            ToolbarShot.ID,
            ToolbarLocalNameVariables.ID,
            ToolbarLocalListVariables.ID
        ) { }
    }
}