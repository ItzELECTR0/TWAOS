using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    abstract class BaseEditorWindow : EditorWindow
    {
        static readonly string k_UXMLFilePath = "Packages/com.unity.sequences/Editor/Core/UI/UIData/WindowHeader.uxml";

        private class Styles
        {
            public static readonly string k_HelpButton = "seq-help-button";
            public static readonly string k_ContentContainer = "seq-header-content";
        }

        // Load the UXML file and apply common style sheets.
        protected void LoadUIData()
        {
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UXMLFilePath);
            visualTree.CloneTree(root);

            // Set style
            StyleSheetUtility.SetStyleSheets(root);
        }

        /// <summary>
        /// Setup the UI with default values and callbacks. This function must always be called after loading any UI
        /// data (UXML files or style sheets), see the LoadUIData function bellow.
        ///
        /// Override this function in inherited class to complete the setup of the view as necessary.
        /// </summary>
        protected virtual void SetupView()
        {
            minSize = new Vector2(200.0f, 250.0f);

            // Help button
            var helpButton = rootVisualElement.Q<Button>(Styles.k_HelpButton);
            helpButton.tooltip = $"Open Reference for {GetType().Name}.";
            helpButton.clicked += OnHelpClicked;
        }

        void OnEnable()
        {
            try
            {
                LoadUIData();
            }
            catch (ArgumentNullException)
            {
                // UI Data (UXML or Stylesheet) couldn't load, try to refresh the UI is the next Editor update frame(s).
                rootVisualElement.Clear();
                EditorApplication.update += TryRefresh;
                return;
            }

            SetupView();
        }

        void TryRefresh()
        {
            try
            {
                LoadUIData();
            }
            catch (ArgumentNullException)
            {
                // UI Data (UXML or Stylesheet) couldn't load, keep trying at the next Editor frame.
                rootVisualElement.Clear();
                return;
            }

            SetupView();
            EditorApplication.update -= TryRefresh;
        }

        /// <summary>
        /// Set the content of the window header. Will be placed to the left of the help button
        /// </summary>
        /// <param name="content"></param>
        protected void SetHeaderContent(VisualElement content)
        {
            var leftContent = rootVisualElement.Q<VisualElement>(Styles.k_ContentContainer);
            leftContent.Add(content);
        }

        void OnHelpClicked()
        {
            Help.ShowHelpForObject(this);
        }
    }
}
