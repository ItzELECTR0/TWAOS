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

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif

#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif

#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif

#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif

#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif

#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif

#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif

#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif

#if UNITY_5_4_PLUS
#define SUPPORTS_SCENE_MANAGEMENT
#endif

#pragma warning disable 0649

namespace Rewired.Glyphs.Editor {
    using System;
    using System.Collections.Generic;
    using Rewired;
    using Rewired.Data;
    using Rewired.Data.Mapping;
    using Rewired.Editor.Libraries.Rotorz.ReorderableList;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [UnityEditor.CustomEditor(typeof(SpriteGlyphSet))]
    public sealed class SpriteGlyphSetInspector : UnityEditor.Editor {

        private const float imagePixelSize = 64f;
        private const float glyphsScrollViewHeight = 572f;

        private const string keyPathSeparator = "/";
        private const string keyControllerBasePath = "controller" + keyPathSeparator;
        private const string keyControllerTemplateBasePath = keyControllerBasePath + "template" + keyPathSeparator;
        private const string keyCustomControllerBasePath = keyControllerBasePath + "custom" + keyPathSeparator;
        private const string keyKeyboard = "keyboard";
        private const string keyMouse = "mouse";
        private const string keyTerm_axisVertical = "vertical";
        private const string keyTerm_axisHorizontal = "horizontal";
        private const string keyTerm_axisPositive = "positive";
        private const string keyTerm_axisNegative = "negative";

        private const string term_hardwareJoystickMap = "Hardware Joystick Map";
        private const string term_hardwareJoystickTemplateMap = "Hardware Joystick Template Map";
        private const string term_customController = "Custom Controller";

        const string generateReplaceWarningWindowTitle = "Generate Entries";
        const string generateReplaceButton = "Replace";
        const string generateCancelButton = "Cancel";
        const string generateReplacementMessage = "All existing entries will be deleted. Are you sure?";
        const string generateNoKeyWarningMessage = "{0} has no key. A unique key is required to generate values.";

        private const string fieldName_baseKeys = "_baseKeys";
        private const string fieldName_glyphs = "_glyphs";
        private const string fieldName_entry_key = "_key";
        private const string fieldName_entry_value = "_value";

        [NonSerialized]
        private UnityEngine.Vector2 _glyphsScrollPosition;
        [NonSerialized]
        private UnityEngine.GUIStyle _style_labelCentered;
        [NonSerialized]
        private UnityEngine.GUIStyle _style_box;
        [NonSerialized]
        private bool _stylesCreated;
        [NonSerialized]
        private ObjectPickerEventType _pendingObjectPickerEvent;
        [NonSerialized]
        private CustomControllerPickedEvent _pendingCustomControllerPickedEvent;

        new private SpriteGlyphSet target { get { return (SpriteGlyphSet)base.target; } }

        public override void OnInspectorGUI() {

            if (!_stylesCreated) TryCreateStyles();

            serializedObject.Update();

            // Process Unity object picker events
            {
                string commandName = UnityEngine.Event.current.commandName;
                if (commandName == "ObjectSelectorClosed") {
                    if (_pendingObjectPickerEvent != ObjectPickerEventType.None) {
                        UnityEngine.Object source = UnityEditor.EditorGUIUtility.GetObjectPickerObject();
                        if (source != null) {
                            HandleUnityObjectPickerResult(source, _pendingObjectPickerEvent);
                        }
                        _pendingObjectPickerEvent = ObjectPickerEventType.None;
                    }
                }
            }

            // Process Generate from Custom Controller window events
            if (_pendingCustomControllerPickedEvent != null) {
                GenerateFromCustomController(_pendingCustomControllerPickedEvent.userData, _pendingCustomControllerPickedEvent.customControllerId);
                _pendingCustomControllerPickedEvent = null;
            }

            {
                UnityEditor.SerializedProperty sp = serializedObject.FindProperty(fieldName_baseKeys);
                ReorderableListGUI.Title("Base Keys");
                if (sp.arraySize == 0 || string.IsNullOrEmpty(sp.GetArrayElementAtIndex(0).stringValue)) {
                    UnityEditor.EditorGUILayout.HelpBox("At least one base key is required.", UnityEditor.MessageType.Error);
                }
                ReorderableListGUI.ListField(sp);
            }

            {
                // Scroll view must be placed around glyphs only and set to a fixed height
                // because Unity 2019+ will size the scroll view to the entire vertical
                // contents, making it not even draw its scroll bars. Unity 5 - 2018 worked.
                // This is a workaround to draw a smaller scroll view within the inspector.
                _glyphsScrollPosition = UnityEditor.EditorGUILayout.BeginScrollView(
                    _glyphsScrollPosition,
                    false, true,
                    UnityEngine.GUILayout.Height(glyphsScrollViewHeight),
                    UnityEngine.GUILayout.ExpandWidth(true),
                    UnityEngine.GUILayout.ExpandHeight(false)
                );

                var adaptor = new SerializedPropertyAdaptor(serializedObject.FindProperty(fieldName_glyphs));
                adaptor.drawItemDelegate = DrawEntry;
                adaptor.FixedItemHeight = imagePixelSize;

                ReorderableListGUI.Title("Glyphs");
                ReorderableListGUI.ListField(adaptor);

                UnityEditor.EditorGUILayout.EndScrollView();
            }

            UnityEditor.EditorGUILayout.Separator();

            UnityEngine.GUILayout.BeginVertical(_style_box);
            UnityEngine.GUILayout.Label("Tools", _style_labelCentered);

            if (UnityEngine.GUILayout.Button(string.Format("Generate from {0}", term_hardwareJoystickMap))) {
                if (target.glyphs == null || target.glyphs.Count == 0 ||
                    UnityEditor.EditorUtility.DisplayDialog(generateReplaceWarningWindowTitle, generateReplacementMessage, generateReplaceButton, generateCancelButton)
                ) {
                    _pendingObjectPickerEvent = ObjectPickerEventType.GenerateFromHardwareJoystickMap;
                    int controlId = UnityEditor.EditorGUIUtility.GetControlID(UnityEngine.FocusType.Passive);
                    UnityEditor.EditorGUIUtility.ShowObjectPicker<HardwareJoystickMap>(null, false, "", controlId);
                }
            }
            if (UnityEngine.GUILayout.Button(string.Format("Generate from {0}", term_hardwareJoystickTemplateMap))) {
                if (target.glyphs == null || target.glyphs.Count == 0 ||
                    UnityEditor.EditorUtility.DisplayDialog(generateReplaceWarningWindowTitle, generateReplacementMessage, generateReplaceButton, generateCancelButton)
                ) {
                    _pendingObjectPickerEvent = ObjectPickerEventType.GenerateFromHardwareJoystickTemplateMap;
                    int controlId = UnityEditor.EditorGUIUtility.GetControlID(UnityEngine.FocusType.Passive);
                    UnityEditor.EditorGUIUtility.ShowObjectPicker<HardwareJoystickTemplateMap>(null, false, "", controlId);
                }
            }
            if (UnityEngine.GUILayout.Button(string.Format("Generate from {0}", term_customController))) {
                if (target.glyphs == null || target.glyphs.Count == 0 ||
                    UnityEditor.EditorUtility.DisplayDialog(generateReplaceWarningWindowTitle, generateReplacementMessage, generateReplaceButton, generateCancelButton)
                ) {
                    _pendingObjectPickerEvent = ObjectPickerEventType.GenerateFromCustomController;
                    int controlId = UnityEditor.EditorGUIUtility.GetControlID(UnityEngine.FocusType.Passive);
                    UnityEditor.EditorGUIUtility.ShowObjectPicker<UnityEngine.GameObject>(null, true, "Rewired Input Manager", controlId);
                }
            }
            if (UnityEngine.GUILayout.Button("Generate Keyboard")) {
                if (target.glyphs == null || target.glyphs.Count == 0 ||
                    UnityEditor.EditorUtility.DisplayDialog(generateReplaceWarningWindowTitle, generateReplacementMessage, generateReplaceButton, generateCancelButton)
                ) {
                    GenerateKeyboard();
                }
            }
            if (UnityEngine.GUILayout.Button("Generate Mouse")) {
                if (target.glyphs == null || target.glyphs.Count == 0 ||
                    UnityEditor.EditorUtility.DisplayDialog(generateReplaceWarningWindowTitle, generateReplacementMessage, generateReplaceButton, generateCancelButton)
                ) {
                    GenerateMouse();
                }
            }

            UnityEditor.EditorGUILayout.Separator();

            if (UnityEngine.GUILayout.Button("Auto Assign Sprites")) {
                bool hasSprites = false;
                if (target.glyphs != null) {
                    for(int i = 0; i < target.glyphs.Count; i++) {
                        if (target.glyphs[i] == null) continue;
                        if (target.glyphs[i].value != null) {
                            hasSprites = true;
                            break;
                        }
                    }
                }
                if (!hasSprites ||
                    UnityEditor.EditorUtility.DisplayDialog("Auto Assign Sprites", "Existing sprites will be replaced. Are you sure?", generateReplaceButton, generateCancelButton)
                ) {
                    _pendingObjectPickerEvent = ObjectPickerEventType.AutoAssignSprites;
                    int controlId = UnityEditor.EditorGUIUtility.GetControlID(UnityEngine.FocusType.Passive);
                    UnityEditor.EditorGUIUtility.ShowObjectPicker<UnityEngine.Texture2D>(null, false, "", controlId);
                }
            }

            UnityEngine.GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void TryCreateStyles() {
            _style_labelCentered = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.label);
            _style_labelCentered.alignment = UnityEngine.TextAnchor.MiddleCenter;
            _style_labelCentered.fontStyle = UnityEngine.FontStyle.Bold;
            _style_box = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.box);
            _style_box.padding = new UnityEngine.RectOffset(10, 10, 10, 10);
            _stylesCreated = true;
        }

        private void DrawEntry(UnityEditor.SerializedProperty property, UnityEngine.Rect position, int index) {

            bool isVisible = GetGlyphsScrollViewVisibleRect().Overlaps(position);
            if (!isVisible) return;

            UnityEditor.EditorGUI.PropertyField(
                new UnityEngine.Rect(
                    position.x,
                    position.y + (imagePixelSize * 0.5f) - (UnityEditor.EditorGUIUtility.singleLineHeight * 0.5f),
                    position.width - imagePixelSize - 20,
                    UnityEditor.EditorGUIUtility.singleLineHeight
                ),
                property.FindPropertyRelative(fieldName_entry_key),
                UnityEngine.GUIContent.none
            );

            UnityEditor.EditorGUI.ObjectField(
                new UnityEngine.Rect(
                    position.width - 34, // remove control width
                    position.y,
                    imagePixelSize,
                    imagePixelSize
                ),
                property.FindPropertyRelative(fieldName_entry_value),
                typeof(UnityEngine.Sprite),
                UnityEngine.GUIContent.none
            );
        }

        private void HandleUnityObjectPickerResult(UnityEngine.Object source, ObjectPickerEventType eventType) {
            bool handled = false;
            switch(eventType) {
                case ObjectPickerEventType.None:
                    return;
                case ObjectPickerEventType.GenerateFromHardwareJoystickMap:
                    if (source as HardwareJoystickMap != null) {
                        handled = GenerateFromHardwareJoystickMap((HardwareJoystickMap)source);
                    }
                    break;
                case ObjectPickerEventType.GenerateFromHardwareJoystickTemplateMap:
                    if (source as HardwareJoystickTemplateMap != null) {
                        handled = GenerateFromHardwareJoystickTemplateMap((HardwareJoystickTemplateMap)source);
                    }
                    break;
                case ObjectPickerEventType.GenerateFromCustomController:
                    if (source as UnityEngine.GameObject != null) {
                        UnityEngine.GameObject go = (UnityEngine.GameObject)source;
                        {
                            var inputManager = Rewired.Utils.UnityTools.GetComponentInSelfOrChildren<InputManager_Base>(go);
                            if (inputManager != null) {
                                handled = GenerateFromCustomControllerOpenPickerWindow(inputManager);
                            } else {
                                UnityEngine.Debug.LogError("Rewired: Object is not a Rewired Input Manager.");
                            }
                        }
                    }
                    break;
                case ObjectPickerEventType.AutoAssignSprites:
                    if (source as UnityEngine.Texture2D != null) {
                        handled = AutoAssignSprites((UnityEngine.Texture2D)source);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!handled) {
                UnityEngine.Debug.LogError("Rewired: Failed to handle object picker result: Event Type: " + eventType + ", Source Type: " + source.GetType());
            }
        }

        private bool GenerateFromHardwareJoystickMap(HardwareJoystickMap map) {
            if (map == null) return false;
            if (string.IsNullOrEmpty(map.Key)) {
                UnityEngine.Debug.LogWarning(string.Format(generateNoKeyWarningMessage, term_hardwareJoystickMap));
                return false;
            }

            string baseKey = keyControllerBasePath + map.Key;
            UnityEditor.SerializedProperty sp = serializedObject.FindProperty(fieldName_baseKeys);
            sp.arraySize = 1;
            sp.GetArrayElementAtIndex(0).stringValue = baseKey;

            var glyphsSp = serializedObject.FindProperty(fieldName_glyphs);
            glyphsSp.ClearArray();

            CreateEntries(glyphsSp, map.ElementIdentifiers);

            return true;
        }

        private bool GenerateFromHardwareJoystickTemplateMap(HardwareJoystickTemplateMap map) {
            if (map == null) return false;
            if (string.IsNullOrEmpty(map.Key)) {
                UnityEngine.Debug.LogWarning(string.Format(generateNoKeyWarningMessage, term_hardwareJoystickTemplateMap));
                return false;
            }

            string baseKey = keyControllerTemplateBasePath + map.Key;
            UnityEditor.SerializedProperty sp = serializedObject.FindProperty(fieldName_baseKeys);
            sp.arraySize = 1;
            sp.GetArrayElementAtIndex(0).stringValue = baseKey;

            var glyphsSp = serializedObject.FindProperty(fieldName_glyphs);
            glyphsSp.ClearArray();

            CreateEntries(glyphsSp, map.ElementIdentifiers);

            return true;
        }

        private bool GenerateFromCustomControllerOpenPickerWindow(InputManager_Base inputManager) {
            if (inputManager == null || inputManager.userData == null) return false;
            CustomControllerPickerWindow window = (CustomControllerPickerWindow)UnityEditor.EditorWindow.GetWindow(typeof(CustomControllerPickerWindow));
            window.SetInitOptions(
                new CustomControllerPickerWindow.InitOptions(
                    inputManager.userData,
                    (userData, id) => _pendingCustomControllerPickedEvent = new CustomControllerPickedEvent(userData, id)
                )
            );
#if UNITY_5_2_PLUS
            window.titleContent = new UnityEngine.GUIContent("Generate Entries");
#else
            window.title = "Generate Entries";
#endif
            window.Show();

            return true;
        }

        private void GenerateFromCustomController(UserData userData, int id) {
            var cc = userData.GetCustomControllerById(id);
            if (cc == null) return;
            if (string.IsNullOrEmpty(cc.key)) {
                UnityEngine.Debug.LogWarning(string.Format(generateNoKeyWarningMessage, term_customController));
                return;
            }

            string baseKey = keyCustomControllerBasePath + cc.key;
            UnityEditor.SerializedProperty sp = serializedObject.FindProperty(fieldName_baseKeys);
            sp.arraySize = 1;
            sp.GetArrayElementAtIndex(0).stringValue = baseKey;

            var glyphsSp = serializedObject.FindProperty(fieldName_glyphs);
            glyphsSp.ClearArray();

            CreateEntries(glyphsSp, cc.ElementIdentifiers);
        }

        private void GenerateKeyboard() {

            string baseKey = keyControllerBasePath + keyKeyboard;
            UnityEditor.SerializedProperty sp = serializedObject.FindProperty(fieldName_baseKeys);
            sp.arraySize = 1;
            sp.GetArrayElementAtIndex(0).stringValue = baseKey;

            var glyphsSp = serializedObject.FindProperty(fieldName_glyphs);
            glyphsSp.ClearArray();

            foreach (string key in Consts.keyboardKeyKeys) {
                if (string.IsNullOrEmpty(key)) continue; // skip "None"
                TryAddEntry(glyphsSp, key);
            }

            foreach (string key in Consts.keyboardModifierKeyKeys) {
                if (string.IsNullOrEmpty(key)) continue; // skip "None"
                TryAddEntry(glyphsSp, key);
            }
        }

        private void GenerateMouse() {

            string baseKey = keyControllerBasePath + keyMouse;
            UnityEditor.SerializedProperty sp = serializedObject.FindProperty(fieldName_baseKeys);
            sp.arraySize = 1;
            sp.GetArrayElementAtIndex(0).stringValue = baseKey;

            var glyphsSp = serializedObject.FindProperty(fieldName_glyphs);
            glyphsSp.ClearArray();

            TryAddEntry(glyphsSp, "move");
            TryAddEntry(glyphsSp, "move/horizontal");
            TryAddEntry(glyphsSp, "move/right");
            TryAddEntry(glyphsSp, "move/left");
            TryAddEntry(glyphsSp, "move/vertical");
            TryAddEntry(glyphsSp, "move/up");
            TryAddEntry(glyphsSp, "move/down");
            TryAddEntry(glyphsSp, "wheel/vertical");
            TryAddEntry(glyphsSp, "wheel/up");
            TryAddEntry(glyphsSp, "wheel/down");
            TryAddEntry(glyphsSp, "wheel/horizontal");
            TryAddEntry(glyphsSp, "wheel/right");
            TryAddEntry(glyphsSp, "wheel/left");
            TryAddEntry(glyphsSp, "left_button");
            TryAddEntry(glyphsSp, "right_button");
            TryAddEntry(glyphsSp, "middle_button");
            TryAddEntry(glyphsSp, "button_4");
            TryAddEntry(glyphsSp, "button_5");
            TryAddEntry(glyphsSp, "button_6");
            TryAddEntry(glyphsSp, "button_7");
        }

        private void CreateEntries(UnityEditor.SerializedProperty glyphsSp, IEnumerable<Rewired.ControllerElementIdentifier> elementIdentifiers) {
            foreach (var ei in elementIdentifiers) {
                if (!TryAddEntry(glyphsSp, ei.key)) {
                    UnityEngine.Debug.LogWarning(string.Format(generateNoKeyWarningMessage, ei.name));
                    continue;
                }
                switch (ei.elementType) {
                    case ControllerElementType.Axis: {
                            string key = ei.positiveKey;
                            if (string.IsNullOrEmpty(key)) key = ei.key + keyPathSeparator + keyTerm_axisPositive; // generate missing keys
                            if (!string.Equals(ei.key, key, StringComparison.Ordinal)) {
                                TryAddEntry(glyphsSp, key);
                            }
                        } {
                            string key = ei.negativeKey;
                            if (string.IsNullOrEmpty(key)) key = ei.key + keyPathSeparator + keyTerm_axisNegative; // generate missing keys
                            if (!string.Equals(ei.key, key, StringComparison.Ordinal)) {
                                TryAddEntry(glyphsSp, key);
                            }
                        }
                        break;
                    case ControllerElementType.Button:
                        break;
                    case ControllerElementType.CompoundElement:
                        switch (ei.compoundElementType) {
                            case CompoundControllerElementType.Axis2D:
                                break;
                            case CompoundControllerElementType.Hat:
                                break;
                            case CompoundControllerElementType.DPad:
                                // Add vertical and horizontal values
                                if (!string.IsNullOrEmpty(ei.key)) {
                                    TryAddEntry(glyphsSp, ei.key + keyPathSeparator + keyTerm_axisHorizontal);
                                    TryAddEntry(glyphsSp, ei.key + keyPathSeparator + keyTerm_axisVertical);
                                }
                                break;
                        }
                        break;
                    default:
                        UnityEngine.Debug.LogError("Rewired: Unknown ControllerElementType: " + ei.elementType);
                        break;
                }
            }
        }
        private void CreateEntries(UnityEditor.SerializedProperty glyphsSp, IEnumerable<ControllerTemplateElementIdentifier> elementIdentifiers) {
            foreach (var ei in elementIdentifiers) {
                if (!TryAddEntry(glyphsSp, ei.key)) {
                    UnityEngine.Debug.LogWarning(string.Format(generateNoKeyWarningMessage, ei.name));
                    continue;
                }
                switch (ei.elementType) {
                    case ControllerTemplateElementType.Axis: {
                            string key = ei.positiveKey;
                            if (string.IsNullOrEmpty(key)) key = ei.key + keyPathSeparator + keyTerm_axisPositive; // generate missing keys
                            if (!string.Equals(ei.key, key, StringComparison.Ordinal)) {
                                TryAddEntry(glyphsSp, key);
                            }
                        } {
                            string key = ei.negativeKey;
                            if (string.IsNullOrEmpty(key)) key = ei.key + keyPathSeparator + keyTerm_axisNegative; // generate missing keys
                            if (!string.Equals(ei.key, key, StringComparison.Ordinal)) {
                                TryAddEntry(glyphsSp, key);
                            }
                        }
                        break;
                    case ControllerTemplateElementType.Button:
                        break;
                    case ControllerTemplateElementType.DPad:
                        // Add vertical and horizontal values
                        if (!string.IsNullOrEmpty(ei.key)) {
                            TryAddEntry(glyphsSp, ei.key + keyPathSeparator + keyTerm_axisHorizontal);
                            TryAddEntry(glyphsSp, ei.key + keyPathSeparator + keyTerm_axisVertical);
                        }
                        break;
                    case ControllerTemplateElementType.Hat:
                        break;
                    case ControllerTemplateElementType.Stick:
                        break;
                    case ControllerTemplateElementType.Stick6D:
                        break;
                    case ControllerTemplateElementType.Throttle:
                        break;
                    case ControllerTemplateElementType.ThumbStick:
                        break;
                    case ControllerTemplateElementType.Yoke:
                        break;
                }
            }
        }

        private bool AutoAssignSprites(UnityEngine.Texture2D texture) {
            if (texture == null) return false;
            if (target.glyphs == null || target.glyphs.Count == 0) return false;

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);

            var objects = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            if (objects == null) return false;

            List<UnityEngine.Sprite> sprites = new List<UnityEngine.Sprite>();
            foreach (var o in objects) {
                if (o is UnityEngine.Sprite) {
                    sprites.Add((UnityEngine.Sprite)o);
                }
            }
            if (sprites == null || sprites.Count == 0) {
                UnityEngine.Debug.Log(assetPath);
                return false;
            }

            var glyphsSp = serializedObject.FindProperty(fieldName_glyphs);

            string key;
            for (int i = 0; i < glyphsSp.arraySize; i++) {
                try {
                    bool found = false;
                    for (int j = 0; j < sprites.Count; j++) {
                        key = glyphsSp.GetArrayElementAtIndex(i).FindPropertyRelative(fieldName_entry_key).stringValue.Replace('/', '_');
                        if (!string.IsNullOrEmpty(key)) {
                            if (string.Equals(sprites[j].name, key, StringComparison.Ordinal)) {
                                glyphsSp.GetArrayElementAtIndex(i).FindPropertyRelative(fieldName_entry_value).objectReferenceValue = sprites[j];
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found) {
                        for (int j = 0; j < sprites.Count; j++) {
                            key = glyphsSp.GetArrayElementAtIndex(i).FindPropertyRelative(fieldName_entry_key).stringValue.Replace('/', '_');
                            if (!string.IsNullOrEmpty(key)) {
                                if (sprites[j].name.Contains(key)) {
                                    glyphsSp.GetArrayElementAtIndex(i).FindPropertyRelative(fieldName_entry_value).objectReferenceValue = sprites[j];
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                } catch (ArgumentException) {
                    UnityEngine.Debug.LogError("Rewired: Duplicate key detected. Keys must be unique.");
                }
            }

            return true;
        }

        private UnityEngine.Rect GetGlyphsScrollViewVisibleRect() {
            return new UnityEngine.Rect(
                _glyphsScrollPosition.x,
                _glyphsScrollPosition.y,
                // This is the entire inspector width. Not exactly correct, but should be good enough.
                UnityEngine.Screen.width,
                glyphsScrollViewHeight
            );
        }

        private static bool TryAddEntry(UnityEditor.SerializedProperty arraySp, string key) {
            if (string.IsNullOrEmpty(key)) return false;
            arraySp.InsertArrayElementAtIndex(arraySp.arraySize);
            var item = arraySp.GetArrayElementAtIndex(arraySp.arraySize - 1);
            item.FindPropertyRelative(fieldName_entry_key).stringValue = key;
            return true;
        }

        private class SerializedPropertyAdaptor : Rewired.Editor.Libraries.Rotorz.ReorderableList.SerializedPropertyAdaptor {

            public delegate void DrawItemDelegate(UnityEditor.SerializedProperty property, UnityEngine.Rect position, int index);
            public delegate float GetItemHeightDelegate(UnityEditor.SerializedProperty property, int index);

            public DrawItemDelegate drawItemDelegate { get; set; }
            public GetItemHeightDelegate getItemHeightDelegate { get; set; }

            public SerializedPropertyAdaptor(UnityEditor.SerializedProperty arrayProperty) : base(arrayProperty) {
            }

            public SerializedPropertyAdaptor(UnityEditor.SerializedProperty arrayProperty, float fixedItemHeight) : base(arrayProperty, fixedItemHeight) {
            }

            public override void DrawItem(UnityEngine.Rect position, int index) {
                if (drawItemDelegate != null) {
                    drawItemDelegate(this[index], position, index);
                } else {
                    UnityEditor.EditorGUI.PropertyField(position, this[index], UnityEngine.GUIContent.none, true);
                }
            }

            public override float GetItemHeight(int index) {
                if (FixedItemHeight != 0f) return FixedItemHeight;
                if (getItemHeightDelegate != null) return getItemHeightDelegate(this[index], index);
                return UnityEditor.EditorGUI.GetPropertyHeight(this[index], UnityEngine.GUIContent.none, true);
            }
        }

        private class CustomControllerPickerWindow : UnityEditor.EditorWindow {

            private InitOptions _options;

            public class InitOptions {
                public readonly UserData userData;
                public readonly Action<UserData, int> customControllerSelectedCallback;

                public InitOptions(
                    UserData userData,
                    Action<UserData, int> customControllerSelectedCallback
                ) {
                    if (userData == null) throw new ArgumentNullException("userData");
                    if (customControllerSelectedCallback == null) throw new ArgumentNullException("customControllerSelectedCallback");
                    this.userData = userData;
                    this.customControllerSelectedCallback = customControllerSelectedCallback;
                }
            }

            public void SetInitOptions(InitOptions options) {
                if (options == null) throw new ArgumentNullException();
                _options = options;
            }

            private void OnGUI() {
                if (_options == null) return;

                UnityEditor.EditorGUILayout.Space();

                int[] ids = _options.userData.GetCustomControllerIds();

                if (ids == null || ids.Length == 0) {
                    UnityEngine.GUILayout.Label(string.Format("No {0}s found.", term_customController));
                    return;
                }

                UnityEngine.GUILayout.BeginVertical();

                UnityEngine.GUILayout.Label(string.Format("Select a {0}", term_customController));

                int selectedId = -1;

                foreach (int id in ids) {
                    var cc = _options.userData.GetCustomControllerById(id);
                    if (cc == null) continue;

                    bool guiEnabled = UnityEngine.GUI.enabled;

                    if (string.IsNullOrEmpty(cc.key)) {
                        UnityEditor.EditorGUILayout.HelpBox(string.Format(generateNoKeyWarningMessage, cc.name), UnityEditor.MessageType.Warning);
                        UnityEngine.GUI.enabled = false;
                    }

                    if (UnityEngine.GUILayout.Button(cc.name)) {
                        selectedId = id;
                    }

                    UnityEngine.GUI.enabled = guiEnabled;
                }

                UnityEngine.GUILayout.EndVertical();

                if (selectedId >= 0) {
                    _options.customControllerSelectedCallback(_options.userData, selectedId);
                    Close();
                }
            }
        }

        private class CustomControllerPickedEvent {
            public readonly UserData userData;
            public readonly int customControllerId;

            public CustomControllerPickedEvent(
                UserData userData,
                int customControllerId
            ) {
                this.userData = userData;
                this.customControllerId = customControllerId;
            }
        }

        private enum ObjectPickerEventType {
            None = 0,
            GenerateFromHardwareJoystickMap,
            GenerateFromHardwareJoystickTemplateMap,
            GenerateFromCustomController,
            AutoAssignSprites
        }
    }
}
