using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Cameras;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemTrack))]
    public class ShotSystemTrackDrawer : TShotSystemDrawer
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Name(SerializedProperty property) => "Track";

        // ON ENABLE: -----------------------------------------------------------------------------
        
        protected override void OnEnable()
        {
            ShotCameraEditor.EventSceneGUI -= OnSceneGUI;
            ShotCameraEditor.EventSceneGUI += OnSceneGUI;
        }
        
        // SCENE GUI: -----------------------------------------------------------------------------

        private static void OnSceneGUI(ShotCamera shotCamera, SerializedObject serializedObject)
        {
            ShotTypeTrack shotTypeTrack = shotCamera != null 
                ? shotCamera.ShotType as ShotTypeTrack 
                : null;
            
            if (shotTypeTrack == null) return;
            Type type = typeof(ShotTypeTrack);
            
            SerializedProperty shotType = serializedObject.FindProperty("m_ShotType");
            if (!shotType.managedReferenceFullTypename.Contains(type.FullName ?? string.Empty))
            {
                return;
            }

            SerializedProperty track = shotType.FindPropertyRelative("m_ShotSystemTrack");

            SerializedProperty bezier = track.FindPropertyRelative("m_Track");
            SerializedProperty segment = track.FindPropertyRelative("m_RelativeTo");

            if (BezierDrawer.OnSceneGUI(bezier, shotCamera.transform) &&
                BezierDrawer.HANDLES_CONTROL_INDEX != 0)
            {
                SegmentDrawer.HANDLES_CONTROL_INDEX = 0;   
            }

            if (SegmentDrawer.OnSceneGUI(segment, shotCamera.transform, false) &&
                SegmentDrawer.HANDLES_CONTROL_INDEX != 0)
            {
                BezierDrawer.HANDLES_CONTROL_INDEX = 0;   
            }

            Handles.color = Color.yellow;
            
            Handles.DrawDottedLine(
                shotCamera.transform.TransformPoint(bezier.FindPropertyRelative("m_PointA").vector3Value),
                shotCamera.transform.TransformPoint(segment.FindPropertyRelative("m_PointA").vector3Value),
                Handles.lineThickness
            );
            
            Handles.DrawDottedLine(
                shotCamera.transform.TransformPoint(bezier.FindPropertyRelative("m_PointB").vector3Value),
                shotCamera.transform.TransformPoint(segment.FindPropertyRelative("m_PointB").vector3Value),
                Handles.lineThickness
            );
        }
    }
}