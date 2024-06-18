using System;
using System.Reflection;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Editor.Common.UnityUI
{
    public static class UnityUIUtilities
    {
        private const BindingFlags FLAGS_RESOURCES = BindingFlags.Static | BindingFlags.NonPublic;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static GameObject GetCanvas()
        {
            GameObject selection = Selection.activeGameObject;

            Canvas canvas = (selection != null) ? selection.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }
            
            canvas = UnityEngine.Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
            {
                return canvas.gameObject;
            }

            GameObject newObject = new GameObject("Canvas");
            newObject.AddComponent<Canvas>();
            newObject.AddComponent<CanvasScaler>();
            newObject.AddComponent<GraphicRaycaster>();
            
            return newObject;
        }

        public static DefaultControls.Resources GetStandardResources()
        {
            Type menuOptions = TypeUtils.GetTypeFromName("MenuOptions");
            if (menuOptions == null) return new DefaultControls.Resources();
            
            MethodInfo method = menuOptions.GetMethod(
                "GetStandardResources", 
                FLAGS_RESOURCES
            );
            
            object result = method?.Invoke(null, new object[0]);
            return (DefaultControls.Resources?) result ?? new DefaultControls.Resources();
        }

        public static TMP_DefaultControls.Resources GetTMPStandardResources()
        {
            MethodInfo method = typeof(TMPro_CreateObjectMenu).GetMethod(
                "GetStandardResources", 
                FLAGS_RESOURCES
            );

            object result = method?.Invoke(null, new object[0]);
            return (TMP_DefaultControls.Resources?) result ?? new TMP_DefaultControls.Resources();
        }
    }
}