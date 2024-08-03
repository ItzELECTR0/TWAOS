// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {

    public abstract class GlyphOrTextBase : UnityEngine.MonoBehaviour {

        protected abstract string textString { get; set; }

        public abstract void ShowText(string text);

        public abstract void ShowGlyph(object glyph);

        public virtual void Hide() {
            Hide(TypeFlags.All);
        }
        protected abstract void Hide(TypeFlags flags);

        [System.Flags]
        protected enum TypeFlags {
            None = 0,
            Glyph = 1,
            Text = 2,
            All = ~0
        }
    }

    public abstract class GlyphOrTextBase<TGlyphComponent, TGlyphGraphic, TTextComponent> : GlyphOrTextBase
        where TGlyphComponent : UnityEngine.Behaviour
        where TGlyphGraphic : class
        where TTextComponent : UnityEngine.Behaviour
    {
        [UnityEngine.SerializeField]
        private TTextComponent _textComponent;
        [UnityEngine.SerializeField]
        private TGlyphComponent _glyphComponent;

        public TTextComponent textComponent {
            get {
                return _textComponent;
            }
            set {
                _textComponent = value;
            }
        }

        public TGlyphComponent glyphComponent {
            get {
                return _glyphComponent;
            }
            set {
                _glyphComponent = value;
            }
        }

        protected abstract TGlyphGraphic glyphGraphic { get; set; }

        public override void ShowText(string text) {
            if (_textComponent == null) return;
            if (!string.Equals(textString, text, System.StringComparison.Ordinal)) {
                textString = text;
            }
            if (!_textComponent.gameObject.activeSelf) {
                _textComponent.gameObject.SetActive(true);
                if (!this.gameObject.activeSelf) {
                    this.gameObject.SetActive(true);
                }
            }
            Hide(TypeFlags.Glyph);
        }

        public override void ShowGlyph(object glyph) {
            if (glyph != null && !(glyph is TGlyphGraphic)) {
                UnityEngine.Debug.LogError("Rewired: Glyph does not implement " + typeof(TGlyphGraphic).Name + ".");
                return;
            }
            ShowGlyph((TGlyphGraphic)glyph);
        }
        public virtual void ShowGlyph(TGlyphGraphic glyph) {
            if (_glyphComponent == null) return;
            if (this.glyphGraphic != glyph) {
                this.glyphGraphic = glyph;
            }
            if (!_glyphComponent.gameObject.activeSelf) {
                _glyphComponent.gameObject.SetActive(true);
                if (!this.gameObject.activeSelf) {
                    this.gameObject.SetActive(true);
                }
            }
            Hide(TypeFlags.Text);
        }

        protected override void Hide(TypeFlags flags) {
            if (_textComponent != null) {
                if ((flags & TypeFlags.Text) != 0) {
                    if (_textComponent.gameObject.activeSelf) {
                        _textComponent.gameObject.SetActive(false);
                    }
                }
            }
            if (_glyphComponent != null) {
                if ((flags & TypeFlags.Glyph) != 0) {
                    if (_glyphComponent.gameObject.activeSelf) {
                        _glyphComponent.gameObject.SetActive(false);
                    }
                }
            }
            if ((_glyphComponent == null || !_glyphComponent.gameObject.activeSelf) &&
                (_textComponent == null || !_textComponent.gameObject.activeSelf)
            ) {
                this.gameObject.SetActive(false);
            }
        }
    }
}
