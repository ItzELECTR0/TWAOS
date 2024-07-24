// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_6000 || UNITY_6000_0_OR_NEWER
#define UNITY_2020_PLUS
#endif

#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2018_PLUS
#define REWIRED_SUPPORTS_TMPRO
#endif

#if REWIRED_SUPPORTS_TMPRO

#if UNITY_2020_PLUS
// A version of Text Mesh Pro or Unity UI that uses asset version 1.1.0
// may or may not be installed in this version of Unity.
// Since there are no preprocessor conditionals defined for the
// Unity UI / TMPro version, the only choice is to use reflection
// to support classes that may or may not exist in some projects.
#define MAY_SUPPORT_TMPRO_ASSET_V_1_1_0
#endif

#if UNITY_6000_0_OR_NEWER
// This Unity version is guaranteed to support at least Unity UI 2.0,
// which uses asset version 1.1.0. Avoid most of the reflection nonsense.
// However, some reflection is still required because
// TMPro.TMP_SpriteAsset now has internal setters for the tables and
// version in Unity UI 2.0. This effectively prevents scripts from
// modfying the Sprite Asset tables at runtime.
#define SUPPORTS_AT_LEAST_TMPRO_ASSET_V_1_1_0
#endif

#pragma warning disable 0649

namespace Rewired.Glyphs.UnityUI {
    using System.Reflection;
    using Rewired;
    using System;
    using System.Text;
    using System.Collections.Generic;

    /// <summary>
    /// A helper class to display Rewired glyphs and display names inline in TMPro Text (Unity UI).
    /// Parses tags within text and replaces them with Sprites or text.
    /// Enter text in this component's text field, not in the TMPro Text field. Text entered here
    /// will be parsed, then the Text Mesh  Pro Text component will be updated with the text, including Sprite tags.
    /// </summary>
    [UnityEngine.AddComponentMenu("Rewired/Glyphs/Unity UI/Unity UI Text Mesh Pro Glyph Helper")]
    [UnityEngine.RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class UnityUITextMeshProGlyphHelper : UnityEngine.MonoBehaviour {

        [UnityEngine.Tooltip("Enter text into this field and not in the TMPro Text field directly. " +
            "Text will be parsed for special tags, and the final result will be passed on to the Text Mesh Pro Text component. " +
            "See the documentation for special tag format.")]
        [UnityEngine.SerializeField]
        [UnityEngine.TextArea(3, 10)]
        private string _text;

        [UnityEngine.Tooltip("Optional reference to an object that defines options. If blank, the global default options will be used.")]
        [UnityEngine.SerializeField]
        private ControllerElementGlyphSelectorOptionsSOBase _options;
        [UnityEngine.Tooltip("Options that control how Text Mesh Pro displays Sprites.")]
        [UnityEngine.SerializeField]
        private TMProSpriteOptions _spriteOptions = TMProSpriteOptions.Default;
        [UnityEngine.Tooltip("Optional material for Sprites. If blank, the default material will be used.\n" +
            "Material is instantiated for each Sprite Asset, so making changes to values in the base material later will not affect Sprites. " +
            "Changing the base material at runtime will copy only certain properties from the new material to Sprite materials."
            )]
        [UnityEngine.SerializeField]
        private UnityEngine.Material _baseSpriteMaterial;
        [UnityEngine.Tooltip("If enabled, local values such as Sprite color will be used instead of the value on the base material.")]
        [UnityEngine.SerializeField]
        private bool _overrideSpriteMaterialProperties = true;
        [UnityEngine.Tooltip("These properties will override the properties on the Sprite material if Override Sprite Material Properties is enabled.")]
        [UnityEngine.SerializeField]
        private SpriteMaterialProperties _spriteMaterialProperties = SpriteMaterialProperties.Default;

        [NonSerialized]
        private TMPro.TextMeshProUGUI _tmProText;
        [NonSerialized]
        private string _textPrev;
        [NonSerialized]
        private readonly StringBuilder _processTagSb = new StringBuilder();
        [NonSerialized]
        private readonly StringBuilder _tempSb = new StringBuilder();
        [NonSerialized]
        private readonly StringBuilder _tempSb2 = new StringBuilder();
        [NonSerialized]
        private Asset _primaryAsset;
        [NonSerialized]
        private readonly List<Asset> _assignedAssets = new List<Asset>();
        [NonSerialized]
        private readonly List<Asset> _assetsPool = new List<Asset>();
        [NonSerialized]
        private readonly List<ActionElementMap> _tempAems = new List<ActionElementMap>();
        [NonSerialized]
        private readonly List<UnityEngine.Sprite> _tempGlyphs = new List<UnityEngine.Sprite>();
        [NonSerialized]
        private readonly List<Asset> _dirtyAssets = new List<Asset>();
        [NonSerialized]
        private readonly List<string> _tempKeys = new List<string>();
        [NonSerialized]
        private readonly List<GlyphOrText> _glyphsOrTextTemp = new List<GlyphOrText>();
        [NonSerialized]
        private readonly List<Asset> _currentlyUsedAssets = new List<Asset>();
        [NonSerialized]
        private readonly List<Tag> _currentTags = new List<Tag>();
        [NonSerialized]
        private Dictionary<string, string> _tempStringDictionary = new Dictionary<string, string>();
        [NonSerialized]
        private bool _initialized;
        [NonSerialized]
        private bool _rebuildRequired;
        [NonSerialized]
        private UnityEngine.Texture2D _stubTexture;

        private Tag.Pool<ControllerElementTag> __controllerElementTagPool;
        private Tag.Pool<ControllerElementTag> controllerElementTagPool {
            get {
                return __controllerElementTagPool != null ? __controllerElementTagPool : (__controllerElementTagPool = new Tag.Pool<ControllerElementTag>());
            }
        }

        private Tag.Pool<ActionTag> __actionTagPool;
        private Tag.Pool<ActionTag> actionTagPool {
            get {
                return __actionTagPool != null ? __actionTagPool : (__actionTagPool = new Tag.Pool<ActionTag>());
            }
        }

        private Tag.Pool<PlayerTag> __playerTagPool;
        private Tag.Pool<PlayerTag> playerTagPool {
            get {
                return __playerTagPool != null ? __playerTagPool : (__playerTagPool = new Tag.Pool<PlayerTag>());
            }
        }

        [NonSerialized]
        private Dictionary<string, ParseTagAttributesHandler> __tagHandlers;
        private Dictionary<string, ParseTagAttributesHandler> tagHandlers {
            get {
                return __tagHandlers != null ? __tagHandlers : (__tagHandlers = new Dictionary<string, ParseTagAttributesHandler>() {
                        { "rewiredelement", ProcessTag_ControllerElement },
                        { "rewiredaction", ProcessTag_Action },
                        { "rewiredplayer", ProcessTag_Player }
                    });
            }
        }

        /// <summary>
        /// Text will be parsed for special tags, and the final result will be passed on to the Text Mesh Pro Text component.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// Tag Format:
        ///   
        ///   Display glyph / text for a controller element bound to an Action for a Player:
        ///   <rewiredElement [attributes]>
        ///   Attributes:
        ///     type="[glyphOrText|glyph|text]" (optional, default: glyphOrText)
        ///     playerId=[player_id] (id or name required)
        ///     playerName="[player_name]" (id or name required)
        ///     actionId=[action_id] (id or name required)
        ///     actionName="[action_name]" (id or name required)
        ///     actionRange="[full|positive|negative]" (optional, default: full)
        ///   Example: <rewiredElement type="glyphOrText" playerId=5 actionName="MoveHorizontal" actionRange="full">
        ///
        ///   Display the name of an Action:
        ///   <rewiredAction [attributes]>
        ///   Attributes:
        ///     id=[action_id] (id or name required)
        ///     name=[action_name] (id or name required)
        ///     range="[full|positive|negative]" (optional, default: full)
        ///   Example: <rewiredAction name="MoveHorizontal" range="positive">
        ///
        ///   Display the name of a Player:
        ///   <rewiredPlayer [attributes]>
        ///   Attributes:
        ///     id=[action_id] (id or name required)
        ///     name=[action_name] (id or name required)
        ///   Example: <rewiredPlayer id=0>
        /// ]]>
        /// </example>
        public virtual string text {
            get {
                return _text;
            }
            set {
                _text = value;
                RequireRebuild();
            }
        }

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
        /// Options that control how Text Mesh Pro displays Sprites.
        /// </summary>
        public virtual TMProSpriteOptions spriteOptions {
            get {
                return _spriteOptions;
            }
            set {
                _spriteOptions = value;

                // Set values in all sprites in all assets
                int count = _assignedAssets.Count;
                int spriteCount;
                ITMProSprite sprite;
                UnityEngine.Rect rect;

                for (int i = 0; i < count; i++) {
                    spriteCount = _assignedAssets[i].spriteAsset.spriteCount;
                    for (int j = 0; j < spriteCount; j++) {
                        sprite = _assignedAssets[i].spriteAsset.GetSprite(j);
                        if (sprite == null || sprite.sprite == null) continue;
                        rect = sprite.sprite.rect;
                        sprite.xOffset = rect.width * _spriteOptions.offsetSizeMultiplier.x + _spriteOptions.extraOffset.x;
                        sprite.yOffset = rect.height * _spriteOptions.offsetSizeMultiplier.y + _spriteOptions.extraOffset.y;
                        sprite.xAdvance = rect.width * _spriteOptions.xAdvanceWidthMultiplier + _spriteOptions.extraXAdvance;
                        sprite.scale = _spriteOptions.scale;
                    }
                    // Make TMPro update the Sprite scale
                    TMPro.TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, _assignedAssets[i].spriteAsset.GetSpriteAsset());
                }
            }
        }

        /// <summary>
        /// Optional material for Sprites. If blank, the default material will be used.
        /// Material is instantiated for each Sprite Asset, so making changes to values in the base material later will not affect Sprites.
        /// Changing the base material at runtime will copy only certain properties from the new material to Sprite materials.
        /// </summary>
        public virtual UnityEngine.Material baseSpriteMaterial {
            get {
                return _baseSpriteMaterial;
            }
            set {
                _baseSpriteMaterial = value;

                UnityEngine.Material sourceMaterial = _baseSpriteMaterial != null ? _baseSpriteMaterial : _primaryAsset.material;

                ForEachAsset(asset => {
                    CopyMaterialProperties(sourceMaterial, asset.material);
                    if (_overrideSpriteMaterialProperties) {
                        CopySpriteMaterialPropertiesToMaterial(_spriteMaterialProperties, asset.material);
                    }
                    TMPro.TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, asset.material);
                });
            }
        }

        /// <summary>
        /// If enabled, local values such as Sprite color will be used instead of the value on the base material.
        /// </summary>
        public virtual bool overrideSpriteMaterialProperties {
            get {
                return _overrideSpriteMaterialProperties;
            }
            set {
                _overrideSpriteMaterialProperties = value;
                if (value) {
                    ForEachAsset(asset => {
                        CopySpriteMaterialPropertiesToMaterial(_spriteMaterialProperties, asset.material);
                        TMPro.TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, asset.material);
                    });
                } else {
                    UnityEngine.Material sourceMaterial = _baseSpriteMaterial != null ? _baseSpriteMaterial : _primaryAsset.material;
                    ForEachAsset(asset => {
                        CopyMaterialProperties(sourceMaterial, asset.material);
                        TMPro.TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, asset.material);
                    });
                }
            }
        }

        /// <summary>
        /// These properties will override the properties on the Sprite material if <see cref="overrideSpriteMaterialProperties"/> is true.
        /// </summary>
        public virtual SpriteMaterialProperties spriteMaterialProperties {
            get {
                return _spriteMaterialProperties;
            }
            set {
                _spriteMaterialProperties = value;
                if (!_overrideSpriteMaterialProperties) return;
                ForEachAsset(asset => {
                    CopySpriteMaterialPropertiesToMaterial(_spriteMaterialProperties, asset.material);
                    TMPro.TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, asset.material);
                });
            }
        }

        protected virtual void OnEnable() {
            Initialize();
        }

        protected virtual void Start() {
            MainUpdate();
        }

        protected virtual void Update() {
            if (!ReInput.isReady) return;

#if UNITY_EDITOR
            // Handle recompile in Play mode
            if (!Initialize()) return;
            
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.M)) {
                this.baseSpriteMaterial = _baseSpriteMaterial;
            }

            // Handle inspector value changes
            if (_editorInspectorChanged) {
                this.spriteOptions = _spriteOptions;
                this.baseSpriteMaterial = _baseSpriteMaterial;
                this.overrideSpriteMaterialProperties = _overrideSpriteMaterialProperties;
                this.spriteMaterialProperties = _spriteMaterialProperties;
                RequireRebuild();
                _editorInspectorChanged = false;
            }
#endif

            MainUpdate();
        }

        protected virtual void OnDestroy() {
            // Clean up all assets
            if (_primaryAsset != null) {
                if (_tmProText != null) {
                    if (_tmProText.spriteAsset == _primaryAsset.spriteAsset.GetSpriteAsset()) {
                        _tmProText.spriteAsset = null;
                    }
                }
                _primaryAsset.Destroy();
                _primaryAsset = null;
            }
            for (int i = 0; i < _assignedAssets.Count; i++) {
                if (_assignedAssets[i] == null) continue;
                _assignedAssets[i].Destroy();
            }
            _assignedAssets.Clear();
            for (int i = 0; i < _assetsPool.Count; i++) {
                if (_assetsPool[i] == null) continue;
                _assetsPool[i].Destroy();
            }
            _assetsPool.Clear();
            if (_stubTexture != null) {
                UnityEngine.Object.Destroy(_stubTexture);
                _stubTexture = null;
            }
            for (int i = 0; i < _currentTags.Count; i++) {
                _currentTags[i].ReturnToPool();
            }
        }

#if UNITY_EDITOR

        [NonSerialized]
        private bool _editorInspectorChanged;

        protected virtual void OnValidate() {
            _editorInspectorChanged = true;
        }

#endif

        /// <summary>
        /// Updates glyphs / text and TMPro text value immedately.
        /// Normally, this happens automatically in the Update loop.
        /// </summary>
        public virtual void ForceUpdate() {
            if (!ReInput.isReady) return;
            _rebuildRequired = true;
            Update();
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

        private bool Initialize() {
            if (_initialized) return true;
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isCompiling) return false;
#endif
            _tmProText = GetComponent<TMPro.TextMeshProUGUI>();
            _stubTexture = new UnityEngine.Texture2D(1, 1);
            CreatePrimaryAsset();
            _initialized = true;
            return true;
        }

        private void MainUpdate() {

            bool changed = false;

            // Monitor changes to glyph / text results.
            // This is necessary because controller element glyph changes may occur when
            // last active controller changes, when glyph sets change, when glyphs are
            // disabled / enabled, when mappings change, or in the case of text,
            // when localization changes.
            {
                Tag tag;
                int count = _currentTags.Count;
                for (int i = 0; i < count; i++) {
                    tag = _currentTags[i];
                    switch (tag.tagType) {
                        case Tag.TagType.ControllerElement: {
                                // Monitor controller element glyph changes
                                ControllerElementTag tTag = (ControllerElementTag)tag;
                                _glyphsOrTextTemp.Clear();
                                TryGetControllerElementGlyphsOrText((ControllerElementTag)tag, _glyphsOrTextTemp);
                                if (!IsEqual(_glyphsOrTextTemp, tTag.glyphsOrText)) {
                                    changed = true;
                                    break;
                                }
                            }
                            break;
                        case Tag.TagType.Action: {
                                // Monitor Action changes
                                ActionTag tTag = (ActionTag)tag;
                                string displayName;
                                TryGetActionDisplayName(tTag, out displayName);
                                if (!string.Equals(tTag.displayName, displayName, StringComparison.Ordinal)) {
                                    changed = true;
                                    break;
                                }
                            }
                            break;
                        case Tag.TagType.Player: {
                                // Monitor Player changes
                                PlayerTag tTag = (PlayerTag)tag;
                                string displayName;
                                TryGetPlayerDisplayName(tTag, out displayName);
                                if (!string.Equals(tTag.displayName, displayName, StringComparison.Ordinal)) {
                                    changed = true;
                                    break;
                                }
                            break;
                        }
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            // Monitor text changes
            if (!string.Equals(_text, _textPrev, StringComparison.Ordinal)) {
                _textPrev = _text;
                changed = true;
            }

            // Update text on change
            if (changed || _rebuildRequired) {
                // Parse text and replace Rewired tags with TMPro sprite tags
                string newText;
                if (ParseText(_textPrev, out newText)) {
                    _tmProText.text = newText;
                }
            }

            // Process Sprite Assets that changed
            {
                int count = _dirtyAssets.Count;
                if (count > 0) {
                    for (int i = 0; i < count; i++) {
                        _dirtyAssets[i].spriteAsset.UpdateLookupTables();
                    }
                    _dirtyAssets.Clear();
                }
            }
        }

        private bool ParseText(string text, out string newText) {
            newText = null;

            // Clear current tags and build new onesfrom the text
            Tag.Clear(_currentTags);

            // Clear list so used assets can be detected after parsing
            _currentlyUsedAssets.Clear();

            bool changed = false;

            // Controller Element
            while (ProcessNextTag(ref text, _processTagSb)) {
                changed = true;
                newText = text;
            }

            RemoveUnusedAssets();

            if (_rebuildRequired) {
                _rebuildRequired = false;
            }

            return changed;
        }

        private bool ProcessNextTag(ref string text, StringBuilder sb) {
            const char tagStart = '<';
            const char tagEnd = '>';

            const int state_searchForStartTag = 0;
            const int state_searchForTagName = 1;
            const int state_parseAttributes = 2;

            int state = state_searchForStartTag;

            ParseTagAttributesHandler parseTagAttributesHandler = null;
            string replacementString;
            char c;
            int tagStartIndex = -1;

            try {
                for (int i = 0; i < text.Length; i++) {
                    c = text[i];

                    switch (state) {
                        case state_searchForStartTag:
                            if (c == tagStart) {
                                tagStartIndex = i;
                                sb.Length = 0;
                                state = state_searchForTagName;
                            }
                            break;
                        case state_searchForTagName:
                            if (IsValidTagNameChar(c)) {
                                sb.Append(char.ToLowerInvariant(c));
                            } else if (char.IsWhiteSpace(c)) {
                                if (sb.Length > 0) {
                                    if (tagHandlers.TryGetValue(sb.ToString(), out parseTagAttributesHandler)) {
                                        sb.Length = 0;
                                        state = state_parseAttributes;
                                    } else { // invalid tag name
                                        state = state_searchForStartTag; // start over
                                        i -= 1;
                                    }
                                }
                            } else { // error
                                state = state_searchForStartTag; // start over
                                i -= 1;
                            }
                            break;
                        case state_parseAttributes: {
                                int tagEndIndex = text.IndexOf(tagEnd, i); // TODO: This is possibly unsafe. If '>' is contained within any attribute string, it will fail.
                                if (tagEndIndex < 0) throw new Exception("Malformed tag."); // error
                                if (parseTagAttributesHandler(text, i, tagEndIndex - i, out replacementString)) {
                                    // replace string
                                    sb.Length = 0;
                                    if (tagStartIndex > 0) {
                                        sb.Append(text, 0, tagStartIndex);
                                    }
                                    sb.Append(replacementString);
                                    int tailStartIndex = tagEndIndex + 1;
                                    if (tailStartIndex < text.Length) {
                                        sb.Append(text, tailStartIndex, text.Length - tailStartIndex);
                                    }
                                    text = sb.ToString();
                                    return true;
                                } else { // error
                                    throw new Exception("Error parsing attributes.");
                                }
                            }
                    }
                }

            } catch (Exception ex) {
                UnityEngine.Debug.LogError(ex);
            }

            return false;
        }

        private bool ProcessTag_ControllerElement(string text, int startIndex, int count, out string replacement) {
            ControllerElementTag tag;
            if (!ControllerElementTag.TryParseString(text, startIndex, count, _tempSb, _tempSb2, _tempStringDictionary, controllerElementTagPool, out tag)) {
                replacement = null;
                return false; // only fail if parsing fails
            }
            
            _currentTags.Add(tag);

            // Replace text with glyph / text

            tag.glyphsOrText.Clear();
            if (!TryGetControllerElementGlyphsOrText(tag, tag.glyphsOrText)) {
                replacement = null;
                return true;
            }

            TryCreateTMProString(tag.glyphsOrText, out replacement);

            return true;
        }

        private bool ProcessTag_Action(string text, int startIndex, int count, out string replacement) {
            ActionTag tag;
            if (!ActionTag.TryParseString(text, startIndex, count, _tempSb, _tempSb2, _tempStringDictionary, actionTagPool, out tag)) {
                replacement = null;
                return false; // only fail if parsing fails
            }

            _currentTags.Add(tag);

            TryGetActionDisplayName(tag, out replacement);

            return true;
        }

        private bool ProcessTag_Player(string text, int startIndex, int count, out string replacement) {
            PlayerTag tag;
            if (!PlayerTag.TryParseString(text, startIndex, count, _tempSb, _tempSb2, _tempStringDictionary, playerTagPool, out tag)) {
                replacement = null;
                return false; // only fail if parsing fails
            }

            _currentTags.Add(tag);

            TryGetPlayerDisplayName(tag, out replacement);

            return true;
        }

        private bool TryCreateTMProString(List<GlyphOrText> glyphs, out string result) {
            
            // Replace text with glyph / text
            
            StringBuilder sb = _tempSb;
            sb.Length = 0;
            string spriteKey;

            int resultCount = glyphs.Count;
            for (int i = 0; i < resultCount; i++) {
                spriteKey = glyphs[i].glyphKey;

                // Prioritize glyphs
                if (glyphs[i].sprite != null &&
                    !string.IsNullOrEmpty(spriteKey) &&
                    TryAssignSprite(glyphs[i].sprite, spriteKey)) {
                    WriteSpriteKey(sb, spriteKey);
                } else {
                    // Fall back to text
                    sb.Append(glyphs[i].name);
                }

                if (i < resultCount - 1) sb.Append(" ");
            }

            result = sb.ToString();

            return !string.IsNullOrEmpty(result);
        }

        private bool TryGetControllerElementGlyphsOrText(ControllerElementTag tag, List<GlyphOrText> results) {
            if (tag == null) return false;

            ActionElementMap aemResult1;
            ActionElementMap aemResult2;

            _tempAems.Clear();
            if (!GlyphTools.TryGetActionElementMaps(tag.playerId, tag.actionId, tag.actionRange, GetOptionsOrDefault(), _tempAems, out aemResult1, out aemResult2)) {
                return false;
            }

            object glyph;
            string glyphKey;
            string name;

            // Two split axis bindings
            if (aemResult1 != null && aemResult2 != null) {

                GlyphOrText r = new GlyphOrText();

                // Handle special combined glyphs (D-Pad Left + D-Pad Right = D-Pad Horizontal)
                _tempAems.Clear();
                _tempAems.Add(aemResult1);
                _tempAems.Add(aemResult2);
                if (IsGlyphAllowed(tag.type) && ActionElementMap.TryGetCombinedElementIdentifierGlyph(_tempAems, out glyph) &&
                    ActionElementMap.TryGetCombinedElementIdentifierFinalGlyphKey(_tempAems, out glyphKey)) {
                    r.glyphKey = glyphKey;
                    r.sprite = glyph as UnityEngine.Sprite;
                    results.Add(r);
                    return true;
                } else if (IsTextAllowed(tag.type) && ActionElementMap.TryGetCombinedElementIdentifierName(_tempAems, out name)) {
                    r.name = name;
                    results.Add(r);
                    return true;
                }
            }

            bool result = false;

            // Positive and negative bindings
            _tempGlyphs.Clear();
            _tempKeys.Clear();
            result |= TryGetGlyphsOrText(aemResult1, tag.type, _tempGlyphs, _tempKeys, results);
            _tempGlyphs.Clear();
            _tempKeys.Clear();
            result |= TryGetGlyphsOrText(aemResult2, tag.type, _tempGlyphs, _tempKeys, results);

            return result;
        }

        private bool TryGetActionDisplayName(ActionTag tag, out string result) {
            if (tag == null) {
                result = null;
                return false;
            }

            InputAction action = ReInput.mapping.GetAction(tag.actionId);
            if (action == null) {
                result = null;
                return false;
            }

            result = action.GetDisplayName(tag.actionRange);

            tag.displayName = result;

            return true;
        }

        private bool TryGetPlayerDisplayName(PlayerTag tag, out string result) {
            if (tag == null) {
                result = null;
                return false;
            }

            var player = ReInput.players.GetPlayer(tag.playerId);
            if (player == null) {
                result = null;
                return false;
            }

            result = player.descriptiveName;

            tag.displayName = result;

            return true;
        }

        private bool TryAssignSprite(UnityEngine.Sprite sprite, string key) {

            Asset asset = GetOrCreateAsset(sprite);
            if (asset == null) return false;

            ITMProSpriteAsset spriteAsset = asset.spriteAsset;

            if (!spriteAsset.Contains(key)) {
                
                UnityEngine.Rect rect = sprite.rect;

                ITMProSprite tmpSprite = TMProAssetVersionHelper.CreateSprite();
                // id is set in AddSprite
                tmpSprite.width = rect.width;
                tmpSprite.height = rect.height;
                tmpSprite.position = new UnityEngine.Vector2(rect.x, rect.y);
                tmpSprite.xOffset = rect.width * _spriteOptions.offsetSizeMultiplier.x + _spriteOptions.extraOffset.x;
                tmpSprite.yOffset = rect.height * _spriteOptions.offsetSizeMultiplier.y + _spriteOptions.extraOffset.y;
                tmpSprite.xAdvance = rect.width * _spriteOptions.xAdvanceWidthMultiplier + _spriteOptions.extraXAdvance;
                tmpSprite.scale = _spriteOptions.scale;
                tmpSprite.pivot = new UnityEngine.Vector2(
                    rect.width * -0.5f,
                    rect.height * 0.5f
                );
                tmpSprite.name = key;
                tmpSprite.hashCode = TMPro.TMP_TextUtilities.GetSimpleHashCode(key);
                tmpSprite.sprite = sprite;

                spriteAsset.AddSprite(tmpSprite);
                SetDirty(asset);
            }

            // Add to currently used assets list
            if (!_currentlyUsedAssets.Contains(asset)) {
                _currentlyUsedAssets.Add(asset);
            }

            return true;
        }

        private void RequireRebuild() {
            _rebuildRequired = true;
        }

        #region Sprite Assets

        private void CreatePrimaryAsset() {
            if (_primaryAsset != null) return;
            _primaryAsset = new Asset(null); // always create with default material, not user-defined base material
            _tmProText.spriteAsset = _primaryAsset.spriteAsset.GetSpriteAsset();
        }

        private Asset GetOrCreateAsset(UnityEngine.Sprite sprite) {
            if (sprite == null || sprite.texture == null) return null;

            // Search assigned list first
            {
                int count = _assignedAssets.Count;
                for (int i = 0; i < count; i++) {
                    if (_assignedAssets[i] == null) continue;
                    if (_assignedAssets[i].spriteAsset.spriteSheet == sprite.texture) {
                        return _assignedAssets[i];
                    }
                }
            }

            Asset asset = null;

            // Not found
            // Check pool for an unused asset
            {
                int count = _assetsPool.Count;
                for (int i = 0; i < count; i++) {
                    if (_assetsPool[i] == null) continue;
                    asset = _assetsPool[i];
                    _assetsPool.RemoveAt(i); // remove from pool
                    break;
                }
            }
            if (asset == null) { // No asset found. Create one.
                asset = CreateAsset();
            }

            // Set texture in Sprite Asset and Material
            asset.spriteAsset.spriteSheet = sprite.texture;
            asset.material.SetTexture(TMPro.ShaderUtilities.ID_MainTex, sprite.texture);

            // Assign the Sprite Asset to the primary as a fallback

            List<TMPro.TMP_SpriteAsset> fallbacks = _primaryAsset.spriteAsset.GetSpriteAsset().fallbackSpriteAssets;
            if (fallbacks == null) {
                fallbacks = new List<TMPro.TMP_SpriteAsset>();
                _primaryAsset.spriteAsset.GetSpriteAsset().fallbackSpriteAssets = fallbacks;
            }
            fallbacks.Add(asset.spriteAsset.GetSpriteAsset());

            _assignedAssets.Add(asset);

            return asset;
        }

        private Asset CreateAsset() {
            Asset asset = new Asset(_baseSpriteMaterial);
            if (_overrideSpriteMaterialProperties) {
                CopySpriteMaterialPropertiesToMaterial(_spriteMaterialProperties, asset.material);
            }
            return asset;
        }

        private void RemoveUnusedAssets() {

            // Allow a few assets to remain assigned to avoid wasted resources swapping
            // assets back and forth when changing between last active controllers.
            const int allowedUnusedAssetCount = 2;

            // Find assets that are unused and remove them

            Asset asset;
            int unusedAssetCount = 0;
            int count = _assignedAssets.Count;

            for (int i = count - 1; i >= 0; i--) { // go backwards so newest unused assets are preserved first
                asset = _assignedAssets[i];
                if (asset == null) continue;
                if (!_currentlyUsedAssets.Contains(asset)) { // not used anymore
                    if (unusedAssetCount >= allowedUnusedAssetCount) { // no more extras allowed
                        // Remove from primary sprite asset fallback list
                        _primaryAsset.spriteAsset.GetSpriteAsset().fallbackSpriteAssets.Remove(asset.spriteAsset.GetSpriteAsset());
                        // Clean up values when adding to pool
                        asset.spriteAsset.spriteSheet = null;
                        asset.spriteAsset.Clear();
                        asset.material.SetTexture(TMPro.ShaderUtilities.ID_MainTex, _stubTexture); // set to stub  to prevent TMPro from throwing null reference exceptions
                        _assetsPool.Add(asset); // add to pool
                        _assignedAssets.RemoveAt(i); // remove from assigned assets
                    } else {
                        unusedAssetCount += 1;
                    }
                }
            }
        }

        private void SetDirty(Asset asset) {
            if (_dirtyAssets.Contains(asset)) return;
            _dirtyAssets.Add(asset);
        }

        private void ForEachAsset(Action<Asset> callback) {
            if (callback == null) return;

            int count;

            // Set values in all sprites in all assets
            count = _assignedAssets.Count;
            for (int i = 0; i < count; i++) {
                if (_assignedAssets[i] == null) continue;
                callback(_assignedAssets[i]);

            }

            // Material properties need to be set in pool as well
            count = _assetsPool.Count;
            for (int i = 0; i < count; i++) {
                if (_assetsPool[i] == null) continue;
                callback(_assetsPool[i]);
            }
        }

        #endregion

        #region Static

        private static int shaderPropertyId_color { get { return UnityEngine.Shader.PropertyToID("_Color"); } }

        private static string[] __s_displayTypeNames;
        private static string[] s_displayTypeNames { get { return __s_displayTypeNames != null ? __s_displayTypeNames : (__s_displayTypeNames = Enum.GetNames(typeof(DisplayType))); } }

        private static DisplayType[] __s_displayTypeValues;
        private static DisplayType[] s_displayTypeValues { get { return __s_displayTypeValues != null ? __s_displayTypeValues : (__s_displayTypeValues = (DisplayType[])Enum.GetValues(typeof(DisplayType))); } }

        private static string[] __s_axisRangeNames;
        private static string[] s_axisRangeNames { get { return __s_axisRangeNames != null ? __s_axisRangeNames : (__s_axisRangeNames = Enum.GetNames(typeof(Rewired.AxisRange))); } }

        private static Rewired.AxisRange[] __s_axisRangeValues;
        private static Rewired.AxisRange[] s_axisRangeValues { get { return __s_axisRangeValues != null ? __s_axisRangeValues : (__s_axisRangeValues = (Rewired.AxisRange[])Enum.GetValues(typeof(Rewired.AxisRange))); } }

        private static void ParseAttributes(string text, int startIndex, int count, StringBuilder sbKey, StringBuilder sbValue, Dictionary<string, string> results) {
            if (string.IsNullOrEmpty(text) || startIndex < 0 || startIndex >= text.Length) {
                return;
            }

            results.Clear();

            const char doubleQuote = '\"';

            const int state_searchForKey = 0;
            const int state_parsingKey = 1;
            const int state_searchForValue = 2;
            const int state_parsingValue = 3;

            sbKey.Length = 0;
            sbValue.Length = 0;
            bool isQuotedValue = true;
            char c;
            int lastIndex = startIndex + count - 1;

            int state = state_searchForKey;

            try {
                
                for (int i = startIndex; i < startIndex + count; i++) {
                    c = text[i];
                    switch (state) {
                        case state_searchForKey:
                            if (IsValidKeyChar(c)) {
                                state = state_parsingKey;
                                i -= 1;
                                sbKey.Length = 0;
                            }
                            break;
                        case state_parsingKey:
                            if (c == '=') {
                                if (sbKey.Length == 0) throw new Exception("Key was blank."); // no key
                                state = state_searchForValue;
                            } else if (IsValidKeyChar(c)) {
                                sbKey.Append(char.ToLowerInvariant(c));
                            } else if (char.IsWhiteSpace(c)) {
                                // skip
                            } else { // parsing error
                                throw new Exception("Error parsing key.");
                            }
                            break;
                        case state_searchForValue: {
                                if ((isQuotedValue = c == doubleQuote) || IsValidNonQuotedValueChar(c)) { // allow quoted and non-quoted values
                                    if (!isQuotedValue) {
                                        i -= 1;
                                    }
                                    sbValue.Length = 0;
                                    state = state_parsingValue;
                                }
                            }
                            break;
                        case state_parsingValue:
                            {
                                if ((isQuotedValue && c == doubleQuote) || (!isQuotedValue && (i == lastIndex || char.IsWhiteSpace(c)))) { // end of value
                                    // Append value on last index for non-quoted values, otherwise it will fail if no space before closing tag
                                    if (!isQuotedValue && i == lastIndex) sbValue.Append(c);
                                    if (sbValue.Length == 0) throw new Exception("Value was blank."); // no value
                                    if (results == null) results = new Dictionary<string, string>();
                                    results.Add(sbKey.ToString(), sbValue.ToString());
                                    state = state_searchForKey;
                                } else {
                                    sbValue.Append(c);
                                }
                            }
                            break;
                    }
                }
            } catch (Exception ex) {
                UnityEngine.Debug.LogError(ex);
            }
        }

        private static bool IsValidKeyChar(char c) {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private static bool IsValidTagNameChar(char c) {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private static bool IsValidNonQuotedValueChar(char c) {
            return char.IsDigit(c);
        }

        private static bool IsEqual(List<GlyphOrText> a, List<GlyphOrText> b) {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++) {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private static void WriteSpriteKey(StringBuilder sb, string key) {
            sb.Append("<sprite name=\"");
            sb.Append(key);
            sb.Append("\">");
        }

        private static bool TryGetGlyphsOrText(ActionElementMap aem, DisplayType displayType, List<UnityEngine.Sprite> glyphs, List<string> keys, List<GlyphOrText> results) {
            if (aem == null || glyphs == null || results == null) return false;

            GlyphOrText r;

            if (IsGlyphAllowed(displayType) && aem.GetElementIdentifierGlyphs(glyphs) > 0) {
                aem.GetElementIdentifierFinalGlyphKeys(keys);
                if (keys.Count != glyphs.Count) {
                    UnityEngine.Debug.LogError("Rewired: Glyph key count does not match glyph count.");
                    goto TextFallback;
                }
                int count = glyphs.Count;
                for (int i = 0; i < count; i++) {
                    r = new GlyphOrText();
                    r.glyphKey = keys[i];
                    r.sprite = glyphs[i];
                    results.Add(r);
                }
                if (count > 0) return true;
            }

        TextFallback:

            if (IsTextAllowed(displayType)) {
                r = new GlyphOrText();
                r.name = aem.elementIdentifierName;
                results.Add(r);
                return true;
            }

            return false;
        }

        private static bool IsGlyphAllowed(DisplayType displayType) {
            return displayType == DisplayType.Glyph || displayType == DisplayType.GlyphOrText;
        }

        private static bool IsTextAllowed(DisplayType displayType) {
            return displayType == DisplayType.Text || displayType == DisplayType.GlyphOrText;
        }

        private static void CopyMaterialProperties(UnityEngine.Material source, UnityEngine.Material destination) {
            if (source == null || destination == null) return;
            destination.shader = source.shader;
            if (source.shaderKeywords != null) {
                string[] keywords = new string[source.shaderKeywords.Length];
                Array.Copy(source.shaderKeywords, keywords, source.shaderKeywords.Length);
                destination.shaderKeywords = keywords;
            } else {
                destination.shaderKeywords = null;
            }
            if (source.HasProperty(shaderPropertyId_color) && destination.HasProperty(shaderPropertyId_color)) {
                destination.color = source.color;
            }
            destination.renderQueue = source.renderQueue;
            destination.globalIlluminationFlags = source.globalIlluminationFlags;
        }

        private static void CopySpriteMaterialPropertiesToMaterial(SpriteMaterialProperties properties, UnityEngine.Material material) {
            if (material == null) return;
            if (material.HasProperty(shaderPropertyId_color)) {
                material.color = properties.color;
            }
        }

        #endregion

        #region Classes

        private delegate bool ParseTagAttributesHandler(string text, int startIndex, int count, out string replacement);

        private abstract class Tag {

            public readonly TagType tagType;
            private Pool _pool;

            protected Tag(TagType tagType) {
                this.tagType = tagType;
            }

            protected Pool pool {
                get {
                    return _pool;
                }
                set {
                    _pool = value;
                }
            }

            public void ReturnToPool() {
                if (_pool == null) return;
                _pool.Return(this);
            }

            protected abstract void Clear();

            public static void Clear(List<Tag> list) {
                int count = list.Count;
                for (int i = 0; i < count; i++) {
                    if (list[i] == null) continue;
                    list[i].ReturnToPool();
                }
                list.Clear();
            }

            public enum TagType {
                ControllerElement,
                Action,
                Player
            }

            public abstract class Pool {

                public abstract bool Return(Tag obj);
            }

            public sealed class Pool<T> : Pool where T : Tag, new() {

                private readonly List<T> _list;

                public Pool() : base() {
                    _list = new List<T>();
                }

                public T Get() {
                    T obj;
                    if (_list.Count == 0) {
                        obj = new T();
                        if (obj != null) {
                            obj.pool = this;
                        }
                        return obj;
                    }
                    int index = _list.Count - 1;
                    obj = _list[index];
                    _list.RemoveAt(index);
                    return obj;
                }

                public override bool Return(Tag obj) {
                    T tObj = obj as T;
                    if (tObj == null || tObj.pool != this) return false;
                    tObj.Clear();
                    if (_list.Contains(tObj)) return false;
                    _list.Add(tObj);
                    return true;
                }
            }
        }

        private sealed class ControllerElementTag : Tag {

            public DisplayType type;
            public int playerId;
            public int actionId;
            public AxisRange actionRange;

            private readonly List<GlyphOrText> _glyphsOrText;

            public List<GlyphOrText> glyphsOrText { get { return _glyphsOrText; } }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append(typeof(ControllerElementTag).Name);
                sb.Append(": ");
                sb.Append("type = ");
                sb.Append(type);
                sb.Append(", playerId = ");
                sb.Append(playerId);
                sb.Append(", actionId = ");
                sb.Append(actionId);
                sb.Append(", actionRange = ");
                sb.Append(actionRange);
                return sb.ToString();
            }

            public ControllerElementTag() : base(TagType.ControllerElement) {
                _glyphsOrText = new List<GlyphOrText>();
                Clear();
            }

            protected override void Clear() {
                type = DisplayType.GlyphOrText;
                playerId = -1;
                actionId = -1;
                actionRange = AxisRange.Full;
                _glyphsOrText.Clear();
            }

            // Static

            public static bool TryParseString(string text, int startIndex, int count, StringBuilder sb1, StringBuilder sb2, Dictionary<string, string> workDictionary, Pool<ControllerElementTag> pool, out ControllerElementTag result) {
                result = null;
                if (string.IsNullOrEmpty(text) || startIndex < 0 || startIndex + count >= text.Length) return false;

                ParseAttributes(text, startIndex, count, sb1, sb2, workDictionary);
                if (workDictionary.Count == 0) return false;

                string value;
                result = pool.Get();

                try {

                    // Type -- optional
                    if (workDictionary.TryGetValue("type", out value)) {
                        bool found = false;
                        for (int i = 0; i < s_displayTypeNames.Length; i++) {
                            if (string.Equals(value, s_displayTypeNames[i], StringComparison.OrdinalIgnoreCase)) {
                                result.type = s_displayTypeValues[i];
                                found = true;
                                break;
                            }
                        }
                        if (!found) throw new Exception("Invalid type: " + value);
                    } else {
                        result.type = DisplayType.GlyphOrText; // default
                    }

                    // Player name or id
                    if (workDictionary.TryGetValue("playerid", out value)) {
                        result.playerId = int.Parse(value);
                        if (Rewired.ReInput.players.GetPlayer(result.playerId) == null) throw new Exception("Invalid Player Id: " + result.playerId);
                    } else if (workDictionary.TryGetValue("playername", out value)) {
                        var player = Rewired.ReInput.players.GetPlayer(value);
                        if (player == null) throw new Exception("Invalid Player name: " + value);
                        result.playerId = player.id;
                    } else { // error
                        throw new Exception("Player name/id missing.");
                    }

                    // Action name or id
                    if (workDictionary.TryGetValue("actionid", out value)) {
                        result.actionId = int.Parse(value);
                        if (Rewired.ReInput.mapping.GetAction(result.actionId) == null) throw new Exception("Invalid Action Id: " + result.actionId);
                    } else if (workDictionary.TryGetValue("actionname", out value)) {
                        var action = Rewired.ReInput.mapping.GetAction(value);
                        if (action == null) throw new Exception("Invalid Action name: " + value);
                        result.actionId = action.id;
                    } else { // error
                        throw new Exception("Action name/id missing.");
                    }

                    // Action Range -- optional
                    if (workDictionary.TryGetValue("actionrange", out value)) {
                        bool found = false;
                        for (int i = 0; i < s_axisRangeNames.Length; i++) {
                            if (string.Equals(value, s_axisRangeNames[i], StringComparison.OrdinalIgnoreCase)) {
                                result.actionRange = s_axisRangeValues[i];
                                found = true;
                                break;
                            }
                        }
                        if (!found) throw new Exception("Invalid Action Range: " + value);
                    } else {
                        result.actionRange = AxisRange.Full;
                    }

                    return true;

                } catch (Exception ex) {
                    UnityEngine.Debug.LogError(ex);
                    result.ReturnToPool();
                    return false;
                }
            }
        }

        private sealed class ActionTag : Tag {

            public int actionId;
            public AxisRange actionRange;

            private string _displayName;
            
            public string displayName {
                get {
                    return _displayName;
                }
                set {
                    _displayName = value;
                }
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append(typeof(ControllerElementTag).Name);
                sb.Append(": ");
                sb.Append("actionId = ");
                sb.Append(actionId);
                sb.Append(", actionRange = ");
                sb.Append(actionRange);
                return sb.ToString();
            }

            public ActionTag() : base(TagType.Action) {
                Clear();
            }

            protected override void Clear() {
                actionId = -1;
                actionRange = AxisRange.Full;
                _displayName = null;
            }

            // Static

            public static bool TryParseString(string text, int startIndex, int count, StringBuilder sb1, StringBuilder sb2, Dictionary<string, string> workDictionary, Pool<ActionTag> pool, out ActionTag result) {
                result = null;
                if (string.IsNullOrEmpty(text) || startIndex < 0 || startIndex + count >= text.Length) return false;

                ParseAttributes(text, startIndex, count, sb1, sb2, workDictionary);
                if (workDictionary.Count == 0) return false;

                string value;
                result = pool.Get();

                try {

                    // Action name or id
                    if (workDictionary.TryGetValue("id", out value) || workDictionary.TryGetValue("actionid", out value)) {
                        result.actionId = int.Parse(value);
                        if (Rewired.ReInput.mapping.GetAction(result.actionId) == null) throw new Exception("Invalid Action Id: " + result.actionId);
                    } else if (workDictionary.TryGetValue("name", out value) || workDictionary.TryGetValue("actionname", out value)) {
                        var action = Rewired.ReInput.mapping.GetAction(value);
                        if (action == null) throw new Exception("Invalid Action name: " + value);
                        result.actionId = action.id;
                    } else { // error
                        throw new Exception("Action name/id missing.");
                    }

                    // Action Range -- optional
                    if (workDictionary.TryGetValue("range", out value) || workDictionary.TryGetValue("actionrange", out value)) {
                        bool found = false;
                        for (int i = 0; i < s_axisRangeNames.Length; i++) {
                            if (string.Equals(value, s_axisRangeNames[i], StringComparison.OrdinalIgnoreCase)) {
                                result.actionRange = s_axisRangeValues[i];
                                found = true;
                                break;
                            }
                        }
                        if (!found) throw new Exception("Invalid Action Range: " + value);
                    } else {
                        result.actionRange = AxisRange.Full;
                    }

                    return true;

                } catch (Exception ex) {
                    UnityEngine.Debug.LogError(ex);
                    result.ReturnToPool();
                    return false;
                }
            }
        }

        private sealed class PlayerTag : Tag {

            public int playerId;

            private string _displayName;

            public string displayName {
                get {
                    return _displayName;
                }
                set {
                    _displayName = value;
                }
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append(typeof(ControllerElementTag).Name);
                sb.Append(": ");
                sb.Append("playerId = ");
                sb.Append(playerId);
                return sb.ToString();
            }

            public PlayerTag() : base(TagType.Player) {
                Clear();
            }

            protected override void Clear() {
                playerId = -1;
                _displayName = null;
            }

            // Static

            public static bool TryParseString(string text, int startIndex, int count, StringBuilder sb1, StringBuilder sb2, Dictionary<string, string> workDictionary, Pool<PlayerTag> pool, out PlayerTag result) {
                result = null;
                if (string.IsNullOrEmpty(text) || startIndex < 0 || startIndex + count >= text.Length) return false;

                ParseAttributes(text, startIndex, count, sb1, sb2, workDictionary);
                if (workDictionary.Count == 0) return false;

                string value;
                result = pool.Get();

                try {

                    // Action name or id
                    if (workDictionary.TryGetValue("id", out value) || workDictionary.TryGetValue("playerid", out value)) {
                        result.playerId = int.Parse(value);
                        if (Rewired.ReInput.players.GetPlayer(result.playerId) == null) throw new Exception("Invalid Player Id: " + result.playerId);
                    } else if (workDictionary.TryGetValue("name", out value) || workDictionary.TryGetValue("playername", out value)) {
                        var player = Rewired.ReInput.players.GetPlayer(value);
                        if (player == null) throw new Exception("Invalid Player name: " + value);
                        result.playerId = player.id;
                    } else { // error
                        throw new Exception("Player name/id missing.");
                    }

                    return true;

                } catch (Exception ex) {
                    UnityEngine.Debug.LogError(ex);
                    result.ReturnToPool();
                    return false;
                }
            }
        }

        private struct GlyphOrText : IEquatable<GlyphOrText> {

            public string glyphKey;
            public UnityEngine.Sprite sprite;
            public string name;

            public override bool Equals(object obj) {
                if (!(obj is GlyphOrText)) return false;
                GlyphOrText item = (GlyphOrText)obj;
                return string.Equals(item.glyphKey, glyphKey, StringComparison.Ordinal) &&
                    item.sprite == sprite &&
                    string.Equals(item.name, name, StringComparison.Ordinal);
            }

            public override int GetHashCode() {
                int hash = 17;
                hash = hash * 29 + glyphKey.GetHashCode();
                hash = hash * 29 + sprite.GetHashCode();
                hash = hash * 29 + name.GetHashCode();
                return hash;
            }

            public bool Equals(GlyphOrText other) {
                return string.Equals(other.glyphKey, glyphKey, StringComparison.Ordinal) &&
                    other.sprite == sprite &&
                    string.Equals(other.name, name, StringComparison.Ordinal);
            }

            public static bool operator ==(GlyphOrText a, GlyphOrText b) {
                return string.Equals(a.glyphKey, b.glyphKey, StringComparison.Ordinal) &&
                    a.sprite == b.sprite &&
                    string.Equals(a.name, b.name, StringComparison.Ordinal);
            }

            public static bool operator !=(GlyphOrText a, GlyphOrText b) {
                return !(a == b);
            }
        }

        private class Asset {

            public readonly uint id;

            private ITMProSpriteAsset _spriteAsset;
            private UnityEngine.Material _material;

            public ITMProSpriteAsset spriteAsset { get { return _spriteAsset; } }
            public UnityEngine.Material material { get { return _material; } }

            public Asset(UnityEngine.Material baseMaterial) {
                id = s_idCounter++;
                _spriteAsset = TMProAssetVersionHelper.CreateSpriteAsset();
                TMPro.TMP_SpriteAsset spriteAsset = _spriteAsset.GetSpriteAsset();
                spriteAsset.name = typeof(UnityUITextMeshProGlyphHelper).Name + " SpriteAsset " + id;
                spriteAsset.hashCode = TMPro.TMP_TextUtilities.GetSimpleHashCode(spriteAsset.name);
                _material = CreateMaterial(baseMaterial, id);
                if (_spriteAsset != null) {
                    spriteAsset.material = material;
                    spriteAsset.materialHashCode = TMPro.TMP_TextUtilities.GetSimpleHashCode(material.name);
                }
            }

            public static UnityEngine.Material CreateMaterial(UnityEngine.Material baseMaterial, uint id) {
                UnityEngine.Material material;
                material = baseMaterial != null ? new UnityEngine.Material(baseMaterial) : new UnityEngine.Material(tmProShader);
                material.name = typeof(UnityUITextMeshProGlyphHelper).Name + " Material " + id;
                material.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
                return material;
            }

            public void Destroy() {
                if (_spriteAsset != null) {
                    _spriteAsset.Destroy();
                    _spriteAsset = null;
                }
                if (_material != null) {
                    UnityEngine.Object.Destroy(_material);
                    _material = null;
                }
            }

            // Static

            private static uint s_idCounter;

            private static UnityEngine.Shader __tmProShader;
            private static UnityEngine.Shader tmProShader {
                get {
                    if (__tmProShader == null) {
                        TMPro.ShaderUtilities.GetShaderPropertyIDs();
                        __tmProShader = UnityEngine.Shader.Find("TextMeshPro/Sprite");
                    }
                    return __tmProShader;
                }
            }
        }

        /// <summary>
        /// Options for TMPro Sprite rendering.
        /// </summary>
        [Serializable]
        public struct TMProSpriteOptions : IEquatable<TMProSpriteOptions> {

            [UnityEngine.Tooltip("Scale.")]
            [UnityEngine.SerializeField]
            private float _scale;
            [UnityEngine.Tooltip("This value will be multiplied by the Sprite width and height and applied to offset.")]
            [UnityEngine.SerializeField]
            private UnityEngine.Vector2 _offsetSizeMultiplier;
            [UnityEngine.Tooltip("An extra offset that is cumulative with Offset Size Multiplier.")]
            [UnityEngine.SerializeField]
            private UnityEngine.Vector2 _extraOffset;
            [UnityEngine.Tooltip("This value will be multiplied by the Sprite width applied to X Advance.")]
            [UnityEngine.SerializeField]
            private float _xAdvanceWidthMultiplier;
            [UnityEngine.Tooltip("An extra offset that is cumulative with X Advance Width Multiplier.")]
            [UnityEngine.SerializeField]
            private float _extraXAdvance;

            /// <summary>
            /// Sprites will be scaled by this value.
            /// </summary>
            public float scale {
                get {
                    return _scale;
                }
                set {
                    _scale = value;
                }
            }

            /// <summary>
            /// This value will be multiplied by the Sprite width and height and applied to offset.
            /// </summary>
            public UnityEngine.Vector2 offsetSizeMultiplier {
                get {
                    return _offsetSizeMultiplier;
                }
                set {
                    _offsetSizeMultiplier = value;
                }
            }

            /// <summary>
            /// An extra offset that is cumulative with <see cref="offsetSizeMultiplier"/>.
            /// </summary>
            public UnityEngine.Vector2 extraOffset {
                get {
                    return _extraOffset;
                }
                set {
                    _extraOffset = value;
                }
            }

            /// <summary>
            /// This value will be multiplied by the Sprite width applied to X Advance.
            /// </summary>
            public float xAdvanceWidthMultiplier {
                get {
                    return _xAdvanceWidthMultiplier;
                }
                set {
                    _xAdvanceWidthMultiplier = value;
                }
            }

            /// <summary>
            /// An extra offset that is cumulative with <see cref="xAdvanceWidthMultiplier"/>.
            /// </summary>
            public float extraXAdvance {
                get {
                    return _extraXAdvance;
                }
                set {
                    _extraXAdvance = value;
                }
            }

            /// <summary>
            /// Creates an instance with the default values.
            /// </summary>
            /// <returns>An instance with the default values.</returns>
            public static TMProSpriteOptions Default {
                get {
                    TMProSpriteOptions o = new TMProSpriteOptions();
                    o.scale = 1.5f;
                    o.extraOffset = new UnityEngine.Vector2();
                    o.offsetSizeMultiplier = new UnityEngine.Vector2(0f, 0.75f);
                    o.xAdvanceWidthMultiplier = 1f;
                    return o;
                }
            }

            public override bool Equals(object obj) {
                if (!(obj is TMProSpriteOptions)) return false;
                TMProSpriteOptions item = (TMProSpriteOptions)obj;
                return item._scale == _scale &&
                    item._offsetSizeMultiplier == _offsetSizeMultiplier &&
                    item._extraOffset == _extraOffset &&
                    item._xAdvanceWidthMultiplier == _xAdvanceWidthMultiplier &&
                    item._extraXAdvance == _extraXAdvance;
            }

            public override int GetHashCode() {
                int hash = 17;
                hash = hash * 29 + _scale.GetHashCode();
                hash = hash * 29 + _offsetSizeMultiplier.GetHashCode();
                hash = hash * 29 + _extraOffset.GetHashCode();
                hash = hash * 29 + _xAdvanceWidthMultiplier.GetHashCode();
                hash = hash * 29 + _extraXAdvance.GetHashCode();
                return hash;
            }

            public bool Equals(TMProSpriteOptions other) {
                return other._scale == _scale &&
                    other._offsetSizeMultiplier == _offsetSizeMultiplier &&
                    other._extraOffset == _extraOffset &&
                    other._xAdvanceWidthMultiplier == _xAdvanceWidthMultiplier &&
                    other._extraXAdvance == _extraXAdvance;
            }

            public static bool operator ==(TMProSpriteOptions a, TMProSpriteOptions b) {
                return a._scale == b._scale &&
                    a._offsetSizeMultiplier == b._offsetSizeMultiplier &&
                    a._extraOffset == b._extraOffset &&
                    a._xAdvanceWidthMultiplier == b._xAdvanceWidthMultiplier &&
                    a._extraXAdvance == b._extraXAdvance;
            }

            public static bool operator !=(TMProSpriteOptions a, TMProSpriteOptions b) {
                return !(a == b);
            }
        }

        /// <summary>
        /// Sprite material properties.
        /// </summary>
        [Serializable]
        public struct SpriteMaterialProperties {

            [UnityEngine.Tooltip("Sprite material color.")]
            [UnityEngine.SerializeField]
            private UnityEngine.Color _color;
            public UnityEngine.Color color { get { return _color; } set { _color = value; } }

            public static SpriteMaterialProperties Default {
                get {
                    SpriteMaterialProperties o = new SpriteMaterialProperties();
                    o._color = UnityEngine.Color.white;
                    return o;
                }
            }
        }

        #region Text Mesh Pro Asset Version Support

        // These wrapper classes are required to support both Unity UI 1.0 and
        // Unity UI 2.0, or mores specifically, versions of Text Mesh Pro pre
        // and post asset v1.1.0 breaking structural changes were made.

        private interface ITMProSprite {
            uint id { get; set; }
            float width { get; set; }
            float height { get; set; }
            float xOffset { get; set; }
            float yOffset { get; set; }
            float xAdvance { get; set; }
            UnityEngine.Vector2 position { get; set; }
            UnityEngine.Vector2 pivot { get; set; }
            float scale { get; set; }
            string name { get; set; }
            uint unicode { get; set; }
            int hashCode { get; set; }
            UnityEngine.Sprite sprite { get; set; }
        }

        private interface ITMProSpriteAsset {
            int spriteCount { get; }
            UnityEngine.Texture spriteSheet { get; set; }
            TMPro.TMP_SpriteAsset GetSpriteAsset();
            ITMProSprite GetSprite(int index);
            void AddSprite(ITMProSprite sprite);
            bool Contains(string spriteName);
            void Clear();
            void UpdateLookupTables();
            void Destroy();
        }

        private static class TMProAssetVersionHelper {

#if MAY_SUPPORT_TMPRO_ASSET_V_1_1_0
            private static bool _isVersionSupportedChecked;

            private static bool CheckVersionSupported() {
                bool isVersionSupported = TMProSprite_AssetV1_1_0.CheckVersionSupported();
                if (_isVersionSupportedChecked) return isVersionSupported;
                if (!isVersionSupported) {
#if UNITY_EDITOR
                    //UnityEngine.Debug.LogWarning(typeof(UnityUITextMeshProGlyphHelper).Name + ": Unity UI 2.0 is not supported.");
#endif
                }
                _isVersionSupportedChecked = true;
                return isVersionSupported;
            }
#endif

            public static ITMProSprite CreateSprite() {
#if MAY_SUPPORT_TMPRO_ASSET_V_1_1_0
                return CheckVersionSupported() ? (ITMProSprite)new TMProSprite_AssetV1_1_0() : (ITMProSprite)new TMProSprite_AssetV1_0_0();
#else
                return new TMProSprite_AssetV1_0_0();
#endif
            }

            public static ITMProSpriteAsset CreateSpriteAsset() {
#if MAY_SUPPORT_TMPRO_ASSET_V_1_1_0
                return CheckVersionSupported() ? (ITMProSpriteAsset)new TMProSprite_AssetV1_1_0.TMPro_SpriteAsset() : (ITMProSpriteAsset)new TMProSprite_AssetV1_0_0.TMPro_SpriteAsset();
#else
                return new TMProSprite_AssetV1_0_0.TMPro_SpriteAsset();
#endif
            }
        }

        private class TMProSprite_AssetV1_0_0 : ITMProSprite {

            public TMPro.TMP_Sprite spriteInfo;

            public TMProSprite_AssetV1_0_0() {
                spriteInfo = new TMPro.TMP_Sprite();
            }

            public uint id {
                get {
                    return (uint)spriteInfo.id;
                }
                set {
                    spriteInfo.id = (int)value;
                }
            }
            public float width {
                get {
                    return spriteInfo.width;
                }
                set {
                    spriteInfo.width = value;
                }
            }

            public float height {
                get {
                    return spriteInfo.height;
                }
                set {
                    spriteInfo.height = value;
                }
            }

            public float xOffset {
                get {
                    return spriteInfo.xOffset;
                }
                set {
                    spriteInfo.xOffset = value;
                }
            }

            public float yOffset {
                get {
                    return spriteInfo.yOffset;
                }
                set {
                    spriteInfo.yOffset = value;
                }
            }

            public float xAdvance {
                get {
                    return spriteInfo.xAdvance;
                }
                set {
                    spriteInfo.xAdvance = value;
                }
            }

            public UnityEngine.Vector2 position {
                get {
                    return new UnityEngine.Vector2(spriteInfo.x, spriteInfo.y);
                }
                set {
                    spriteInfo.x = value.x;
                    spriteInfo.y = value.y;
                }
            }

            public UnityEngine.Vector2 pivot {
                get {
                    return spriteInfo.pivot;
                }
                set {
                    spriteInfo.pivot = value;
                }
            }

            public float scale {
                get {
                    return spriteInfo.scale;
                }
                set {
                    spriteInfo.scale = value;
                }
            }

            public string name {
                get {
                    return spriteInfo.name;
                }
                set {
                    spriteInfo.name = value;
                }
            }

            public uint unicode {
                get {
                    return (uint)spriteInfo.unicode;
                }
                set {
                    spriteInfo.unicode = (int)value;
                }
            }

            public int hashCode {
                get {
                    return spriteInfo.hashCode;
                }
                set {
                    spriteInfo.hashCode = value;
                }
            }

            public UnityEngine.Sprite sprite {
                get {
                    return spriteInfo.sprite;
                }
                set {
                    spriteInfo.sprite = value;
                }
            }

            public class TMPro_SpriteAsset : ITMProSpriteAsset {

                private TMPro.TMP_SpriteAsset _spriteAsset;
                private readonly List<TMProSprite_AssetV1_0_0> _sprites;

                public int spriteCount { get { return _sprites.Count; } }

                public UnityEngine.Texture spriteSheet {
                    get {
                        return _spriteAsset.spriteSheet;
                    }
                    set {
                        _spriteAsset.spriteSheet = value;
                    }
                }

                public TMPro_SpriteAsset() {
                    _spriteAsset = UnityEngine.ScriptableObject.CreateInstance<TMPro.TMP_SpriteAsset>();
                    _spriteAsset.hideFlags = UnityEngine.HideFlags.DontSave;
                    if (_spriteAsset.spriteInfoList == null) _spriteAsset.spriteInfoList = new List<TMPro.TMP_Sprite>();
                    _sprites = new List<TMProSprite_AssetV1_0_0>();
                }

                public TMPro.TMP_SpriteAsset GetSpriteAsset() {
                    return _spriteAsset;
                }

                public ITMProSprite GetSprite(int index) {
                    if ((uint)index >= (uint)_sprites.Count) return null;
                    return _sprites[index];
                }

                public void AddSprite(ITMProSprite sprite) {
                    TMProSprite_AssetV1_0_0 tSprite = sprite as TMProSprite_AssetV1_0_0;
                    if (sprite == null) throw new ArgumentException();
                    tSprite.spriteInfo.id = _spriteAsset.spriteInfoList.Count;
                    _spriteAsset.spriteInfoList.Add(tSprite.spriteInfo);
                    _sprites.Add(tSprite);
                }

                public void Clear() {
                    _spriteAsset.spriteInfoList.Clear();
                    _sprites.Clear();
                }

                public bool Contains(string spriteName) {
                    int count = _sprites.Count;
                    for (int i = 0; i < count; i++) {
                        if (string.Equals(_sprites[i].name, spriteName, StringComparison.Ordinal)) return true;
                    }
                    return false;
                }

                public void UpdateLookupTables() {
                    _spriteAsset.UpdateLookupTables();
                }

                public void Destroy() {
                    if (_spriteAsset == null) return;
                    UnityEngine.Object.Destroy(_spriteAsset);
                    _spriteAsset = null;
                }
            }
        }

#if MAY_SUPPORT_TMPRO_ASSET_V_1_1_0

        private class TMProSprite_AssetV1_1_0 : ITMProSprite {

            private readonly TMPro_SpriteGlyph _spriteGlyph;
            private readonly TMPro_SpriteCharacter _spriteCharacter;

            public TMProSprite_AssetV1_1_0() {
                _spriteGlyph = new TMPro_SpriteGlyph();
                _spriteCharacter = new TMPro_SpriteCharacter();
                _spriteCharacter.glyph = _spriteGlyph.source;
            }

            public TMPro_SpriteGlyph spriteGlyph {
                get {
                    return _spriteGlyph;
                }
            }

            public TMPro_SpriteCharacter spriteCharacter {
                get {
                    return _spriteCharacter;
                }
            }

            public uint id {
                get {
                    return _spriteGlyph.source.index;
                }
                set {
                    _spriteGlyph.source.index = value;
                    _spriteCharacter.glyphIndex = value;
                }
            }

            public float width {
                get {
                    return _spriteGlyph.source.metrics.width;
                }
                set {
                    UnityEngine.TextCore.GlyphMetrics m = _spriteGlyph.source.metrics;
                    m.width = value;
                    _spriteGlyph.source.metrics = m;
                    UnityEngine.TextCore.GlyphRect r = _spriteGlyph.source.glyphRect;
                    r.width = (int)value;
                    _spriteGlyph.source.glyphRect = r;
                }
            }

            public float height {
                get {
                    return _spriteGlyph.source.metrics.height;
                }
                set {
                    UnityEngine.TextCore.GlyphMetrics m = _spriteGlyph.source.metrics;
                    m.height = value;
                    _spriteGlyph.source.metrics = m;
                    UnityEngine.TextCore.GlyphRect r = _spriteGlyph.source.glyphRect;
                    r.height = (int)value;
                    _spriteGlyph.source.glyphRect = r;
                }
            }

            public float xOffset {
                get {
                    return _spriteGlyph.source.metrics.horizontalBearingX;
                }
                set {
                    UnityEngine.TextCore.GlyphMetrics m = _spriteGlyph.source.metrics;
                    m.horizontalBearingX = value;
                    _spriteGlyph.source.metrics = m;
                }
            }

            public float yOffset {
                get {
                    return _spriteGlyph.source.metrics.horizontalBearingY;
                }
                set {
                    UnityEngine.TextCore.GlyphMetrics m = _spriteGlyph.source.metrics;
                    m.horizontalBearingY = value;
                    _spriteGlyph.source.metrics = m;
                }
            }

            public float xAdvance {
                get {
                    return _spriteGlyph.source.metrics.horizontalAdvance;
                }
                set {
                    UnityEngine.TextCore.GlyphMetrics m = _spriteGlyph.source.metrics;
                    m.horizontalAdvance = value;
                    _spriteGlyph.source.metrics = m;
                }
            }

            public UnityEngine.Vector2 position {
                get {
                    var rect = _spriteGlyph.source.glyphRect;
                    return new UnityEngine.Vector2(rect.x, rect.y);
                }
                set {
                    UnityEngine.TextCore.GlyphRect r = _spriteGlyph.source.glyphRect;
                    r.x = (int)value.x;
                    r.y = (int)value.y;
                    _spriteGlyph.source.glyphRect = r;
                }
            }

            public UnityEngine.Vector2 pivot {
                get {
                    return new UnityEngine.Vector2();
                }
                set {
                }
            }

            public float scale {
                get {
                    return _spriteCharacter.scale;
                }
                set {
                    _spriteCharacter.scale = value;
                }
            }

            public string name {
                get {
                    return _spriteCharacter.name;
                }
                set {
                    _spriteCharacter.name = value;
                }
            }

            public uint unicode {
                get {
                    return _spriteCharacter.unicode;
                }
                set {
                    _spriteCharacter.unicode = value;
                }
            }

            public int hashCode {
                get {
                    return 0;
                }
                set {
                }
            }

            public UnityEngine.Sprite sprite {
                get {
                    return _spriteGlyph.sprite;
                }
                set {
                    _spriteGlyph.sprite = value;
                }
            }

            // Static

            private static bool? s_isVersionSupported;

            public static bool CheckVersionSupported() {
                if (s_isVersionSupported != null) return s_isVersionSupported.Value;
                try {
#if !SUPPORTS_AT_LEAST_TMPRO_ASSET_V_1_1_0
                    var a = new TMPro_SpriteCharacter();
                    var b = new TMPro_SpriteGlyph();
#endif
                    var c = new TMPro_SpriteAsset();
                    s_isVersionSupported = true;
                } catch {
                    s_isVersionSupported = false;
                }
                return s_isVersionSupported.Value;
            }

            // Classes

            public class TMPro_SpriteCharacter {

#if SUPPORTS_AT_LEAST_TMPRO_ASSET_V_1_1_0

                private readonly TMPro.TMP_SpriteCharacter _source;

                public TMPro.TMP_SpriteCharacter source {
                    get {
                        return _source;
                    }
                }

                public UnityEngine.TextCore.Glyph glyph {
                    get {
                        return _source.glyph;
                    }
                    set {
                        _source.glyph = value;
                    }
                }

                public uint unicode {
                    get {
                        return _source.unicode;
                    }
                    set {
                        if (value == 0x0) value = 0xFFFE;
                        _source.unicode = value;
                    }
                }

                public string name {
                    get {
                        return _source.name;
                    }
                    set {
                        _source.name = value;
                    }
                }

                public float scale {
                    get {
                        return _source.scale;
                    }
                    set {
                        _source.scale = value;
                    }
                }

                public uint glyphIndex {
                    get {
                        return _source.glyphIndex;
                    }
                    set {
                        _source.glyphIndex = value;
                    }
                }

                public TMPro_SpriteCharacter() {
                    _source = new TMPro.TMP_SpriteCharacter();
                }

#else

                private const string typeFullName = "TMPro.TMP_SpriteCharacter";

                private readonly object _source;
                private readonly PropertyInfo _glyph;
                private readonly PropertyInfo _unicode;
                private readonly PropertyInfo _name;
                private readonly PropertyInfo _scale;
                private readonly PropertyInfo _glyphIndex;

                public object source {
                    get {
                        return _source;
                    }
                }

                public UnityEngine.TextCore.Glyph glyph {
                    get {
                        return (UnityEngine.TextCore.Glyph)_glyph.GetValue(_source);
                    }
                    set {
                        _glyph.SetValue(_source, value);
                    }
                }

                public uint unicode {
                    get {
                        return (uint)_unicode.GetValue(_source);
                    }
                    set {
                        if (value == 0x0) value = 0xFFFE;
                        _unicode.SetValue(_source, value);
                    }
                }

                public string name {
                    get {
                        return (string)_name.GetValue(_source);
                    }
                    set {
                        _name.SetValue(_source, value);
                    }
                }

                public float scale {
                    get {
                        return (float)_scale.GetValue(_source);
                    }
                    set {
                        _scale.SetValue(_source, value);
                    }
                }

                public uint glyphIndex {
                    get {
                        return (uint)_glyphIndex.GetValue(_source);
                    }
                    set {
                        _glyphIndex.SetValue(_source, value);
                    }
                }

                public TMPro_SpriteCharacter() {
                    System.Type type = GetReflectedType();
                    if (type == null) throw new ArgumentNullException("type");
                    _source = System.Activator.CreateInstance(type);
                    if (_source == null) throw new ArgumentNullException("source");
                    _glyph = type.GetProperty("glyph", BindingFlags.Public | BindingFlags.Instance);
                    if (_glyph == null) throw new ArgumentNullException("glyph");
                    _unicode = type.GetProperty("unicode", BindingFlags.Public | BindingFlags.Instance);
                    if (_unicode == null) throw new ArgumentNullException("unicode");
                    _name = type.GetProperty("name", BindingFlags.Public | BindingFlags.Instance);
                    if (_name == null) throw new ArgumentNullException("name");
                    _scale = type.GetProperty("scale", BindingFlags.Public | BindingFlags.Instance);
                    if (_scale == null) throw new ArgumentNullException("scale");
                    _glyphIndex = type.GetProperty("glyphIndex", BindingFlags.Public | BindingFlags.Instance);
                    if (_glyphIndex == null) throw new ArgumentNullException("glyphIndex");
                }

                // Static

                private static System.Type s_type;

                private static System.Type GetReflectedType() {
                    if (s_type != null) return s_type;
                    System.Type[] types = typeof(TMPro.TMP_SpriteAsset).Assembly.GetTypes();
                    if (types == null) return null;
                    for(int i = 0; i < types.Length; i++) {
                        if (string.Equals(types[i].FullName, typeFullName)) {
                            s_type = types[i];
                            break;
                        }
                    }
                    return s_type;
                }
#endif
            }

            public class TMPro_SpriteGlyph {

#if SUPPORTS_AT_LEAST_TMPRO_ASSET_V_1_1_0

                private readonly TMPro.TMP_SpriteGlyph _source;

                public TMPro.TMP_SpriteGlyph source {
                    get {
                        return _source;
                    }
                }

                public UnityEngine.Sprite sprite {
                    get {
                        return _source.sprite;
                    }
                    set {
                        _source.sprite = value;
                    }
                }

                public TMPro_SpriteGlyph() {
                    _source = new TMPro.TMP_SpriteGlyph();
                    Initialize(_source);
                }

#else

                private const string typeFullName = "TMPro.TMP_SpriteGlyph";

                private readonly UnityEngine.TextCore.Glyph _source;

                private readonly FieldInfo _sprite;

                public UnityEngine.TextCore.Glyph source {
                    get {
                        return _source;
                    }
                }

                public UnityEngine.Sprite sprite {
                    get {
                        return (UnityEngine.Sprite)_sprite.GetValue(_source);
                    }
                    set {
                        _sprite.SetValue(_source, value);
                    }
                }

                public TMPro_SpriteGlyph() {
                    System.Type type = GetReflectedType();
                    if (type == null) throw new ArgumentNullException("type");
                    _source = (UnityEngine.TextCore.Glyph)System.Activator.CreateInstance(type);
                    if (_source == null) throw new ArgumentNullException("glyph");
                    _sprite = type.GetField("sprite", BindingFlags.Public | BindingFlags.Instance);
                    if (_sprite == null) throw new ArgumentNullException("sprite");
                    Initialize(_source);
                }

                // Static

                private static System.Type s_type;

                private static System.Type GetReflectedType() {
                    if (s_type != null) return s_type;
                    System.Type[] types = typeof(TMPro.TMP_SpriteAsset).Assembly.GetTypes();
                    if (types == null) return null;
                    for (int i = 0; i < types.Length; i++) {
                        if (string.Equals(types[i].FullName, typeFullName)) {
                            s_type = types[i];
                            break;
                        }
                    }
                    return s_type;
                }

#endif
                private static void Initialize(UnityEngine.TextCore.Glyph glyph) {
                    glyph.scale = 1.0f;
                    glyph.atlasIndex = 0;
                }
            }

            public class TMPro_SpriteAsset : ITMProSpriteAsset {

                // Cannot avoid using reflection in this class because TMPro.TMP_SpriteAsset
                // has internal setters for the tables and version in Unity UI 2.0. This
                // effectively prevents scripts from modfying the Sprite Asset tables at runtime.
                // This needs be changed back in a future version of Unity UI / TMPro.

                private readonly PropertyInfo _spriteCharacterTable;
                private readonly PropertyInfo _spriteGlyphTable;
                private readonly System.Collections.IList _spriteCharacterTableList;
                private readonly System.Collections.IList _spriteGlyphTableList;
                private readonly List<TMProSprite_AssetV1_1_0> _sprites;

                private TMPro.TMP_SpriteAsset _spriteAsset;

                public int spriteCount { get { return _sprites.Count; } }

                public UnityEngine.Texture spriteSheet {
                    get {
                        return _spriteAsset.spriteSheet;
                    }
                    set {
                        _spriteAsset.spriteSheet = value;
                    }
                }

                public TMPro_SpriteAsset() {
                    _spriteAsset = UnityEngine.ScriptableObject.CreateInstance<TMPro.TMP_SpriteAsset>();
                    _spriteAsset.hideFlags = UnityEngine.HideFlags.DontSave;

                    System.Type type = typeof(TMPro.TMP_SpriteAsset);
                    if (type == null) throw new ArgumentNullException("type");

                    // Set version to prevent TMPro asset version upgrade logging
                    PropertyInfo version = type.GetProperty("version", BindingFlags.Public | BindingFlags.Instance);
                    if (version == null) throw new ArgumentNullException("version");
                    version.SetValue(_spriteAsset, "1.1.0");
                    
                    _spriteCharacterTable = type.GetProperty("spriteCharacterTable", BindingFlags.Public | BindingFlags.Instance);
                    if (_spriteCharacterTable == null) throw new ArgumentNullException("spriteCharacterTable");
                    _spriteCharacterTableList = (System.Collections.IList)_spriteCharacterTable.GetValue(_spriteAsset);
                    if (_spriteCharacterTableList == null) throw new ArgumentNullException("spriteCharacterTableList");
                    _spriteGlyphTable = type.GetProperty("spriteGlyphTable", BindingFlags.Public | BindingFlags.Instance);
                    if (_spriteGlyphTable == null) throw new ArgumentNullException("spriteGlyphTable");
                    _spriteGlyphTableList = (System.Collections.IList)_spriteGlyphTable.GetValue(_spriteAsset);
                    if (_spriteGlyphTableList == null) throw new ArgumentNullException("spriteGlyphTableList");
                    _sprites = new List<TMProSprite_AssetV1_1_0>();
                }

                public TMPro.TMP_SpriteAsset GetSpriteAsset() {
                    return _spriteAsset;
                }

                public ITMProSprite GetSprite(int index) {
                    if ((uint)index >= (uint)_sprites.Count) return null;
                    return _sprites[index];
                }

                public void AddSprite(ITMProSprite sprite) {
                    TMProSprite_AssetV1_1_0 tSprite = sprite as TMProSprite_AssetV1_1_0;
                    if (tSprite == null) throw new ArgumentException();
                    tSprite.id = (uint)_spriteCharacterTableList.Count;
                    _spriteCharacterTableList.Add(tSprite.spriteCharacter.source);
                    _spriteGlyphTableList.Add(tSprite.spriteGlyph.source);
                    _sprites.Add(tSprite);
                }

                public void Clear() {
                    _spriteCharacterTableList.Clear();
                    _spriteGlyphTableList.Clear();
                    _sprites.Clear();
                }

                public bool Contains(string spriteName) {
                    int count = _sprites.Count;
                    for (int i = 0; i < count; i++) {
                        if (string.Equals(_sprites[i].name, spriteName, StringComparison.Ordinal)) return true;
                    }
                    return false;
                }

                public void UpdateLookupTables() {
                    _spriteAsset.UpdateLookupTables();
                }

                public void Destroy() {
                    if (_spriteAsset == null) return;
                    UnityEngine.Object.Destroy(_spriteAsset);
                    _spriteAsset = null;
                }
            }
        }
#endif

        #endregion

        #endregion

        #region Enums

        private enum DisplayType {
            Glyph,
            Text,
            GlyphOrText
        }

        #endregion
    }
}

#endif
