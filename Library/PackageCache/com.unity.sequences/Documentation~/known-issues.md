# Known issues and limitations

This page lists some known issues and limitations that you might experience with the Sequences package. It also gives basic instructions to help you work around them when possible.

#### All sequences suffixed with _\_Timeline_ in the Sequences window

**Issue:** Upgrading the Sequences package from version 1.x has the side effect of adding a _\_Timeline_ suffix to the display name of all your existing sequences in the Sequences window.

**Note:** This behavior doesn't affect the Sequences functionality in your project. If needed, you can safely rename each sequence as you wish.

#### Can’t rename a Sequence whose GameObject is a Prefab

**Limitation:** You can’t currently rename a Prefab-converted Sequence from the Sequences window.

**Workaround:** Depending on the scope of your need, use the Hierarchy window to rename the instantiated Prefab, or the Project view to rename the Prefab Asset, or both.

**Note:** if you rename the instantiated Prefab, Unity automatically renames the Sequence in the Sequences window.

#### Game view not always updated on Sequence Asset Variant swap

**Issue:** When you swap Variants of a Sequence Asset that is currently framed in the Game view, you might not always be able to see the expected visual result of the swap.

**Workaround:** To see the actual result of the Variant swap, you need to slightly scrub the playhead in the Timeline window.
