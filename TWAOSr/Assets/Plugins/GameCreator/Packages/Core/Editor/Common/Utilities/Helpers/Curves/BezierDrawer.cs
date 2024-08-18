using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(Bezier))]
    public class BezierDrawer : TSectionDrawer
    {
        public static int HANDLES_CONTROL_INDEX = 0;
        
        private const float HANDLE_SIZE = 0.1f;
        private const float PATH_SIZE_COEFFICIENT = 4f;
        private const float TANGENT_SIZE_COEFFICIENT = 1f;
        
        private static readonly Color COLOR_CURVE = Color.blue;
        private static readonly Color COLOR_POINTS = Color.blue;
        private static readonly Color COLOR_CONTROLS = Color.green;

        // PAINT METHODS: -------------------------------------------------------------------------
        
        protected override void CreatePropertyContent(VisualElement container, SerializedProperty property)
        {
            SerializedProperty pointA = property.FindPropertyRelative("m_PointA");
            SerializedProperty pointB = property.FindPropertyRelative("m_PointB");
            
            SerializedProperty controlA = property.FindPropertyRelative("m_ControlA");
            SerializedProperty controlB = property.FindPropertyRelative("m_ControlB");
            
            container.Add(new PropertyField(pointA));
            container.Add(new PropertyField(pointB));

            VisualElement space = new VisualElement();
            space.AddToClassList("gc-space-small");
            container.Add(space);
            
            container.Add(new PropertyField(controlA));
            container.Add(new PropertyField(controlB));
        }
        
        // SCENE GUI: -----------------------------------------------------------------------------

        public static bool OnSceneGUI(SerializedProperty bezier, Transform transform)
        {
            EditorGUI.BeginChangeCheck();

            SerializedProperty pointA = bezier.FindPropertyRelative("m_PointA");
            SerializedProperty pointB = bezier.FindPropertyRelative("m_PointB");
            SerializedProperty controlA = bezier.FindPropertyRelative("m_ControlA");
            SerializedProperty controlB = bezier.FindPropertyRelative("m_ControlB");

            if (Tools.current != Tool.None) HANDLES_CONTROL_INDEX = 0;

            bool changeTool = false;
            
            changeTool |= HandleButton(pointA.vector3Value, transform, COLOR_POINTS, 1);
            changeTool |= HandleButton(pointB.vector3Value, transform, COLOR_POINTS, 2);
            changeTool |= HandleButton(pointA.vector3Value + controlA.vector3Value, transform, COLOR_CONTROLS, 3);
            changeTool |= HandleButton(pointB.vector3Value + controlB.vector3Value, transform, COLOR_CONTROLS, 4);

            switch (HANDLES_CONTROL_INDEX)
            {
                case 1: pointA.vector3Value = HandlePoint(pointA, transform); break;
                case 2: pointB.vector3Value = HandlePoint(pointB, transform); break;
                case 3: controlA.vector3Value = HandleControl(controlA, pointA, transform); break;
                case 4: controlB.vector3Value = HandleControl(controlB, pointB, transform); break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                bezier.serializedObject.ApplyModifiedProperties();
                bezier.serializedObject.Update();
            }
            
            Handles.color = COLOR_CONTROLS;

            Handles.DrawLine(
                transform.TransformPoint(pointA.vector3Value),
                transform.TransformPoint(pointA.vector3Value + controlA.vector3Value),
                Handles.lineThickness * TANGENT_SIZE_COEFFICIENT
            );

            Handles.color = COLOR_CONTROLS;
            Handles.DrawLine(
                transform.TransformPoint(pointB.vector3Value),
                transform.TransformPoint(pointB.vector3Value + controlB.vector3Value),
                Handles.lineThickness * TANGENT_SIZE_COEFFICIENT
            );
            
            Handles.color = COLOR_CURVE;
            Handles.DrawBezier(
                transform.TransformPoint(pointA.vector3Value), 
                transform.TransformPoint(pointB.vector3Value),
                transform.TransformPoint(pointA.vector3Value + controlA.vector3Value), 
                transform.TransformPoint(pointB.vector3Value + controlB.vector3Value), 
                COLOR_CURVE, null, Handles.lineThickness * PATH_SIZE_COEFFICIENT
            );

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
        
        private static Vector3 HandleControl(SerializedProperty property, SerializedProperty parent, Transform transform)
        {
            Quaternion rotation = Tools.pivotRotation switch
            {
                PivotRotation.Local => transform.rotation,
                PivotRotation.Global => Quaternion.identity,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Vector3 position = transform.TransformPoint(parent.vector3Value + property.vector3Value);
            Vector3 result = Handles.PositionHandle(position, rotation);

            return transform.InverseTransformPoint(result) - parent.vector3Value;
        }
    }
}