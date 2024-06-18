#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    public class LocalizationTableWindow : EditorWindow
    {
        static LocalizationTableWindow window;
        static LocalizationSettings localizationSettings;
        static LocalizationTable selectedTable;
        static int tableIndex;

        private GUISkin customSkin;
        protected GUIStyle panelStyle;
        static GUIStyle langLabelStyle;
        static float textFieldWidth = 180;
        static float textFieldHeight = 80;
        static float itemSpacing = 3;
        private string searchString;
        Vector2 scrollPosition = Vector2.zero;

        // Caching a table in case of compilation
        public LocalizationTable cachedTable;

        public static void ShowWindow(LocalizationSettings settings, LocalizationTable table, int index)
        {
            window = GetWindow<LocalizationTableWindow>();
            window.minSize = new Vector2(600, 400);

            // Replace variables
            localizationSettings = null;
            localizationSettings = settings;
            selectedTable = null;
            selectedTable = table;
            tableIndex = index;

            // Set title
            var icon = Resources.Load<Texture>("LocalizationWindowIcon");
            GUIContent newTitle = new GUIContent("Localization Table (" + table.tableID + ")", icon);
            window.titleContent = newTitle;
        }

        void OnEnable()
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        void OnGUI()
        {
            // Initialize table and content
            if (cachedTable == null && selectedTable != null) { cachedTable = selectedTable; }
            if (langLabelStyle == null) { langLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter }; }
            if (EditorStyles.textArea.wordWrap == false) { EditorStyles.textArea.wordWrap = true; }
            if (localizationSettings == null || selectedTable == null)
            {
                if (cachedTable != null) { selectedTable = cachedTable; localizationSettings = selectedTable.localizationSettings; }
                else
                {
                    EditorGUILayout.HelpBox("There's no selected table. You can reopen the table via Localization Settings or the table file.", MessageType.Info);
                    return;
                }
            }

            // Top Bar
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);

            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSearchTextField"), GUILayout.Width(textFieldWidth + 24));
            if (!string.IsNullOrEmpty(searchString) && GUILayout.Button(new GUIContent("", "Clear search bar"), GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                searchString = "";
                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();
            GUI.enabled = false;
            EditorGUILayout.LabelField("Table", GUILayout.Width(40));
            selectedTable = EditorGUILayout.ObjectField(selectedTable, typeof(LocalizationTable), true, GUILayout.Width(200)) as LocalizationTable;
            GUI.enabled = true;
            GUILayout.Space(8);
            GUILayout.EndHorizontal();

            // Horizontal line to separate stuff
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Height(14));
            GUILayout.Space(8);
            GUILayout.EndHorizontal();

            // Top indicators
            GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUIStyle.none);
            GUILayout.BeginHorizontal();
            GUILayout.Space(27);

            EditorGUILayout.LabelField(new GUIContent("String Key"), langLabelStyle, GUILayout.Width(textFieldWidth));
            EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(0));

            for (int i = 0; i < localizationSettings.languages.Count; i++)
            {
                if (localizationSettings.languages[i].localizationLanguage == null)
                    continue;

                EditorGUILayout.LabelField(new GUIContent(localizationSettings.languages[i].languageName + " (" + localizationSettings.languages[i].languageID + ")")
                     , langLabelStyle, GUILayout.Width(textFieldWidth));
                if (i != localizationSettings.languages.Count - 1) { EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(0)); }
            }

            GUILayout.Space(27);
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            // Custom panel
            panelStyle = new GUIStyle(GUI.skin.box);

            // Scroll panel
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            GUILayout.BeginVertical(panelStyle);

            for (int i = 0; i < selectedTable.tableContent.Count; i++)
            {
                // If search field is blank
                if (string.IsNullOrEmpty(searchString))
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("-", "Remove string key"), customSkin.button, GUILayout.Width(20), GUILayout.Height(textFieldHeight)))
                    {
                        Undo.RecordObject(this, "Removed localization string key");

                        for (int x = 0; x < localizationSettings.languages.Count; x++)
                        {
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent.RemoveAt(i);
                            EditorUtility.SetDirty(localizationSettings.languages[x].localizationLanguage);
                        }

                        selectedTable.tableContent.RemoveAt(i);
                        EditorUtility.SetDirty(selectedTable);
                        GUILayout.EndScrollView();
                        continue;
                    }

                    selectedTable.tableContent[i].key = EditorGUILayout.TextArea(selectedTable.tableContent[i].key, GUILayout.Width(textFieldWidth), GUILayout.Height(textFieldHeight));
                    EditorUtility.SetDirty(selectedTable);
                    GUILayout.Space(itemSpacing);

                    for (int x = 0; x < localizationSettings.languages.Count; x++)
                    {
                        if (localizationSettings.languages[x].localizationLanguage == null || localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].table != selectedTable)
                            continue;

                        if (localizationSettings.enableExperimental)
                        {
                            GUILayout.BeginVertical(GUILayout.Width(textFieldWidth));

                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].value = EditorGUILayout.TextArea(localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].value, EditorStyles.textArea, GUILayout.Width(textFieldWidth), GUILayout.Height(textFieldHeight - 20));
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].key = selectedTable.tableContent[i].key;

                            GUILayout.BeginHorizontal();
                            // GUILayout.Label("Audio Clip", GUILayout.Width(70));
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].audioValue = EditorGUILayout.ObjectField(localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].audioValue, typeof(AudioClip), true) as AudioClip;
                            // GUILayout.Label("Sprite", GUILayout.Width(70));
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].spriteValue = EditorGUILayout.ObjectField(localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].spriteValue, typeof(Sprite), true) as Sprite;
                            GUILayout.EndHorizontal();

                            GUILayout.EndVertical();
                            GUILayout.Space(itemSpacing);
                        }

                        else
                        {
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].value = EditorGUILayout.TextArea(localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].value, EditorStyles.textArea, GUILayout.Width(textFieldWidth), GUILayout.Height(textFieldHeight));
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].key = selectedTable.tableContent[i].key;
                            GUILayout.Space(itemSpacing);
                        }

                        EditorUtility.SetDirty(localizationSettings.languages[x].localizationLanguage);
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.Space(itemSpacing);
                }

                // If using search field
                else if (localizationSettings.languages[0].localizationLanguage.tableList[tableIndex].tableContent[i].key.ToLower().Contains(searchString.ToLower()))
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("-", "Remove string key"), customSkin.button, GUILayout.Width(20), GUILayout.Height(textFieldHeight)))
                    {
                        Undo.RecordObject(this, "Removed localization string key");

                        for (int x = 0; x < localizationSettings.languages.Count; x++)
                        {
                            localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent.RemoveAt(i);
                            EditorUtility.SetDirty(localizationSettings.languages[x].localizationLanguage);
                        }

                        selectedTable.tableContent.RemoveAt(i);
                        EditorUtility.SetDirty(selectedTable);
                        GUILayout.EndScrollView();
                        continue;
                    }

                    // To do later: Add key edit support while searching
                    GUI.enabled = false;
                    EditorGUILayout.TextArea(selectedTable.tableContent[i].key, GUILayout.Width(textFieldWidth), GUILayout.Height(textFieldHeight));
                    GUI.enabled = true;
                    GUILayout.Space(itemSpacing);

                    for (int x = 0; x < localizationSettings.languages.Count; x++)
                    {
                        if (localizationSettings.languages[x].localizationLanguage == null || localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].table != selectedTable)
                            continue;

                        localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].value = EditorGUILayout.TextArea(localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].value, EditorStyles.textArea, GUILayout.Width(textFieldWidth), GUILayout.Height(textFieldHeight));
                        GUILayout.Space(itemSpacing);
                        localizationSettings.languages[x].localizationLanguage.tableList[tableIndex].tableContent[i].key = selectedTable.tableContent[i].key;
                        EditorUtility.SetDirty(localizationSettings.languages[x].localizationLanguage);
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.Space(itemSpacing);
                }
            }

            // Scroll Panel End
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();

            // New entry
            if (GUILayout.Button("+ Add New Table Entry", customSkin.button))
            {
                Undo.RecordObject(this, "Added localization string key");

                LocalizationTable.TableContent tempContent = new LocalizationTable.TableContent();
                tempContent.key = "Key " + selectedTable.tableContent.Count.ToString();
                selectedTable.tableContent.Add(tempContent);
                EditorUtility.SetDirty(selectedTable);

                for (int i = 0; i < localizationSettings.languages.Count; i++)
                {
                    if (localizationSettings.languages[i].localizationLanguage == null)
                        continue;

                    LocalizationLanguage.TableContent ltc = new LocalizationLanguage.TableContent();
                    ltc.key = tempContent.key;
                    localizationSettings.languages[i].localizationLanguage.tableList[tableIndex].table = selectedTable;
                    localizationSettings.languages[i].localizationLanguage.tableList[tableIndex].tableContent.Add(ltc);
                    EditorUtility.SetDirty(localizationSettings.languages[i].localizationLanguage);
                }

                scrollPosition = new Vector2(scrollPosition.x, selectedTable.tableContent.Count * (textFieldHeight + itemSpacing));
            }

            if (GUILayout.Button("Open Settings", customSkin.button, GUILayout.Width(100)))
            {
                Selection.activeObject = localizationSettings;
            }

            GUILayout.EndHorizontal();
        }
    }
}
#endif