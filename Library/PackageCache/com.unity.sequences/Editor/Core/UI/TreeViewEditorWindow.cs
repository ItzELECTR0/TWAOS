using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// A base class for EditorWindow in Sequences that hold a TreeView (i.e. the Sequences window and the
    /// Sequence Assets window).
    /// </summary>
    /// <typeparam name="T">The type of the TreeView class to create.</typeparam>
    abstract class TreeViewEditorWindow<T> : BaseEditorWindow where T : SequencesTreeView, new()
    {
        internal class Styles
        {
            public static readonly string treeViewContainerClassName = "seq-tree-view-container";
            public static readonly string createButtonClassName = "seq-create-button";
            public static readonly string createNewIconClassName = "seq-create-add-new-icon";
        }

        internal T treeView { get; private set; }

        /// <summary>
        /// Setup the UI with default values and callbacks. This function must always be called after loading any UI
        /// data (UXML files or style sheets), see the LoadUIData function bellow.
        ///
        /// Override this function in inherited class to complete the setup of the view as necessary.
        /// </summary>
        protected override void SetupView()
        {
            base.SetupView();
            minSize = new Vector2(200.0f,  250.0f);

            SetHeaderContent(BuildAddMenu());

            // Tree View
            var container = new VisualElement();
            container.AddToClassList(Styles.treeViewContainerClassName);
            treeView = new T();
            container.Add(treeView);
            rootVisualElement.Add(container);
        }

        protected VisualElement BuildAddMenu()
        {
            var addMenu = new ToolbarMenu();
            addMenu.AddToClassList(Styles.createButtonClassName);
            addMenu.tooltip = GetAddMenuTooltip();
            PopulateAddMenu(addMenu.menu);

            var plusIcon = new VisualElement();
            plusIcon.AddToClassList(Styles.createNewIconClassName);

            addMenu.Insert(0, plusIcon);
            return addMenu;
        }

        protected void AddManipulator(ContextualMenuManipulator manipulator)
        {
            var container = rootVisualElement.Q<VisualElement>(className: Styles.treeViewContainerClassName);
            container.AddManipulator(manipulator);
        }

        /// <summary>
        /// Implement this function to return the tooltip string that should be displayed on the Add menu button at
        /// the top left of the window.
        /// </summary>
        /// <returns>The tooltip string to set on the Add menu button.</returns>
        protected abstract string GetAddMenuTooltip();

        /// <summary>
        /// Implement this function to populate the Add menu with action items.
        /// </summary>
        /// <param name="menu">The DropdownMenu to populate.</param>
        /// <param name="contextual">Whether the menu to populate is a contextual menu or not.</param>
        protected abstract void PopulateAddMenu(DropdownMenu menu, bool contextual = false);

        void OnFocus()
        {
            if (treeView == null)
                return;

            treeView.OnWindowFocused();
        }
    }
}
