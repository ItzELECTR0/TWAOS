# Identify null references

A null reference exception can happen when Unity removes (strips) GameObjects or components for a multiplayer role when those objects are referenced by other objects. Unity checks for null references when you build a project or enter play mode.

To identify when a null reference exception happens, do the following:

1. Open  the Project Settings window (**Edit** > **Project Settings**).
2. Select  **Multiplayer** > **Multiplayer Roles**.
3. Select the **Safety Checks** checkbox.

For more information about the Multiplayer Roles properties, refer to [Multiplayer Role reference](multiplayer-roles-reference.md).
