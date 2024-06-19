using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// A Visual Element which handles and displays a Tile Palette Clipboard.
    /// A Tile Palette Clipboard shows the Active Palette for Grid Painting and allows
    /// users to use the Active Brush to assign and pick items for painting.
    /// </summary>
    [UxmlElement]
    public partial class TilePaletteClipboardElement : VisualElement
    {
        /// <summary>
        /// Factory for TilePaletteClipboardElement.
        /// </summary>
        [Obsolete("TilePaletteClipboardElementFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
        public class TilePaletteClipboardElementFactory : UxmlFactory<TilePaletteClipboardElement, TilePaletteClipboardElementUxmlTraits> {}
        /// <summary>
        /// UxmlTraits for TilePaletteClipboardElement.
        /// </summary>
        [Obsolete("TilePaletteClipboardElementUxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
        public class TilePaletteClipboardElementUxmlTraits : UxmlTraits {}

        private static readonly string ussClassName = "unity-tilepalette-clipboard-element";
        private static readonly string k_Name = L10n.Tr("Tile Palette Clipboard Element");

        private GridPaintPaletteClipboard m_TilePaletteClipboard;
        private EditorWindow m_Window;

        /// <summary>
        /// Callback when the active Brush does a Pick on the Clipboard.
        /// </summary>
        public event Action onBrushPicked;

        /// <summary>
        /// Whether the clipboard is unlocked for editing.
        /// </summary>
        public bool clipboardUnlocked
        {
            get => m_TilePaletteClipboard.unlocked;
            set => m_TilePaletteClipboard.unlocked = value;
        }

        /// <summary>
        /// The last active grid position on the clipboard.
        /// </summary>
        public Vector3Int clipboardMouseGridPosition => new Vector3Int(m_TilePaletteClipboard.mouseGridPosition.x, m_TilePaletteClipboard.mouseGridPosition.y, m_TilePaletteClipboard.zPosition);

        /// <summary>
        /// Callback when the clipboard unlock status has changed
        /// </summary>
        public event Action<bool> clipboardUnlockedChanged;

        internal GridPaintPaletteClipboard clipboardView => m_TilePaletteClipboard;

        private TilePaletteClipboardFirstUserElement m_FirstUserElement;
        private TilePaletteClipboardErrorElement m_ErrorElement;
        private Image m_ClipboardImageElement;

        private static readonly PrefColor tilePaletteBackgroundColor = new PrefColor("2D/Tile Palette Background"
            , 1.0f / 255.0f // Light
            , 35.0f / 255.0f
            , 90.0f / 255.0f
            , 127.0f / 255.0f
            , 1.0f / 255.0f // Dark
            , 35.0f / 255.0f
            , 90.0f / 255.0f
            , 127.0f / 255.0f);

        /// <summary>
        /// Initializes and returns an instance of TilePaletteClipboardElement.
        /// </summary>
        public TilePaletteClipboardElement()
        {
            AddToClassList(ussClassName);

            name = k_Name;
            TilePaletteOverlayUtility.SetStyleSheet(this);

            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            m_ClipboardImageElement = new Image();
            m_ClipboardImageElement.style.backgroundColor = tilePaletteBackgroundColor.Color;
            m_ClipboardImageElement.focusable = true;
            Add(m_ClipboardImageElement);

            m_ErrorElement = new TilePaletteClipboardErrorElement();
            m_ErrorElement.style.display = DisplayStyle.None;
            m_ErrorElement.style.visibility = Visibility.Hidden;
            m_ErrorElement.SetEmptyPaletteText();
            Add(m_ErrorElement);

            m_FirstUserElement = new TilePaletteClipboardFirstUserElement();
            m_FirstUserElement.style.display = DisplayStyle.None;
            m_FirstUserElement.style.visibility = Visibility.Hidden;
            Add(m_FirstUserElement);

            ScrollView ms = new ScrollView();
        }

        private void UnlockChanged(bool unlocked)
        {
            clipboardUnlockedChanged?.Invoke(unlocked);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                // Delay AttachToPanel if Editor is entering playmode
                EditorApplication.delayCall += AttachToPanel;
            }
            else
            {
                AttachToPanel();
            }
        }

        private void AttachToPanel()
        {
            if (m_TilePaletteClipboard == null)
            {
                m_TilePaletteClipboard = ScriptableObject.CreateInstance<GridPaintPaletteClipboard>();
                m_TilePaletteClipboard.hideFlags = HideFlags.HideAndDontSave;
                m_TilePaletteClipboard.unlockedChanged += UnlockChanged;
                m_TilePaletteClipboard.unlocked = false;
                m_TilePaletteClipboard.attachedVisualElement = this;

                var guiRect = new Rect(0, 0, layout.width, layout.height);
                m_TilePaletteClipboard.guiRect = guiRect;

                CheckPaletteState(m_TilePaletteClipboard.paletteInstance);
            }

            RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            RegisterCallback<ValidateCommandEvent>(OnValidateCommandEvent);
            RegisterCallback<ExecuteCommandEvent>(OnExecuteCommandEvent);
            RegisterCallback<WheelEvent>(OnWheelEvent);
            RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
            RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
            RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
            RegisterCallback<PointerEnterEvent>(OnPointerEnterEvent);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
            m_ClipboardImageElement.RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
            m_ClipboardImageElement.RegisterCallback<KeyUpEvent>(OnKeyUpEvent);

            RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
            RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

            generateVisualContent += GenerateVisualContent;

            GridPaintingState.beforePaletteChanged += BeforePaletteChanged;
            GridPaintingState.paletteChanged += PaletteChanged;
        }

        private void GenerateVisualContent(MeshGenerationContext obj)
        {
            if (m_ClipboardImageElement.visible)
            {
                var texture = m_TilePaletteClipboard.RenderTexture();
                EditorApplication.delayCall += () =>
                {
                    m_ClipboardImageElement.image = texture;
                };
            }
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            UnregisterCallback<ValidateCommandEvent>(OnValidateCommandEvent);
            UnregisterCallback<ExecuteCommandEvent>(OnExecuteCommandEvent);
            UnregisterCallback<WheelEvent>(OnWheelEvent);
            UnregisterCallback<PointerDownEvent>(OnPointerDownEvent);
            UnregisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
            UnregisterCallback<PointerUpEvent>(OnPointerUpEvent);
            UnregisterCallback<PointerEnterEvent>(OnPointerEnterEvent);
            UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
            m_ClipboardImageElement.UnregisterCallback<KeyDownEvent>(OnKeyDownEvent);
            m_ClipboardImageElement.UnregisterCallback<KeyUpEvent>(OnKeyUpEvent);

            UnregisterCallback<DragEnterEvent>(OnDragEnterEvent);
            UnregisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            UnregisterCallback<DragExitedEvent>(OnDragExitedEvent);
            UnregisterCallback<DragLeaveEvent>(OnDragLeaveEvent);

            generateVisualContent -= GenerateVisualContent;

            if (m_TilePaletteClipboard != null)
                m_TilePaletteClipboard.unlockedChanged -= UnlockChanged;
            GridPaintingState.beforePaletteChanged -= BeforePaletteChanged;
            GridPaintingState.paletteChanged -= PaletteChanged;

            Cleanup();
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            var guiRect = new Rect(0, 0, layout.width, layout.height);
            m_TilePaletteClipboard.guiRect = guiRect;
            m_ClipboardImageElement.image = m_TilePaletteClipboard.RenderTexture();
        }

        private void OnExecuteCommandEvent(ExecuteCommandEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleExecuteCommandEvent(evt);
        }

        private void OnValidateCommandEvent(ValidateCommandEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleValidateCommandEvent(evt);
        }

        private void OnWheelEvent(WheelEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleWheelEvent(evt.delta, evt.mousePosition, evt.shiftKey);
            evt.StopPropagation();
            MarkDirtyRepaint();
        }

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandlePointerDownEvent(evt
                , evt.button
                , evt.altKey
                , evt.ctrlKey
                , evt.localPosition);
            MarkDirtyRepaint();
        }

        private void OnPointerMoveEvent(PointerMoveEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandlePointerMoveEvent(evt
                , evt.button
                , evt.altKey
                , evt.localPosition
                , evt.deltaPosition);
            MarkDirtyRepaint();
        }

        private void OnPointerUpEvent(PointerUpEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            if (onBrushPicked != null && m_TilePaletteClipboard != null)
                m_TilePaletteClipboard.onBrushPicked += onBrushPicked;
            m_TilePaletteClipboard.HandlePointerUpEvent(evt);
            if (onBrushPicked != null && m_TilePaletteClipboard != null)
                m_TilePaletteClipboard.onBrushPicked -= onBrushPicked;
            MarkDirtyRepaint();
        }

        private void OnPointerEnterEvent(PointerEnterEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_ClipboardImageElement.Focus();
            m_TilePaletteClipboard.HandlePointerEnterEvent(evt);
        }

        private void OnPointerLeaveEvent(PointerLeaveEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandlePointerLeaveEvent(evt);
        }

        private void OnKeyDownEvent(KeyDownEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleKeyDownEvent(evt);
            MarkDirtyRepaint();
        }

        private void OnKeyUpEvent(KeyUpEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleKeyUpEvent();
            MarkDirtyRepaint();
        }

        private void OnDragEnterEvent(DragEnterEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleDragEnterEvent(evt);
            CheckPaletteState(m_TilePaletteClipboard.paletteInstance);
        }

        private void OnDragUpdatedEvent(DragUpdatedEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleDragUpdatedEvent(evt);
            MarkDirtyRepaint();
        }

        private void OnDragPerformEvent(DragPerformEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleDragPerformEvent(evt);
            CheckPaletteState(m_TilePaletteClipboard.paletteInstance);
            MarkDirtyRepaint();
        }

        private void OnDragLeaveEvent(DragLeaveEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleDragLeaveEvent(evt);
            CheckPaletteState(m_TilePaletteClipboard.paletteInstance);
            MarkDirtyRepaint();
        }

        private void OnDragExitedEvent(DragExitedEvent evt)
        {
            if (m_TilePaletteClipboard == null)
                return;

            m_TilePaletteClipboard.HandleDragExitedEvent(evt);
            MarkDirtyRepaint();
        }

        /// <summary>
        /// Handles cleanup for the Tile Palette Clipboard.
        /// </summary>
        private void Cleanup()
        {
            UnityEngine.Object.DestroyImmediate(m_TilePaletteClipboard);
            m_TilePaletteClipboard = null;
        }

        private void BeforePaletteChanged()
        {
            if (m_TilePaletteClipboard == null)
                return;
            m_TilePaletteClipboard.OnBeforePaletteSelectionChanged();
        }

        private void PaletteChanged(GameObject palette)
        {
            if (m_TilePaletteClipboard == null)
                return;
            m_TilePaletteClipboard.OnAfterPaletteSelectionChanged();
            CheckPaletteState(palette);
        }

        private void CheckPaletteState(GameObject palette)
        {
            if (palette == null && GridPaintingState.palettes.Count == 0)
            {
                m_ClipboardImageElement.style.display = DisplayStyle.None;
                m_ClipboardImageElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.style.display = DisplayStyle.None;
                m_ErrorElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.ClearText();
                m_FirstUserElement.style.display = DisplayStyle.Flex;
                m_FirstUserElement.style.visibility = Visibility.Visible;
            }
            else if (palette == null && GridPaintingState.palettes.Count > 0)
            {
                m_ClipboardImageElement.style.display = DisplayStyle.None;
                m_ClipboardImageElement.style.visibility = Visibility.Hidden;
                m_FirstUserElement.style.display = DisplayStyle.None;
                m_FirstUserElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.style.display = DisplayStyle.Flex;
                m_ErrorElement.style.visibility = Visibility.Visible;
                m_ErrorElement.SetInvalidPaletteText();
            }
            else if (m_TilePaletteClipboard.activeDragAndDrop && m_TilePaletteClipboard.invalidDragAndDrop)
            {
                m_ClipboardImageElement.style.display = DisplayStyle.None;
                m_ClipboardImageElement.style.visibility = Visibility.Hidden;
                m_FirstUserElement.style.display = DisplayStyle.None;
                m_FirstUserElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.style.display = DisplayStyle.Flex;
                m_ErrorElement.style.visibility = Visibility.Visible;
                m_ErrorElement.SetInvalidDragAndDropText();
            }
            else if (palette.GetComponent<Grid>() == null)
            {
                m_ClipboardImageElement.style.display = DisplayStyle.None;
                m_ClipboardImageElement.style.visibility = Visibility.Hidden;
                m_FirstUserElement.style.display = DisplayStyle.None;
                m_FirstUserElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.style.display = DisplayStyle.Flex;
                m_ErrorElement.style.visibility = Visibility.Visible;
                m_ErrorElement.SetInvalidGridText();
            }
            else if (m_TilePaletteClipboard.showNewEmptyClipboardInfo)
            {
                m_ClipboardImageElement.style.display = DisplayStyle.None;
                m_ClipboardImageElement.style.visibility = Visibility.Hidden;
                m_FirstUserElement.style.display = DisplayStyle.None;
                m_FirstUserElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.style.display = DisplayStyle.Flex;
                m_ErrorElement.style.visibility = Visibility.Visible;
                m_ErrorElement.SetEmptyPaletteText();
            }
            else
            {
                m_ErrorElement.style.display = DisplayStyle.None;
                m_ErrorElement.style.visibility = Visibility.Hidden;
                m_ErrorElement.ClearText();
                m_FirstUserElement.style.display = DisplayStyle.None;
                m_FirstUserElement.style.visibility = Visibility.Hidden;
                m_ClipboardImageElement.style.display = DisplayStyle.Flex;
                m_ClipboardImageElement.style.visibility = Visibility.Visible;
            }

            if (m_Window != null)
                m_Window.Repaint();
        }
    }
}
