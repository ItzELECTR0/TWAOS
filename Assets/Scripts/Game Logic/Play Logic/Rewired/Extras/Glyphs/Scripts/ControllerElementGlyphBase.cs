// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System;
    using System.Collections.Generic;
    using Rewired;

    /// <summary>
    /// Base class for a Controller Element Glyph component.
    /// </summary>
    public abstract class ControllerElementGlyphBase : UnityEngine.MonoBehaviour {

        [UnityEngine.Tooltip("If set, when glyph/text objects are created, they will be instantiated from this prefab. If left blank, the global default prefab will be used.")]
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject _glyphOrTextPrefab;

        [UnityEngine.Tooltip("Determines what types of objects are allowed.")]
        [UnityEngine.SerializeField]
        private AllowedTypes _allowedTypes;

        [NonSerialized]
        private readonly List<GlyphOrTextObject> _entries = new List<GlyphOrTextObject>();

        [NonSerialized]
        private List<object> _tempGlyphs = new List<object>();

        [NonSerialized]
        private UnityEngine.GameObject _lastGlyphOrTextPrefab;

        /// <summary>
        /// If set, when glyph/text objects are created, they will be instantiated from this prefab.
        /// If left blank, the global default prefab will be used.
        /// </summary>
        public virtual UnityEngine.GameObject glyphOrTextPrefab {
            get {
                return _glyphOrTextPrefab;
            }
            set {
                _glyphOrTextPrefab = value;
                RequireRebuild();
            }
        }

        /// <summary>
        /// Determines what types of objects are allowed.
        /// </summary>
        public virtual AllowedTypes allowedTypes {
            get {
                return _allowedTypes;
            }
            set {
                _allowedTypes = value;
            }
        }

        /// <summary>
        /// Gets the list of entries.
        /// </summary>
        protected List<GlyphOrTextObject> entries {
            get {
                return _entries;
            }
        }

        protected virtual void Awake() {
        }

        protected virtual void Start() {
        }

        protected virtual void OnDestroy() {
#if UNITY_EDITOR
            ReInput.EditorRecompileEvent -= OnEditorRecompile;
#endif
        }

        protected virtual void OnEnable() {
#if UNITY_EDITOR
            ReInput.EditorRecompileEvent -= OnEditorRecompile;
            ReInput.EditorRecompileEvent += OnEditorRecompile;
#endif
        }

        protected virtual void OnDisable() {
        }

        protected virtual void Update() {
            if (!ReInput.isReady) return;

            // Monitor for prefab changes to automatically rebuild
            if (_lastGlyphOrTextPrefab != GetGlyphOrTextPrefabOrDefault()) {
                _lastGlyphOrTextPrefab = GetGlyphOrTextPrefabOrDefault();
                RequireRebuild();
            }

#if UNITY_EDITOR
            HandleInspectorValueChanges();
#endif
        }

        /// <summary>
        /// Clears all instantiated glyph / text objects so they can be recreated from the prefab.
        /// </summary>
        public virtual void RequireRebuild() {
            ClearObjects();
        }

        /// <summary>
        /// Clears all instantiated glyph / text objects objects.
        /// </summary>
        protected virtual void ClearObjects() {
            for (int i = 0; i < _entries.Count; i++) {
                if (_entries[i] == null) continue;
                _entries[i].Destroy();
            }
            _entries.Clear();
            Hide();
        }

        /// <summary>
        /// Hides glyph / text objects that are no longer active.
        /// </summary>
        protected virtual void EvaluateObjectVisibility() {
            // Disable all fields except those that are visible
            for (int i = 0; i < _entries.Count; i++) {
                if (_entries[i] == null) continue;
                _entries[i].HideIfIdle();
            }
        }
        /// <summary>
        /// Hides all glyph / text objects that are no longer active.
        /// If the parent transform is on a different GameObject, the parent
        /// GameObject will be hidden if all of its glyphs / text objects
        /// are inactive.
        /// </summary>
        /// <param name="transform">The parent transform of the objects.</param>
        protected virtual void EvaluateObjectVisibility(UnityEngine.Transform transform) {
            EvaluateObjectVisibility(transform, _entries);
        }
        /// <summary>
        /// Hides glyph / text objects that are no longer active in the specified list.
        /// If the parent transform is on a different GameObject, the parent
        /// GameObject will be hidden if all of its glyphs / text objects
        /// are inactive.
        /// </summary>
        /// <param name="transform">The parent transform of the objects.</param>
        /// <param name="entries">The list of entries.</param>
        protected virtual void EvaluateObjectVisibility(UnityEngine.Transform transform, List<GlyphOrTextObject> entries) {
            if (transform == this.transform) return; // do not modify visibility of this GameObject
            bool isVisible = false;
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].isVisible) {
                    isVisible = true;
                }
            }
            if (transform.gameObject.activeSelf != isVisible) {
                transform.gameObject.SetActive(isVisible);
            }
        }

        /// <summary>
        /// Displays glyphs or text for an Action Element Map and adds them to the specified entries list.
        /// </summary>
        /// <param name="actionElementMap">The action element map.</param>
        /// <param name="parent">The parent transform of the objects.</param>
        /// <param name="entries">The list of entries.</param>
        /// <returns>The number of glyphs or text objects that were displayed.</returns>
        protected virtual int ShowGlyphsOrText(ActionElementMap actionElementMap, UnityEngine.Transform parent, List<GlyphOrTextObject> entries) {
            _tempGlyphs.Clear();

            int count = 0;

            if (IsAllowed(AllowedTypes.Glyphs) && GetGlyphs(actionElementMap, _tempGlyphs) > 0) {
                if (!CreateObjectsAsNeeded(parent, entries, _tempGlyphs.Count)) return 0;
                for (int i = 0; i < _tempGlyphs.Count; i++) {
                    entries[i].ShowGlyph(_tempGlyphs[i]);
                }
                count += _tempGlyphs.Count;
            } else if (IsAllowed(AllowedTypes.Text) && actionElementMap != null) { // fall back to text
                if (!CreateObjectsAsNeeded(parent, entries, 1)) return 0;
                entries[0].ShowText(actionElementMap.elementIdentifierName);
                count += 1;
            }

            return count;
        }
        /// <summary>
        /// Displays glyphs or text for an Action Element Map.
        /// </summary>
        /// <param name="actionElementMap">The action element map.</param>
        /// <returns>The number of glyphs or text objects that were displayed.</returns>
        protected virtual int ShowGlyphsOrText(ActionElementMap actionElementMap) {
            return ShowGlyphsOrText(actionElementMap, this.transform, _entries);
        }
        /// <summary>
        /// Displays glyphs or text for an Action Element Map and adds them to the specified entries list.
        /// </summary>
        /// <param name="elementIdentifier">The element identifier.</param>
        /// <param name="parent">The parent transform of the objects.</param>
        /// <param name="entries">The list of entries.</param>
        /// <returns>The number of glyphs or text objects that were displayed.</returns>
        protected virtual int ShowGlyphsOrText(ControllerElementIdentifier elementIdentifier, AxisRange axisRange, UnityEngine.Transform parent, List<GlyphOrTextObject> entries) {
            if (elementIdentifier == null) return 0;

            object glyph;
            int count = 0;

            if (IsAllowed(AllowedTypes.Glyphs) && (glyph = elementIdentifier.GetGlyph(axisRange)) != null) {
                if (!CreateObjectsAsNeeded(parent, entries, 1)) return 0;
                entries[0].ShowGlyph(glyph);
                count += 1;
            } else if (IsAllowed(AllowedTypes.Text)) { // fall back to text
                if (!CreateObjectsAsNeeded(parent, entries, 1)) return 0;
                entries[0].ShowText(elementIdentifier.GetDisplayName(axisRange));
                count += 1;
            }

            return count;
        }
        /// <summary>
        /// Displays glyphs or text for an Action Element Map.
        /// </summary>
        /// <param name="elementIdentifier">The element identifier.</param>
        /// <param name="axisRange">The axis range.</param>
        /// <returns>The number of glyphs or text objects that were displayed.</returns>
        protected virtual int ShowGlyphsOrText(ControllerElementIdentifier elementIdentifier, AxisRange axisRange) {
            return ShowGlyphsOrText(elementIdentifier, axisRange, this.transform, _entries);
        }

        /// <summary>
        /// Hides all glyph / text objects.
        /// </summary>
        protected virtual void Hide() {
            for (int i = 0; i < _entries.Count; i++) {
                if (_entries[i] == null) continue;
                _entries[i].Hide();
            }
        }

        /// <summary>
        /// Gets the glyph or text prefab if set, otherwise the default prefab.
        /// </summary>
        /// <returns>Glyph or text prefab if set, otherwise the default prefab.</returns>
        protected virtual UnityEngine.GameObject GetGlyphOrTextPrefabOrDefault() {
            return _glyphOrTextPrefab != null ? _glyphOrTextPrefab : GetDefaultGlyphOrTextPrefab();
        }

        /// <summary>
        /// Gets the default glyph or text prefab.
        /// </summary>
        /// <returns>The default glyph or text prefab.</returns>
        protected abstract UnityEngine.GameObject GetDefaultGlyphOrTextPrefab();

        /// <summary>
        /// Creates glyph / text objects on-demand.
        /// </summary>
        /// <param name="parent">The parent transform of the objects.</param>
        /// <param name="entries">The list of entries.</param>
        /// <param name="count">The number of objects required.</param>
        /// <returns>True if objects were created, false if an error occurred.</returns>
        protected virtual bool CreateObjectsAsNeeded(UnityEngine.Transform parent, List<GlyphOrTextObject> entries, int count) {
            if (count <= 0) return false;
            UnityEngine.GameObject prefab = GetGlyphOrTextPrefabOrDefault();
            if (prefab == null) {
                UnityEngine.Debug.LogError("Rewired: Default prefab is null.");
                return false;
            }
            if (entries == null) return false;
            int existingCount = entries.Count;
            UnityEngine.GameObject instance;
            GlyphOrTextBase component;
            for (int i = existingCount; i < count; i++) {
                instance = UnityEngine.GameObject.Instantiate(prefab);
                instance.name = "Object";
                instance.hideFlags = UnityEngine.HideFlags.DontSave;
                instance.transform.SetParent(parent, false);
                component = instance.GetComponent<GlyphOrTextBase>();
                if (component == null) {
                    UnityEngine.Debug.LogError("Rewired: Prefab does not contain a " + typeof(GlyphOrTextBase) + " component.");
                    UnityEngine.Object.Destroy(instance);
                    continue;
                }
                GlyphOrTextObject obj = new GlyphOrTextObject(component);
                entries.Add(obj);
                if (entries != _entries) {
                    _entries.Add(obj);
                }
            }
            return true;
        }

        /// <summary>
        /// Is the type allowed?
        /// </summary>
        /// <param name="allowedType">Allowed type.</param>
        /// <returns>True if the type is allowed, False otherwise.</returns>
        protected virtual bool IsAllowed(AllowedTypes allowedType) {
            if (_allowedTypes == AllowedTypes.All) return true;
            return allowedType == _allowedTypes;
        }

#if UNITY_EDITOR

        [NonSerialized]
        private System.Action _inspectorActions;

        protected virtual void OnValidate() {
            _inspectorActions = null; // prevent buffering of actions in edit mode
            CheckInspectorValues(ref _inspectorActions);
        }

        protected virtual void CheckInspectorValues(ref System.Action actions) {
        }

        protected virtual void HandleInspectorValueChanges() {
            System.Action actions = _inspectorActions;
            _inspectorActions = null;
            if (actions != null) actions();
        }

        protected virtual void OnEditorRecompile() {
            ClearObjects();
        }
#endif

        // Static

        /// <summary>
        /// Gets all glyphs for an Action Element Map including modifier glyphs.
        /// If any glyphs are missing, for example, one modifier key has no glyph, no glyphs will be returned.
        /// If multiple glyphs are returned, the list will be ordered modifier key glyphs first, primary glyph last.
        /// </summary>
        /// <param name="actionElementMap">The Action Element Map.</param>
        /// <param name="results">The list of glyph results.</param>
        /// <returns>The number of glyphs returned.</returns>
        protected static int GetGlyphs(ActionElementMap actionElementMap, List<object> results) {
            if (actionElementMap == null) return 0;
            int count = 1;
            if (actionElementMap.hasModifiers) {
                if (actionElementMap.modifierKey1 != ModifierKey.None) count += 1;
                if (actionElementMap.modifierKey2 != ModifierKey.None) count += 1;
                if (actionElementMap.modifierKey3 != ModifierKey.None) count += 1;
            }
            if (actionElementMap.elementIdentifierGlyphCount != count) return 0;
            actionElementMap.GetElementIdentifierGlyphs(results);
            return count;
        }

        // Classes

        /// <summary>
        /// Represents a glyph / text object.
        /// </summary>
        protected class GlyphOrTextObject {

            private GlyphOrTextBase _glyphOrText;
            private int _frame;
            private bool _isVisible;

            /// <summary>
            /// Determines if the glyph / text is visible.
            /// </summary>
            public virtual bool isVisible {
                get {
                    return _isVisible;
                }
                protected set {
                    _isVisible = value;
                }
            }

            /// <summary>
            /// The glyph / text object.
            /// </summary>
            public GlyphOrTextBase glyphOrText {
                get {
                    return _glyphOrText;
                }
                set {
                    _glyphOrText = value;
                }
            }

            /// <summary>
            /// Creates an instance.
            /// </summary>
            /// <param name="glyphOrText">The glyph / text object.</param>
            public GlyphOrTextObject(GlyphOrTextBase glyphOrText) {
                _glyphOrText = glyphOrText;
            }

            /// <summary>
            /// Displays the specified glyph.
            /// </summary>
            /// <param name="glyph">Glyph to display.</param>
            public virtual void ShowGlyph(object glyph) {
                if (_glyphOrText == null) return;
                _glyphOrText.ShowGlyph(glyph);
                _frame = UnityEngine.Time.frameCount;
                _isVisible = true;
            }

            /// <summary>
            /// Displays the specified text.
            /// </summary>
            /// <param name="text">The text string to display.</param>
            public virtual void ShowText(string text) {
                if (_glyphOrText == null) return;
                _glyphOrText.ShowText(text);
                _frame = UnityEngine.Time.frameCount;
                _isVisible = true;
            }

            /// <summary>
            /// Hides the glyph / text.
            /// </summary>
            public virtual void Hide() {
                if (_glyphOrText == null) return;
                if (!_isVisible) return;
                _glyphOrText.Hide();
                _isVisible = false;
            }

            /// <summary>
            /// Hides the glyph / text if it hasn't been updated this frame.
            /// </summary>
            public virtual void HideIfIdle() {
                if (_frame == UnityEngine.Time.frameCount) return;
                Hide();
            }

            /// <summary>
            /// Destroys glyph / text object.
            /// </summary>
            public virtual void Destroy() {
                if (_glyphOrText == null) return;
                UnityEngine.Object.Destroy(_glyphOrText.gameObject);
                _glyphOrText = null;
                _isVisible = false;
            }
        }

        /// <summary>
        /// The type of objects that are allowed.
        /// </summary>
        public enum AllowedTypes {
            /// <summary>
            /// All types are allowed. Glyphs will be displayed, or text if no glyph is available.
            /// </summary>
            All = 0,
            /// <summary>
            /// Only glyphs will be displayed.
            /// </summary>
            Glyphs = 1,
            /// <summary>
            /// Only text will be displayed.
            /// </summary>
            Text = 2
        }
    }
}
