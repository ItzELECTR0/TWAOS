// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Integration.Cinemachine.Editor {

    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Rewired;
    using Rewired.Data;
    using Rewired.Utils;

    [CustomEditor(typeof(RewiredCinemachineBridge))]
    public class RewiredCinemachineBridgeInspector : UnityEditor.Editor {

        private const string editorPrefsKeyBase = "Rewired.Integration.Cinemachine.Editor.RewiredCinemachineBridgeInspector";
        private const float miniButtonWidth = 20f;

        #region Inspector Variable Name Consts

        private const string c_rewiredInputManager = "_rewiredInputManager";
        private const string c_absoluteAxisSensitivity = "_absoluteAxisSensitivity";
        private const string c_scaleAbsoluteAxesToScreen = "_scaleAbsoluteAxesToScreen";
        private const string c_playerMappings = "_playerMappings";
        private const string c_runInEditMode = "_runInEditMode";

        private const string c_playerMapping_playerId = "_playerId";
        private const string c_playerMapping_actionMappings = "_actionMappings";

        private const string c_actionMapping_cinemachineAxis = "_cinemachineAxis";
        private const string c_actionMapping_rewiredActionName = "_rewiredActionName";
        private const string c_actionMapping_rewiredActionId = "_rewiredActionId";

        #endregion

        #region Working Vars

        private Dictionary<string, SerializedProperty> properties;
        private List<int> _actionIds;
        private List<string> _actionNames;
        private List<int> _playerIds;
        private List<string> _playerNames;
        private List<ModifyArrayEvent> _modifyPlayerArrayEvents;
        private List<ModifyArrayEvent> _modifyActionArrayEvents;

        private GUIStyle _boxStyle;
        private GUIStyle _errorLabel;

        #endregion

        #region Properties

        private InputManager inputManager { get { return properties[c_rewiredInputManager].objectReferenceValue as InputManager; } }
        private UserData userData { get { return inputManager != null ? inputManager.userData : null; } }

        #endregion

        #region MonoBehaviour Events

        protected virtual void OnEnable() {
            properties = new Dictionary<string, SerializedProperty>();
            _actionIds = new List<int>();
            _actionNames = new List<string>();
            _playerIds = new List<int>();
            _playerNames = new List<string>();
            _modifyPlayerArrayEvents = new List<ModifyArrayEvent>();
            _modifyActionArrayEvents = new List<ModifyArrayEvent>();

            properties.Add(c_rewiredInputManager, serializedObject.FindProperty(c_rewiredInputManager));
            properties.Add(c_absoluteAxisSensitivity, serializedObject.FindProperty(c_absoluteAxisSensitivity));
            properties.Add(c_scaleAbsoluteAxesToScreen, serializedObject.FindProperty(c_scaleAbsoluteAxesToScreen));
            properties.Add(c_playerMappings, serializedObject.FindProperty(c_playerMappings));
            properties.Add(c_runInEditMode, serializedObject.FindProperty(c_runInEditMode));
        }

        public override void OnInspectorGUI() {
            if(_boxStyle == null) {
                _boxStyle = new GUIStyle(GUI.skin.box);
                _boxStyle.padding = new RectOffset(5, 5, 5, 5);
                _errorLabel = new GUIStyle(GUI.skin.label);
                _errorLabel.fontStyle = FontStyle.Bold;
                _errorLabel.normal.textColor = Color.red;
            }
            serializedObject.Update();

            EditorGUILayout.Space();

            // Rewired Input Manager link
            SerializedProperty rewiredInputManagerSP = properties[c_rewiredInputManager];
            InputManager_Base inputManager = rewiredInputManagerSP.objectReferenceValue as InputManager_Base;
            bool rimIsOnSameGameObject = false;
            if(inputManager == null) {
                inputManager = (target as Behaviour).GetComponent<InputManager_Base>();
                if(inputManager != null) {
                    rimIsOnSameGameObject = true;
                    if(rewiredInputManagerSP.objectReferenceValue != null) rewiredInputManagerSP.objectReferenceValue = null;
                }
            }
            if(!rimIsOnSameGameObject) {
                EditorGUILayout.PropertyField(rewiredInputManagerSP);
                if(rewiredInputManagerSP.objectReferenceValue == null) {
                    EditorGUILayout.Space();
                    if(GUILayout.Button("Find Rewired Input Manager")) {
                        inputManager = UnityEngine.Object.FindObjectOfType<InputManager_Base>();
                        if(inputManager == null) {
                            Debug.LogWarning("Rewired: No Rewired Input Manager found in the scene.");
                        } else {
                            rewiredInputManagerSP.objectReferenceValue = inputManager;
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Link the Rewired Input Manager you are using here for easier access to Actions, Players, etc. " +
                        "This will allow you to select Actions from drop downs instead of having to use ids to select items.\n\n" +
                        "If you are using a Prefab, it is recommended that you link the on-disk Prefab parent here instead of the scene instance.",
                        MessageType.Info
                    );
                    EditorGUILayout.Space();
                }

                EditorGUILayout.Separator();
            }

            EditorGUILayout.PropertyField(properties[c_absoluteAxisSensitivity]);
            EditorGUILayout.PropertyField(properties[c_scaleAbsoluteAxesToScreen]);
            EditorGUILayout.PropertyField(properties[c_runInEditMode]);

            EditorGUILayout.Separator();

            // Mappings
            EditorGUILayout.LabelField("Action Mappings", EditorStyles.boldLabel);

            if(Application.isPlaying || properties[c_runInEditMode].boolValue) {

                EditorGUILayout.HelpBox("Action mappings can not be edited while the Rewired Cinemachine Bridge is running.", MessageType.Warning);

            } else {

                if(inputManager != null) {
                    UserData userData = inputManager.userData;
                    userData.GetActionIds(_actionIds);
                    userData.GetActionNames(_actionNames);
                    _actionIds.Insert(0, -1);
                    _actionNames.Insert(0, "None");
                    userData.GetPlayerRuntimeIds(_playerIds);
                    userData.GetPlayerNames(_playerNames);
                }

                SerializedProperty mappingsSP = properties[c_playerMappings];
                EditorGUI.indentLevel++;

                float width = EditorGUIUtility.currentViewWidth;

                for(int i = 0; i < mappingsSP.arraySize; i++) {
                    SerializedProperty mappingSP = mappingsSP.GetArrayElementAtIndex(i);
                    SerializedProperty playerIdSP = mappingSP.FindPropertyRelative(c_playerMapping_playerId);
                    SerializedProperty actionMappingsSP = mappingSP.FindPropertyRelative(c_playerMapping_actionMappings);

                    GUILayout.BeginVertical(_boxStyle, GUILayout.ExpandWidth(false));
                    {
                        bool open;
                        EditorGUILayout.BeginHorizontal();
                        {
                            string key = editorPrefsKeyBase + "_playerMapping_" + i;
                            EditorGUI.BeginChangeCheck();
                            open = EditorGUILayout.Foldout(EditorPrefs.GetBool(key, true), "Entry " + i.ToString());
                            if(EditorGUI.EndChangeCheck()) EditorPrefs.SetBool(key, open);
                            GUILayout.FlexibleSpace();
                            if(GUILayout.Button("▲", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false))) {
                                _modifyPlayerArrayEvents.Add(new ModifyArrayEvent() { command = ModifyArrayEvent.Cmd.MoveUp, index = i });
                            }
                            if(GUILayout.Button("▼", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false))) {
                                _modifyPlayerArrayEvents.Add(new ModifyArrayEvent() { command = ModifyArrayEvent.Cmd.MoveDown, index = i });
                            }
                            if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false), GUILayout.Width(miniButtonWidth))) {
                                _modifyPlayerArrayEvents.Add(new ModifyArrayEvent() { command = ModifyArrayEvent.Cmd.Insert, index = i });
                            }
                            if(GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false), GUILayout.Width(miniButtonWidth))) {
                                _modifyPlayerArrayEvents.Add(new ModifyArrayEvent() { command = ModifyArrayEvent.Cmd.Remove, index = i });
                            }
                        }
                        EditorGUILayout.EndVertical();

                        if(open) {
                            EditorGUI.indentLevel++;

                            EditorGUIUtility.labelWidth = 95f;
                            EditorGUIUtility.fieldWidth = Mathf.Max(width * 0.2f, 100f);

                            if(inputManager != null) {
                                int result = EditorGUILayout.IntPopup("Player", playerIdSP.intValue, _playerNames.ToArray(), _playerIds.ToArray(), GUILayout.ExpandWidth(false));
                                if(result != playerIdSP.intValue) playerIdSP.intValue = result;
                            } else {
                                EditorGUILayout.PropertyField(playerIdSP, GUILayout.ExpandWidth(false));
                            }

                            if(IsPlayerIdConflict(mappingsSP, playerIdSP.intValue, i)) {
                                EditorGUILayout.LabelField("Duplicate Player Id.", _errorLabel);
                            }

                            GUILayout.Space(5f);
                            EditorGUILayout.LabelField(new GUIContent("Actions"));

                            if(actionMappingsSP.arraySize == 0) {
                                EditorGUILayout.LabelField("Press \"Add Action\" to add an Action.");
                                if(GUILayout.Button("Add Action")) {
                                    actionMappingsSP.InsertArrayElementAtIndex(actionMappingsSP.arraySize);
                                }
                            }
                            for(int j = 0; j < actionMappingsSP.arraySize; j++) {

                                EditorGUIUtility.labelWidth = 140f;
                                EditorGUIUtility.fieldWidth = Mathf.Max(width * 0.2f, 100f);

                                SerializedProperty actionMappingSP = actionMappingsSP.GetArrayElementAtIndex(j);
                                SerializedProperty unityAxisSP = actionMappingSP.FindPropertyRelative(c_actionMapping_cinemachineAxis);

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.PropertyField(unityAxisSP);
                                    GUILayout.Space(-25f);

                                    SerializedProperty rewiredActionNameSP = actionMappingSP.FindPropertyRelative(c_actionMapping_rewiredActionName);
                                    SerializedProperty rewiredActionIdSP = actionMappingSP.FindPropertyRelative(c_actionMapping_rewiredActionId);

                                    EditorGUIUtility.labelWidth = 125f;

                                    if(inputManager != null) {

                                        int result = EditorGUILayout.IntPopup("Rewired Action", rewiredActionIdSP.intValue, _actionNames.ToArray(), _actionIds.ToArray(), GUILayout.ExpandWidth(false));
                                        if(result != rewiredActionIdSP.intValue) rewiredActionIdSP.intValue = result;

                                    } else {
                                        EditorGUILayout.PropertyField(rewiredActionNameSP, new GUIContent("Rewired Action"), GUILayout.ExpandWidth(false));
                                    }

                                    GUILayout.Space(-10f);
                                    GUILayout.FlexibleSpace();
                                    GUILayout.BeginVertical();
                                    {
                                        if(GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false), GUILayout.Width(miniButtonWidth))) {
                                            _modifyActionArrayEvents.Add(new ModifyArrayEvent() { command = ModifyArrayEvent.Cmd.Remove, index = j });
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                EditorGUILayout.EndHorizontal();

                                if(IsUnityAxisConflict(mappingsSP, actionMappingsSP, unityAxisSP.stringValue, i, j)) {
                                    EditorGUILayout.LabelField("Duplicate Cinemachine Axis name.", _errorLabel);
                                }
                            }

                            if(actionMappingsSP.arraySize > 0) {
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.FlexibleSpace();
                                    if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false), GUILayout.Width(miniButtonWidth))) {
                                        _modifyActionArrayEvents.Add(new ModifyArrayEvent() { command = ModifyArrayEvent.Cmd.Insert, index = actionMappingsSP.arraySize });
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }

                            ModifyArray(actionMappingsSP, _modifyActionArrayEvents);

                            EditorGUI.indentLevel--;
                        } else {
                            bool error = false;
                            if(IsPlayerIdConflict(mappingsSP, playerIdSP.intValue, i)) {
                                error = true;
                            }
                            for(int j = 0; j < actionMappingsSP.arraySize; j++) {
                                if(IsUnityAxisConflict(mappingsSP, actionMappingsSP, actionMappingsSP.GetArrayElementAtIndex(j).FindPropertyRelative(c_actionMapping_cinemachineAxis).stringValue, i, j)) {
                                    error = true;
                                }
                            }
                            if(error) EditorGUILayout.HelpBox("This entry contains errors which must be fixed.", MessageType.Error);
                        }
                    }
                    GUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;

                ModifyArray(mappingsSP, _modifyPlayerArrayEvents);

                if(mappingsSP.arraySize == 0) {
                    EditorGUILayout.LabelField("Press \"Add Entry\" to add an entry.");
                    if(GUILayout.Button("Add Entry")) {
                        mappingsSP.InsertArrayElementAtIndex(mappingsSP.arraySize);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            //base.DrawDefaultInspector();
        }

        private static void ModifyArray(SerializedProperty arrayProperty, List<ModifyArrayEvent> modifyArrayEvents) {
            // Process array modification events
            for(int i = modifyArrayEvents.Count - 1; i >= 0; i--) {
                int index = modifyArrayEvents[i].index;
                if(modifyArrayEvents[i].command == ModifyArrayEvent.Cmd.Remove) {
                    arrayProperty.DeleteArrayElementAtIndex(index);
                } else if(modifyArrayEvents[i].command == ModifyArrayEvent.Cmd.MoveUp) {
                    if(index > 0) arrayProperty.MoveArrayElement(index, index - 1);
                } else if(modifyArrayEvents[i].command == ModifyArrayEvent.Cmd.MoveDown) {
                    if(index < arrayProperty.arraySize - 1) arrayProperty.MoveArrayElement(index, index + 1);
                } else if(modifyArrayEvents[i].command == ModifyArrayEvent.Cmd.Insert) {
                    arrayProperty.InsertArrayElementAtIndex(index);
                }
                modifyArrayEvents.Clear();
            }
        }

        private static bool IsPlayerIdConflict(SerializedProperty mappingsSP, int playerId, int index) {
            for(int i = 0; i < index; i++) {
                if(mappingsSP.GetArrayElementAtIndex(i).FindPropertyRelative(c_playerMapping_playerId).intValue == playerId) return true;
            }
            return false;
        }

        private static bool IsUnityAxisConflict(SerializedProperty mappingsSP, SerializedProperty actionMappingsSP, string axisName, int mappingsIndex, int actionsIndex) {
            for(int i = 0; i <= mappingsIndex; i++) {
                var thisActionMappingsSP = mappingsSP.GetArrayElementAtIndex(i).FindPropertyRelative(c_playerMapping_actionMappings);
                int count = i == mappingsIndex ? actionsIndex : thisActionMappingsSP.arraySize;
                for(int j = 0; j < count; j++) {
                    if(i == mappingsIndex && j == actionsIndex) return false; // don't check against self
                    string thisName = thisActionMappingsSP.GetArrayElementAtIndex(j).FindPropertyRelative(c_actionMapping_cinemachineAxis).stringValue;
                    if(thisName == axisName) return true;
                }
            }
            return false;
        }

        private struct ModifyArrayEvent {

            public Cmd command;
            public int index;

            public enum Cmd {
                Remove,
                MoveUp,
                MoveDown,
                Insert
            }
        }

        #endregion
    }
}