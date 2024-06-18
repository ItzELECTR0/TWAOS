using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Cameras;
using UnityEditor;

namespace GameCreator.Editor.Cameras
{
    [CustomPropertyDrawer(typeof(ShotSystemAnimation))]
    public class ShotSystemAnimationDrawer : TShotSystemDrawer
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Name(SerializedProperty property) => "Animation";

        // ON ENABLE: -----------------------------------------------------------------------------
        
        protected override void OnEnable()
        {
            ShotCameraEditor.EventSceneGUI -= OnSceneGUI;
            ShotCameraEditor.EventSceneGUI += OnSceneGUI;
        }
        
        // SCENE GUI: -----------------------------------------------------------------------------

        private static void OnSceneGUI(ShotCamera shotCamera, SerializedObject serializedObject)
        {
            ShotTypeAnimation shotTypeAnimation = shotCamera != null 
                ? shotCamera.ShotType as ShotTypeAnimation 
                : null;
            
            if (shotTypeAnimation == null) return;
            Type type = typeof(ShotTypeAnimation);
            
            SerializedProperty shotType = serializedObject.FindProperty("m_ShotType");
            if (!shotType.managedReferenceFullTypename.Contains(type.FullName ?? string.Empty))
            {
                return;
            }

            SerializedProperty animation = shotType.FindPropertyRelative("m_ShotSystemAnimation");

            SerializedProperty bezier = animation.FindPropertyRelative("m_Path");
            BezierDrawer.OnSceneGUI(bezier, shotCamera.transform);
        }
    }
}