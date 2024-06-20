# Create and manage Sequence Assets

## Creating a Sequence Asset

To create a new Sequence Asset:

1.  Right-click on an [Asset Collection type](concepts.md#asset-collections) and select **Create Sequence Asset**.
    <br />- OR -
    <br />Select the **+** (plus) button at the top left of the Sequence Assets window, then select the Sequence Asset type you want to create.

2.  Rename the new Sequence Asset as you wish and press the Enter key or click anywhere to complete the creation.

## Creating a Variant from a Sequence Asset

To create a new Variant from an existing Sequence Asset:

1.  Right-click on the Sequence Asset and select **Create Variant**.

2.  Rename the new Sequence Asset Variant as you wish and press the Enter key or click anywhere to complete the creation.

>**Note:** By default, the Sequence Asset Variant inherits from the name of the Sequence Asset, suffixed with `_Variant`, and an incremental ID number if the name already exists.

## Duplicating a Sequence Asset Variant

To duplicate a Sequence Asset Variant:

* Right-click on the Sequence Asset Variant and select **Duplicate**.
<br />- OR -
* Select the Sequence Asset Variant and press Ctrl+D / Cmd+D.

This action automatically names the duplicate Variant after the name of the source Variant, suffixed with an incremental ID number.

>**Note:** After the duplication, the two Variants have the exact same content but are independent Variants of the same Sequence Asset.

## Editing a Sequence Asset or Variant

To edit a Sequence Asset or Sequence Asset Variant, right-click on it and select **Open**. 

This opens the corresponding Prefab or Prefab Variant in isolation in the Scene view so that you can edit its content.

>**Note:** You can also open your Sequence Assets and Variants directly from the Project window, or from the Hierarchy. In all cases, see the Unity Manual for more details on how to [edit Prefabs](https://docs.unity3d.com/2020.1/Documentation/Manual/EditingInPrefabMode.html) and manage overrides in [Prefab Variants](https://docs.unity3d.com/Manual/PrefabVariants.html).

## Renaming a Sequence Asset or Variant

To rename a Sequence Asset or Sequence Asset Variant:
1. Right-click it and select **Rename**.
2. Type the new name.
3. Press the Enter key or click anywhere to confirm the renaming.

When you rename a Sequence Asset, this also renames its Variants if they were still named after it.

>**Note:** You can fully rename Variants independently from the original Sequence Asset without affecting their functional relationship.

## Deleting a Sequence Asset or Variant

To delete a Sequence Asset or a Sequence Asset Variant, select it and press the Delete key or right-click on it and select **Delete**.

>**Attention:**
>* When you delete a Sequence Asset, this also deletes all its Sequence Asset Variants if it has any.
>* You cannot undo the deletion of Sequence Assets and Sequence Asset Variants.
