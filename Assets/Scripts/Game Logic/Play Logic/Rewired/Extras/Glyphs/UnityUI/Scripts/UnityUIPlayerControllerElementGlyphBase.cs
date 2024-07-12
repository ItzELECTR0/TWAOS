// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {
    using System;
    using System.Collections.Generic;
    using Rewired;

    /// <summary>
    /// Base class for a Unity UI Player Controller Element Glyph component.
    /// Displays glyphs / text for a specific Action for a Player.
    /// </summary>
    public abstract class UnityUIPlayerControllerElementGlyphBase : UnityUIControllerElementGlyphBase {

        [UnityEngine.Tooltip("Optional reference to an object that defines options. If blank, the global default options will be used.")]
        [UnityEngine.SerializeField]
        private ControllerElementGlyphSelectorOptionsSOBase _options;

        [UnityEngine.Tooltip("The range of the Action for which to show glyphs / text. This determines whether to show the glyph for an axis-type Action " +
            "(ex: Move Horizontal), or the positive/negative pole of an Action (ex: Move Right). For button-type Actions, Full and Positive are equivalent.")]
        [UnityEngine.SerializeField]
        private AxisRange _actionRange = AxisRange.Full;

        [UnityEngine.Tooltip("Optional parent Transform of the first group of instantiated glyph / text objects. " +
            "If an axis-type Action is bound to multiple elements, the glyphs bound to the negative pole of the Action will be instantiated under this Transform. " +
            "This allows you to separate negative and positive groups in order to stack glyph groups horizontally or vertically, for example. " +
            "If an Action is only bound to one element, the glyph will be instantiated under this transform. " +
            "If blank, objects will be created as children of this object's Transform.")]
        [UnityEngine.SerializeField]
        private UnityEngine.Transform _group1;

        [UnityEngine.Tooltip("Optional parent Transform of the second group of instantiated glyph / text objects. " +
            "If an axis-type Action is bound to multiple elements, the glyphs bound to the positive pole of the Action will be instantiated under this Transform. " +
            "This allows you to separate negative and positive groups in order to stack glyph groups horizontally or vertically, for example. " +
            "If an Action is only bound to one element, the glyph will be instantiated under group1 instead. " +
            "If blank, objects will be created as children of either group1 if set or the object's Transform.")]
        [UnityEngine.SerializeField]
        private UnityEngine.Transform _group2;

        [NonSerialized]
        private List<ActionElementMap> _tempAems = new List<ActionElementMap>();

        [NonSerialized]
        private List<ActionElementMap> _tempCombinedElementAems = new List<ActionElementMap>();

        [NonSerialized]
        private readonly List<GlyphOrTextObject> _group1Objects = new List<GlyphOrTextObject>();

        [NonSerialized]
        private readonly List<GlyphOrTextObject> _group2Objects = new List<GlyphOrTextObject>();

        /// <summary>
        /// Optional reference to an object that defines options.
        /// If blank, the global default options will be used.
        /// </summary>
        public virtual ControllerElementGlyphSelectorOptionsSOBase options {
            get {
                return _options;
            }
            set {
                _options = value;
                RequireRebuild();
            }
        }

        /// <summary>
        /// The Player id.
        /// </summary>
        public abstract int playerId { get; set; }

        /// <summary>
        /// The Action id.
        /// </summary>
        public abstract int actionId { get; set; }

        /// <summary>
        /// The range of the Action for which to show glyphs / text. 
        /// This determines whether to show the glyph for an axis-type Action (ex: Move Horizontal), 
        /// or the positive/negative pole of an Action (ex: Move Right).
        /// For button-type Actions, Full and Positive are equivalent.
        /// </summary>
        public virtual AxisRange actionRange {
            get {
                return _actionRange;
            }
            set {
                _actionRange = value;
            }
        }

        /// <summary>
        /// Optional parent Transform of the first group of instantiated glyph / text objects.
        /// If an axis-type Action is bound to multiple elements, the glyphs bound to the negative pole of the Action will be instantiated under this Transform.
        /// This allows you to separate negative and positive groups in order to stack glyph groups horizontally or vertically, for example.
        /// If an Action is only bound to one element, the glyph will be instantiated under this transform.
        /// If blank, objects will be created as children of this object's Transform.
        /// </summary>
        public virtual UnityEngine.Transform group1 {
            get {
                return _group1;
            }
            set {
                _group1 = value;
                RequireRebuild();
            }
        }

        /// <summary>
        /// Optional parent Transform of the second group of instantiated glyph / text objects.
        /// If an axis-type Action is bound to multiple elements, the glyphs bound to the positive pole of the Action will be instantiated under this Transform.
        /// This allows you to separate negative and positive groups in order to stack glyph groups horizontally or vertically, for example.
        /// If an Action is only bound to one element, the glyph will be instantiated under group1 instead.
        /// If blank,If blank, objects will be created as children of either group1 if set or the object's Transform.
        /// </summary>
        public virtual UnityEngine.Transform group2 {
            get {
                return _group2;
            }
            set {
                _group2 = value;
                RequireRebuild();
            }
        }

        protected virtual bool isMousePrioritizedOverKeyboard {
            get {
                ControllerType controllerType;
                for (int i = 0; TryGetControllerTypeOrder(i, out controllerType); i++) {
                    if (controllerType == ControllerType.Mouse) return true;
                    if (controllerType == ControllerType.Keyboard) return false;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the controller type order at the index.
        /// Lower values are evaulated first.
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <returns>Controller type at index.</returns>
        protected virtual bool TryGetControllerTypeOrder(int index, out ControllerType controllerType) {
            return GetOptionsOrDefault().TryGetControllerTypeOrder(index, out controllerType);
        }

        protected override void Update() {
            base.Update();
            if (!ReInput.isReady) return;

            ActionElementMap aem1;
            ActionElementMap aem2;

            if (!GlyphTools.TryGetActionElementMaps(
                playerId,
                actionId,
                actionRange,
                GetOptionsOrDefault(),
                _tempAems,
                out aem1,
                out aem2
            )) {
                Hide();
                return;
            }

            // Show bindings
            if (aem1 != null && aem2 != null) { // two split axis bindings
                ShowSplitAxisBindings(aem1, aem2);
            } else if (aem1 != null) {
                ShowBinding(aem1);
            } else if (aem2 != null) {
                ShowBinding(aem2);
            }
        }

        protected override void ClearObjects() {
            _group1Objects.Clear();
            _group2Objects.Clear();
            base.ClearObjects();
        }

        protected virtual bool ShowBinding(ActionElementMap actionElementMap) {
            if (actionElementMap == null) return false;

            // Display glyph or text for a single Action Element Map

            int count = ShowGlyphsOrText(actionElementMap, GetObjectGroupTransform(0), _group1Objects);

            EvaluateObjectVisibility();

            return count > 0;
        }

        protected virtual bool ShowSplitAxisBindings(ActionElementMap negativeAem, ActionElementMap positiveAem) {
            if (negativeAem == null && positiveAem == null) return false;

            // Display glyph or text for up to two Action Element Maps

            int count = 0;

            // Handle special combined glyphs (D-Pad Left + D-Pad Right = D-Pad Horizontal)
            if (negativeAem != null && positiveAem != null) {
                _tempCombinedElementAems.Clear();
                _tempCombinedElementAems.Add(negativeAem);
                _tempCombinedElementAems.Add(positiveAem);
                count = ShowGlyphsOrText(_tempCombinedElementAems, GetObjectGroupTransform(0), _group1Objects);
            }
            
            // Positive and negative bindings
            if (count == 0) {
                count += ShowGlyphsOrText(negativeAem, GetObjectGroupTransform(0), _group1Objects);
                count += ShowGlyphsOrText(positiveAem, GetObjectGroupTransform(1), _group2Objects);
            }

            EvaluateObjectVisibility();

            return count > 0;
        }

        protected override void EvaluateObjectVisibility() {
            base.EvaluateObjectVisibility();
            
            // Enable / disable groups
            var transform1 = GetObjectGroupTransform(0);
            var transform2 = GetObjectGroupTransform(1);

            if (transform1 == transform2) {
                EvaluateObjectVisibility(transform1);
            } else {
                EvaluateObjectVisibility(transform1, _group1Objects);
                EvaluateObjectVisibility(transform2, _group2Objects);
            }
        }

        protected virtual int ShowGlyphsOrText(IList<ActionElementMap> bindings, UnityEngine.Transform parent, List<GlyphOrTextObject> objects) {
            if (bindings == null) return 0;

            // Show combined glyph or text for multiple bindings.
            // This does not support modifier keys.

            object glyph;
            string name;

            if (IsAllowed(AllowedTypes.Glyphs) && ActionElementMap.TryGetCombinedElementIdentifierGlyph(bindings, out glyph)) {
                if (!CreateObjectsAsNeeded(parent, objects, 1)) return 0;
                objects[0].ShowGlyph(glyph);
                return 1;
            } else if (IsAllowed(AllowedTypes.Text) && ActionElementMap.TryGetCombinedElementIdentifierName(bindings, out name)) {
                if (!CreateObjectsAsNeeded(parent, objects, 1)) return 0;
                objects[0].ShowText(name);
                return 1;
            }

            return 0;
        }

        protected override void Hide() {
            base.Hide();
            if (_group1 != null && _group1 != this.transform) _group1.gameObject.SetActive(false);
            if (_group2 != null && _group2 != this.transform) _group2.gameObject.SetActive(false);
        }

        protected virtual UnityEngine.Transform GetObjectGroupTransform(int groupIndex) {
            if ((uint)groupIndex > 1u) throw new ArgumentOutOfRangeException();
            switch (groupIndex) {
                case 0: return _group1 != null ? _group1 : this.transform;
                case 1:
                    if (_group1 == null) return this.transform; // ignore 2 if 1 not set
                    if (_group2 != null) return _group2;
                    if (_group1 != null) return _group1;
                    return this.transform;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the Controller Element Glyph Options if set, otherwise the default options.
        /// </summary>
        /// <returns>The Controller Element Glyph Options if set, otherwise the default options.</returns>
        protected virtual ControllerElementGlyphSelectorOptions GetOptionsOrDefault() {
            if (_options != null && _options.options == null) {
                UnityEngine.Debug.LogError("Rewired: Options missing on " + typeof(ControllerElementGlyphSelectorOptions).Name + ". Global default options will be used instead.");
                return ControllerElementGlyphSelectorOptions.defaultOptions;
            }
            return _options != null ? _options.options : ControllerElementGlyphSelectorOptions.defaultOptions;
        }
    }
}
