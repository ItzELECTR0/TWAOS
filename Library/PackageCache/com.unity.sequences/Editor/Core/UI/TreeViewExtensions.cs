using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Adds extra functionality to <see cref="TreeView"/>.
    /// </summary>
    static class TreeViewExtensions
    {
        /// <summary>
        /// Expands all the parents of the given item.
        /// </summary>
        /// <param name="treeView">Tree view.</param>
        /// <param name="id">The TreeView item identifier.</param>
        public static void ExpandItemParents(this TreeView treeView, int id)
        {
            for (var parentId = treeView.viewController.GetParentId(id);
                 parentId != -1;
                 parentId = treeView.viewController.GetParentId(parentId))
            {
                treeView.ExpandItem(parentId);
            }
        }
    }
}
