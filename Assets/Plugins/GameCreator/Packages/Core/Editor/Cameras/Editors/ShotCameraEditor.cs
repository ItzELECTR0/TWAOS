using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Cameras;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Cameras
{
    [CustomEditor(typeof(ShotCamera), true)]
    public class ShotCameraEditor : UnityEditor.Editor
    {
        private const string PROP_IS_MAIN = "m_IsMainShot";
        private const string PROP_TIME_MODE = "m_TimeMode";
        private const string PROP_CLIPPING = "m_Clipping";
        private const string PROP_SHOT_TYPE = "m_ShotType";

        // EVENTS: --------------------------------------------------------------------------------
        
        public static event Action<ShotCamera, SerializedObject> EventSceneGUI;
        
        // CREATE INSPECTOR: ----------------------------------------------------------------------

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            SerializedProperty isMainShot = this.serializedObject.FindProperty(PROP_IS_MAIN);
            SerializedProperty timeMode = this.serializedObject.FindProperty(PROP_TIME_MODE);
            SerializedProperty clipping = this.serializedObject.FindProperty(PROP_CLIPPING);
            SerializedProperty shotType = this.serializedObject.FindProperty(PROP_SHOT_TYPE);

            PropertyField fieldIsMainShot = new PropertyField(isMainShot);
            PropertyField fieldTimeMode = new PropertyField(timeMode);
            PropertyField fieldClipping = new PropertyField(clipping);
            PropertyField fieldShotType = new PropertyField(shotType);

            root.Add(fieldIsMainShot);
            root.Add(fieldTimeMode);
            root.Add(fieldClipping);
            root.Add(fieldShotType);

            return root;
        }

        private void OnEnable()
        {
            EventSceneGUI = null;
        }
        
        private void OnDisable()
        {
            EventSceneGUI = null;
        }

        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/Cameras/Camera Shot", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Camera Shot");
            ShotCamera shotCamera = instance.AddComponent<ShotCamera>();

            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
            
            TCamera camera = FindObjectOfType<TCamera>();
            if (camera != null)
            {
                if (camera.Transition.CurrentShotCamera != null) return;
            }
            else
            {
                Camera mainCamera = Camera.main;
                if (mainCamera == null) mainCamera = FindObjectOfType<Camera>();

                if (mainCamera == null) return;
                camera = mainCamera.gameObject.AddComponent<MainCamera>();
            }

            camera.Transition.CurrentShotCamera = shotCamera;
        }
        
        // GUI: -----------------------------------------------------------------------------------
        
        private void OnSceneGUI()
        {
            ShotCamera shotCamera = this.target as ShotCamera;
            EventSceneGUI?.Invoke(shotCamera, this.serializedObject);
        }
    }
}