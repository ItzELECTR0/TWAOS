using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Editor
{
    [ProjectSettingsSection(SettingsPath = k_SettingsPath)]
    internal class MultiplayerRolesSettingsSection : VisualElement
    {
        private const string k_SettingsPath = ProjectSettingsProvider.k_SettingsGroupPath + "Multiplayer Roles";

        private AutomaticSelectionOptions m_AutomaticSelectionOptions;

        private VisualElement m_ContentSelectionContainer;

        private Button m_ApplyButton;
        private Button m_RevertButton;

        private TabGroup m_TabGroup;

        public MultiplayerRolesSettingsSection()
        {
            var enableContentSelectionBox = new Toggle("Enable Multiplayer Roles")
            {
                value = EditorMultiplayerRolesManager.EnableMultiplayerRoles,
                tooltip = "Enable Multiplayer Roles to strip components from builds based on the multiplayer role."
            };
            enableContentSelectionBox.RegisterValueChangedCallback((evt) => EditorMultiplayerRolesManager.EnableMultiplayerRoles = evt.newValue);
            Add(enableContentSelectionBox);

            m_ContentSelectionContainer = new VisualElement();
            Add(m_ContentSelectionContainer);

            var enableSafetyChecks = new Toggle("Enable safety checks")
            {
                value = EditorMultiplayerRolesManager.EnableSafetyChecks,
                tooltip = "Enable safety checks to prevent null reference errors when stripping components. Disabling this will improve building and entering play mode performance."
            };
            enableSafetyChecks.RegisterValueChangedCallback((evt) => EditorMultiplayerRolesManager.EnableSafetyChecks = evt.newValue);
            m_ContentSelectionContainer.Add(enableSafetyChecks);

            m_AutomaticSelectionOptions = ContentSelectionSettings.AutomaticSelection.Clone();

            var helpBox = new HelpBox("Select the components that will be stripped from Server or Client builds.", HelpBoxMessageType.Info);

            m_TabGroup = new TabGroup();
            m_TabGroup.AddTab("Server", CreateServerContent);
            m_TabGroup.AddTab("Clients", CreateClientContent);

            m_ContentSelectionContainer.Add(helpBox);
            m_ContentSelectionContainer.Add(m_TabGroup);

            var buttonsContainer = new VisualElement();
            buttonsContainer.style.flexDirection = FlexDirection.Row;
            buttonsContainer.style.alignSelf = Align.FlexEnd;
            buttonsContainer.style.flexShrink = 0;

            m_ApplyButton = new Button(Apply) { text = "Apply" };
            m_ApplyButton.style.marginLeft = 0;

            m_RevertButton = new Button(Revert) { text = "Revert" };

            buttonsContainer.Add(m_RevertButton);
            buttonsContainer.Add(m_ApplyButton);

            m_ContentSelectionContainer.Add(buttonsContainer);

            RegisterCallback<DetachFromPanelEvent>(OnDeactivate);
            RegisterCallback<ChangeEvent<bool>>((v) => OnChange());
            OnChange();

            RegisterCallback<AttachToPanelEvent>(_ => EditorApplication.playModeStateChanged += OnPlayModeStateChanged);
            RegisterCallback<DetachFromPanelEvent>(_ => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged);

            SetEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            SetEnabled(state != PlayModeStateChange.EnteredPlayMode);
        }

        private void OnDeactivate(DetachFromPanelEvent evt)
        {
            if (HasPendingChanges())
            {
                if (EditorUtility.DisplayDialog("Content Selection Settings Have Been Modified", "Do you want to apply changes?", "Apply", "Revert"))
                    Apply();
                else
                    Revert();
            }
        }

        private bool HasPendingChanges()
        {
            var changed = false;
            var currentOptions = ContentSelectionSettings.AutomaticSelection;

            if (m_AutomaticSelectionOptions.StripAudioComponents != currentOptions.StripAudioComponents ||
                m_AutomaticSelectionOptions.StripRenderingComponents != currentOptions.StripRenderingComponents ||
                m_AutomaticSelectionOptions.StripUIComponents != currentOptions.StripUIComponents)
            {
                changed = true;
            }
            else if (!m_AutomaticSelectionOptions.GetCustomComponents().SequenceEqual(currentOptions.GetCustomComponents()))
            {
                changed = true;
            }

            return changed;
        }

        private void OnChange()
        {
            m_ContentSelectionContainer.style.display = EditorMultiplayerRolesManager.EnableMultiplayerRoles
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            var changed = HasPendingChanges();

            m_ApplyButton.SetEnabled(changed);
            m_RevertButton.SetEnabled(changed);

            RefreshWindows();
        }

        private void RefreshWindows()
        {
            var windows = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            for (int i = 0; i < windows.Length; i++)
            {
                var window = windows[i] as EditorWindow;
                if (window != null)
                    window.Repaint();
            }
        }

        private void Apply()
        {
            ContentSelectionSettings.AutomaticSelection = m_AutomaticSelectionOptions.Clone();
            EditorUtility.SetDirty(ContentSelectionSettings.instance);
            ContentSelectionSettings.instance.SaveIfDirty();
            AssetDatabase.Refresh();
            OnChange();
        }

        private void Revert()
        {
            m_AutomaticSelectionOptions = ContentSelectionSettings.AutomaticSelection.Clone();
            m_TabGroup.Refresh();
            OnChange();
        }

        private VisualElement CreateServerContent()
        {
            var content = new VisualElement();

            var serializedObject = new SerializedObject(ContentSelectionSettings.instance);

            var stripRendering = new Toggle("Strip Rendering Components") { value = m_AutomaticSelectionOptions.StripRenderingComponents };
            stripRendering.RegisterValueChangedCallback((evt) => m_AutomaticSelectionOptions.StripRenderingComponents = evt.newValue);

            var stripUI = new Toggle("Strip UI Components") { value = m_AutomaticSelectionOptions.StripUIComponents };
            stripUI.RegisterValueChangedCallback((evt) => m_AutomaticSelectionOptions.StripUIComponents = evt.newValue);

            var stripAudio = new Toggle("Strip Audio Components") { value = m_AutomaticSelectionOptions.StripAudioComponents };
            stripAudio.RegisterValueChangedCallback((evt) => m_AutomaticSelectionOptions.StripAudioComponents = evt.newValue);

            var componentsList = m_AutomaticSelectionOptions.GetCustomComponents()
                .Where(kvp => (kvp.Value & MultiplayerRoleToFlags(MultiplayerRole.Server)) == 0)
                .Select(kvp => kvp.Key)
                .ToList();
            var listView = new ListView(componentsList)
            {
                headerTitle = "Strip Custom Components",
                showFoldoutHeader = true,
                selectionType = SelectionType.Multiple,
                // reorderMode = ListViewReorderMode.Animated,
                // reorderable = true,
                showBorder = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                showAddRemoveFooter = true,
                showBoundCollectionSize = false
            };
            listView.itemsRemoved += (i) => OnRemoveClicked(componentsList, MultiplayerRole.Server, i);
            SetupListAddButton(listView, MultiplayerRole.Server);

            content.Add(stripRendering);
            content.Add(stripUI);
            content.Add(stripAudio);
            content.Add(listView);

            return content;
        }

        private VisualElement CreateClientContent()
        {
            var content = new VisualElement();

            var componentsList = m_AutomaticSelectionOptions.GetCustomComponents()
                .Where(kvp => (kvp.Value & MultiplayerRoleToFlags(MultiplayerRole.Client)) == 0)
                .Select(kvp => kvp.Key)
                .ToList();
            var listView = new ListView(componentsList);
            listView.headerTitle = "Strip Custom Components";
            listView.showFoldoutHeader = true;
            listView.selectionType = SelectionType.Single;
            // listView.reorderMode = ListViewReorderMode.Animated;
            // listView.reorderable = true;
            listView.showBorder = true;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            listView.showAddRemoveFooter = true;
            listView.showBoundCollectionSize = false;
            listView.itemsRemoved += (i) => OnRemoveClicked(componentsList, MultiplayerRole.Client, i);
            SetupListAddButton(listView, MultiplayerRole.Client);

            content.Add(listView);

            return content;
        }

        private void SetupListAddButton(ListView list, MultiplayerRole target)
        {
            var oldButton = list.Q<Button>("unity-list-view__add-button");
            var parent = oldButton.parent;
            var addButton = new Button(() => OnAddClicked(list, target)) { name = "unity-list-view__add-button", text = "+" };
            parent.Add(addButton);
            addButton.PlaceBehind(oldButton);
            oldButton.RemoveFromHierarchy();
        }

        private void OnAddClicked(ListView list, MultiplayerRole target)
        {
            var position = Event.current.mousePosition;
            UnityEditor.PopupWindow.Show(new Rect(position.x, position.y, 0, 0), new NewComponentSelectionPopup(type =>
            {
                if (list.itemsSource.Contains(type))
                    return;

                var currentTarget = m_AutomaticSelectionOptions.GetMultiplayerRoleFlagsForType(type);
                m_AutomaticSelectionOptions.SetCustomComponentMultiplayerRoleFlags(type, currentTarget & ~MultiplayerRoleToFlags(target));

                list.itemsSource.Add(type);
                list.RefreshItems();
                OnChange();
            }));
        }

        private void OnRemoveClicked(List<Type> types, MultiplayerRole target, IEnumerable<int> items)
        {
            foreach (var item in items)
            {
                var type = types[item];
                var currentTarget = m_AutomaticSelectionOptions.GetMultiplayerRoleFlagsForType(type);
                m_AutomaticSelectionOptions.SetCustomComponentMultiplayerRoleFlags(type, currentTarget | MultiplayerRoleToFlags(target));
            }

            OnChange();
        }

        private static MultiplayerRoleFlags MultiplayerRoleToFlags(MultiplayerRole role)
            => (MultiplayerRoleFlags)(1 << (int)role);

        private class NewComponentSelectionPopup : PopupWindowContent
        {
            public delegate void AddDelegate(Type type);
            private AddDelegate m_AddCallback;
            private SearchField m_SearchField;
            private string m_SearchText;
            private IEnumerable<Type> m_Types;
            private IEnumerable<Type> m_FilteredTypes;
            private Vector2 m_ScrollPosition;

            public NewComponentSelectionPopup(AddDelegate addCallback)
            {
                m_AddCallback = addCallback;
                m_SearchField = new SearchField();
                m_Types = TypeCache.GetTypesDerivedFrom<Component>()
                    .Where(t => t.IsPublic && t.GetCustomAttribute<MultiplayerRoleRestrictedAttribute>() == null)
                    .Except(new[] { typeof(Transform), typeof(RectTransform) });
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(250, 300);
            }

            public override void OnGUI(Rect rect)
            {
                GUILayout.Space(5);

                var searchText = m_SearchField.OnGUI(EditorGUILayout.GetControlRect(), m_SearchText);
                if (searchText != m_SearchText)
                {
                    m_SearchText = searchText;
                    if (string.IsNullOrWhiteSpace(searchText))
                        m_FilteredTypes = null;
                    else
                        m_FilteredTypes = m_Types
                            .Where(t => t.FullName.Contains(m_SearchText, StringComparison.CurrentCultureIgnoreCase))
                            .OrderBy(t => t.Name);
                }

                m_SearchField.SetFocus();

                if (m_FilteredTypes != null)
                {
                    m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                    foreach (var type in m_FilteredTypes)
                    {
                        var style = GUI.skin.button;
                        style.alignment = TextAnchor.MiddleLeft;
                        var guiContent = new GUIContent(type.Name, type.FullName);
                        if (GUILayout.Button(guiContent, style))
                        {
                            // editorWindow.Close();
                            m_AddCallback.Invoke(type);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }
    }
}
