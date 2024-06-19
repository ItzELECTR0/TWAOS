# Automatically remove components from a Multiplayer Role

You can select which types of components exist in project scenes and prefabs in the client or server multiplayer roles in the **Project Settings** window. When you build or enter play mode, Unity  automatically removes (strips) the corresponding component types and their subclasses.

For more information about which components each property strips, refer to [Multiplayer Role reference](multiplayer-roles-reference.md)


## Automatically strip custom components from a multiplayer mode

To select specific types of component to strip from the server or client, do the following:

1. In the Project Settings window, select **Multiplayer** > **Multiplayer Roles**
2. In the Multiplayer Roles window, select the **Server** or **Clients** tab.
3. In the **Strip Custom Components** section, select the plus icon (**+**) to add a component to the list.
