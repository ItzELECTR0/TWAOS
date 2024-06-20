# Create and manage an editorial structure

## Creating a Sequence

To create a Master Sequence, select the **+** (plus) button at the top left of the Sequences window, then select **Create Master Sequence**.
>**Note:** Regardless of whether a Sequence is selected in the window, the new Master Sequence will appear in alphabetical order at the same level as other Master Sequences in the Sequences window.

To create a Sequence within another Sequence:
1. Select the Sequence (or Master Sequence) to contain the one you want to create.
2. Select the **+** (plus) button at the top left of the Sequences window and select **Create Sequence**.

  >**Note:** If the parent Sequence is a Prefab, you might need to apply overrides from the Prefab instance to the Prefab asset to keep them synchronized. See [how to manage Sequences as Prefabs](sequences-as-prefabs.md).

When the new Sequence appears in the structure, you can directly rename it and press the Enter key or click anywhere to complete the creation.

Once you have created a new Sequence, you can use the [Sequence Assembly window](sequence-assembly-window.md) to populate it with Sequence Assets or Variants from your [Asset Collections](sequence-assets-window.md).

## Renaming a Sequence

To rename a Sequence:
1. Right-click the sequence and select **Rename**.
2. Type the new name.
3. Press the Enter key or click anywhere to confirm the renaming.

>**Note:**
>* The Sequences window currently prevents you from renaming a Sequence that is a Prefab (see the [suggested workaround](known-issues.md)).
>* If you rename a Sequence within a Sequence that is a Prefab, you might need to apply overrides from the Prefab instance to the Prefab asset to keep them synchronized. See [how to manage Sequences as Prefabs](sequences-as-prefabs.md).

## Deleting Sequences

To delete a Sequence from your project, select it and press the Delete key or right-click on it and select **Delete**. This also automatically deletes from your project all Sequences the deleted Sequence may contain.

You can also select multiple Sequences and delete them as a batch.

>**Note:** If you want to delete a Sequence within a Sequence that is a Prefab, you might need to enter the Prefab Mode first. See [how to manage Sequences as Prefabs](sequences-as-prefabs.md).
