using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(Segment))]
    public class SegmentDrawer : TSectionDrawer
    {
        public static int HANDLES_CONTROL_INDEX = 0;
        
        private const float HANDLE_SIZE = 0.1f;
        private const float LINE_SIZE_COEFFICIENT = 1f;
        
        private static readonly Color COLOR = Color.yellow;

        // PAINT METHODS: -------------------------------------------------------------------------
        
        protected override void CreatePropertyContent(VisualElement container, SerializedProperty property)
        {
            SerializedProperty pointA = property.FindPropertyRelative("m_PointA");
            SerializedProperty pointB = property.FindPropertyRelative("m_PointB");

            container.Add(new PropertyField(pointA));
            container.Add(new PropertyField(pointB));
        }
        
        // SCENE GUI: -----------------------------------------------------------------------------

        public static bool OnSceneGUI(SerializedProperty segment, Transform transform, bool dotted)
        {
            EditorGUI.BeginChangeCheck();

            SerializedProperty pointA = segment.FindPropertyRelative("m_PointA");
            SerializedProperty pointB = segment.FindPropertyRelative("m_PointB");

            if (Tools.current != Tool.None) HANDLES_CONTROL_INDEX = 0;
            
            bool changeTool = false;
            
            changeTool |=  HandleButton(pointA.vector3Value, transform, COLOR, 1);
            changeTool |= HandleButton(pointB.vector3Value, transform, COLOR, 2);

            switch (HANDLES_CONTROL_INDEX)
            {
                case 1: pointA.vector3Value = HandlePoint(pointA, transform); break;
                case 2: pointB.vector3Value = HandlePoint(pointB, transform); break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                segment.serializedObject.ApplyModifiedProperties();
                segment.serializedObject.Update();
            }
            
            Handles.color = COLOR;
            float width = Handles.lineThickness * LINE_SIZE_COEFFICIENT;
            
            if (dotted)
            {
                Handles.DrawDottedLine(
                    transform.TransformPoint(pointA.vector3Value), 
                    transform.TransformPoint(pointB.vector3Value),
                    width
                );
            }
            else
            {
                Handles.DrawLine(
                    transform.TransformPoint(pointA.vector3Value), 
                    transform.TransformPoint(pointB.vector3Value),
                    width
                );
            }

            SceneView.RepaintAll();
            return changeTool;
        }
        
        private static bool HandleButton(Vector3 point, Transform transform, Color color, int index)
        {
            Handles.color = color;
            Vector3 position = transform.TransformPoint(point);
            Quaternion rotation = transform.rotation;
            
            bool select = Handles.Button(
                position, 
                rotation, 
                HANDLE_SIZE, 
                HANDLE_SIZE * 2f,
                Handles.SphereHandleCap
            );
            
            if (select)
            {
                HANDLES_CONTROL_INDEX = index;
                Tools.current = Tool.None;
                return true;
            }

            return false;
        }

        private static Vector3 HandlePoint(SerializedProperty property, Transform transform)
        {
            Quaternion rotation = Tools.pivotRotation switch
            {
                PivotRotation.Local => transform.rotation,
                PivotRotation.Global => Quaternion.identity,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Vector3 position = transform.TransformPoint(property.vector3Value);
            Vector3 result = Handles.PositionHandle(position, rotation);

            return transform.InverseTransformPoint(result);
        }
    }
}