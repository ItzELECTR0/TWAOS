using UnityEditor;

namespace GameCreator.Editor.Common
{
    public interface IPolymorphicListTool
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        SerializedObject SerializedObject { get; }
        SerializedProperty PropertyList { get; }

        ManipulatorPolymorphicListSort ManipulatorSort { get; }
        
        bool AllowReordering    { get; }
        bool AllowDuplicating   { get; }
        bool AllowDeleting      { get; }
        bool AllowContextMenu   { get; }
        bool AllowCopyPaste     { get; }
        bool AllowInsertion     { get; }
        bool AllowBreakpoint    { get; }
        bool AllowDisable       { get; }
        bool AllowDocumentation { get; }
        bool AllowGroupCollapse { get; }
        bool AllowGroupExpand   { get; }

        // METHODS: -------------------------------------------------------------------------------
        
        void Refresh();

        void InsertItem(int index, object value);
        void DeleteItem(int index);
        void DuplicateItem(int index);

        void Expand();
        void Collapse();
        
        void MoveItems(int sourceIndex, int destinationIndex);
    }
}
