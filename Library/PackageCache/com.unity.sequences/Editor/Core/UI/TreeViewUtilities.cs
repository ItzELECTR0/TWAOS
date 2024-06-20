using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Miscellaneous helper functions for working with <see cref="TreeView"/>.
    /// </summary>
    static class TreeViewUtilities
    {
        /// <summary>
        /// Recursively traverses a tree, preorder depth first.
        /// The order is the same as the vertical order that TreeView displays it in.
        /// </summary>
        /// <param name="root">Tree root item.</param>
        /// <returns>Flattened enumeration of a tree's item data.</returns>
        internal static IEnumerable<TreeViewItemData<T>> TraverseItemData<T>(TreeViewItemData<T> root)
        {
            var stack = new Stack<TreeViewItemData<T>>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var item = stack.Pop();
                yield return new TreeViewItemData<T>(item.id, item.data);

                foreach (var child in item.children.Reverse())
                    stack.Push(child);
            }
        }
    }
}
