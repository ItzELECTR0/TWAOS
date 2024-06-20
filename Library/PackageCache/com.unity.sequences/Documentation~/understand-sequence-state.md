# Understand Sequence's state

### Instantiating a Sequence in the active Scene

If a Sequence structure (a Master Sequence and its Sequences) in the Sequences window is gray because the Scene that contains it was removed or unloaded, you can create an instance of the Sequence structure in the active Scene. You can use this feature to make orphaned Sequences editable and to move Sequences to a different Scene before you add Sequence Assets or other creative content.

>**Note:**
>* The instantiated Sequence structure contains all the Sequences and Timelines in the original Scene, but the structure does not contain any creative content.
>* The instantiated Sequence is independent of other instances of the same Sequence in other Scenes. Changes to one instance of a Sequence have no effect on other instances.

To create an instance of a grayed-out Master Sequence and its Sequences in the active Scene:

In the Sequences window, right-click any Sequence in the grayed-out sequence structure and select **Instantiate in active Scene**. <br />
The Master Sequence and all its child Sequences appear under the active Scene in the Hierarchy window. The Sequences are editable in the Sequences window.
