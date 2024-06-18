using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common
{
    public static class TouchStickUtils
    {
        // STATIC AND CONST: ----------------------------------------------------------------------
        
        private const int SORT_ORDER = 9999;
        private static readonly Vector2 RESOLUTION = new Vector2(1920, 1080);
        
        private static readonly Vector2 ANCHOR_LEFT = new Vector2(0f, 0f);
        private static readonly Vector2 ANCHOR_MIDDLE = new Vector2(0.5f, 0.5f);
        private static readonly Vector2 ANCHOR_RIGHT = new Vector2(1f, 0f);

        private const float SURFACE_OFFSET = 150f + SURFACE_SIZE * 0.5f;
        private const float SURFACE_SIZE = 250f;
        private const float STICK_SIZE = 100f;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void CreateCanvas(GameObject instance)
        {
            Canvas canvas = instance.AddComponent<Canvas>();
            CanvasScaler canvasScaler = instance.AddComponent<CanvasScaler>();
            instance.AddComponent<GraphicRaycaster>();
            
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SORT_ORDER;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = RESOLUTION;
            canvasScaler.matchWidthOrHeight = 1f;
        }
        
        public static void CreateControlsLeft(GameObject instance)
        {
            CreateControls(
                instance,
                out RectTransform surface,
                out RectTransform stick
            );
            
            TouchStickLeft touchStick = surface.gameObject.AddComponent<TouchStickLeft>();

            touchStick.Surface = surface;
            touchStick.Stick = stick;
            touchStick.Root = instance;

            surface.pivot = ANCHOR_MIDDLE;
            surface.anchorMin = ANCHOR_LEFT;
            surface.anchorMax = ANCHOR_LEFT;
            surface.anchoredPosition = new Vector2(SURFACE_OFFSET, SURFACE_OFFSET);
            surface.sizeDelta = new Vector2(SURFACE_SIZE, SURFACE_SIZE);
        
            stick.pivot = ANCHOR_MIDDLE;
            stick.anchorMin = ANCHOR_MIDDLE;
            stick.anchorMax = ANCHOR_MIDDLE;
            stick.anchoredPosition = Vector2.zero;
            stick.sizeDelta = new Vector2(STICK_SIZE, STICK_SIZE);
        }
        
        public static void CreateControlsRight(GameObject instance)
        {
            CreateControls(
                instance,
                out RectTransform surface,
                out RectTransform stick
            );

            TouchStickRight touchStick = surface.gameObject.AddComponent<TouchStickRight>();

            touchStick.Surface = surface;
            touchStick.Stick = stick;
            touchStick.Root = instance;

            surface.pivot = ANCHOR_MIDDLE;
            surface.anchorMin = ANCHOR_RIGHT;
            surface.anchorMax = ANCHOR_RIGHT;
            surface.anchoredPosition = new Vector2(-SURFACE_OFFSET, SURFACE_OFFSET);
            surface.sizeDelta = new Vector2(SURFACE_SIZE, SURFACE_SIZE);
        
            stick.pivot = ANCHOR_MIDDLE;
            stick.anchorMin = ANCHOR_MIDDLE;
            stick.anchorMax = ANCHOR_MIDDLE;
            stick.anchoredPosition = Vector2.zero;
            stick.sizeDelta = new Vector2(STICK_SIZE, STICK_SIZE);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void CreateControls(
            GameObject instance, 
            out RectTransform surfaceTransform,
            out RectTransform stickTransform)
        {
            GameObject surface = new GameObject("Surface");
            GameObject stick = new GameObject("Stick");
            
            surface.transform.SetParent(instance.transform);
            stick.transform.SetParent(surface.transform);
        
            surfaceTransform = surface.AddComponent<RectTransform>();
            stickTransform = stick.AddComponent<RectTransform>();
        
            Image surfaceImage = surface.AddComponent<Image>();
            Image stickImage = stick.AddComponent<Image>();

            surfaceImage.overrideSprite = TouchStickImageSurface.Value;
            stickImage.overrideSprite = TouchStickImageStick.Value;
        }
    }
}