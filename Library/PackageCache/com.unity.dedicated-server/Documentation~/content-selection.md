# Control which GameObjects and components exist on the client or the server

In your multiplayer project, you can use [multiplayer roles](LINK) to define which Game Objects and Components exist on the client, the server, or both. For example, you can use this functionality to do the following:

- Assign GameObjects that use a high amount of memory to the server to save memory on the client.
- Assign GameObjects that communicate with other services on a server to that server.

Unity removes (strips) GameObjects and components that aren’t assigned to the multiplayer role the device uses when you build a project or enter play mode.

You can do this in the following ways:

- [Assign a GameObject to a multiplayer role](#gameobject-mp-role).
- [Assign a component to a multiplayer role](#component-mp-role).
- [Assign multiple GameObjects and components a multiplayer role](#multiple-mp-role).
- [Assign Prefab assets and instances to a multiplayer role](#prefab-mp-role).

<a name="gameobject-mp-role"></a>

## Assign a GameObject to a multiplayer role

To select which GameObjects exist in the client or server multiplayer roles:

1. Open a scene.
2. In the hierarchy window, select one or more GameObjects.
3. In the inspector window, select the [multiplayer role icon](mutliplayer-roles-icons.md).
4. Select the multiplayer role(s) you want this GameObject to exist in: **Client** or **Server**.

<a name="component-mp-role"></a>

## Assign a component to a multiplayer role

To select which components exist in the client or server multiplayer roles:

1. Open a scene.
2. In the Hierarchy window, select one or more Game Objects.
3. In the Inspector window, select the [multiplayer role icon](#mp-role-icons) for the component you want to assign.
4. Select the multiplayer role(s) you want this GameObject to exist in: **Client** or **Server**.

<a name="multiple-mp-role"></a>

## Assign multiple GameObjects and Components to the server or client

You can also use the search window to control which multiplayer role multiple GameObjects or components exist in:

1. Open a scene.
2. Open the[ Search window](https://docs.unity3d.com/Manual/search-overview.html) (menu: **Edit** > **Search All**)
3. Search for the objects to select. To learn about Unity search terms, refer to [Search expressions](https://docs.unity3d.com/Manual/search-expressions.html).
4. Select the GameObjects you want to assign to a multiplayer role or that contain a component that you want to assign to a multiplayer role.
5. In the Preview Inspector panel, select the [multiplayer role icon](mutliplayer-roles-icons.md) for the selected GameObject or component.
6. Select the multiplayer role(s) you want the GameObjects or components to exist in: **Client** or **Server**. This assigns the same role to every GameObject or component you select in the Search window.

<a name="prefab-mp-role"></a>

## Assign prefabs to a multiplayer role

You can't assign a root GameObject of a prefab asset to a multiplayer role. To assign the contents of a prefab to a multiplayer role, use one of the following methods:

- Assign children of a prefab asset to a multiplayer role.
- Assign an instance of a prefab to a multiplayer role.

### Assign children of a prefab asset to a multiplayer role

To assign all GameObjects in a prefab asset to a multiplayer role, do the following:

1. Select a child GameObject of a prefab asset in the Hierarchy window.
2. In the inspector window, select the [multiplayer role icon](mutliplayer-roles-icons.md).
3. Select the multiplayer role(s) you want the contents of this prefab asset, and its instances, to exist in: **Client** or **Server**.

The multiplayer role you select for the prefab asset applies to every instance of this prefab you create.

### Assign a prefab instance to a multiplayer role

To assign a prefab instance, which is the actual GameObject in the scene, to a multiplayer role, do the following:

1. Select a prefab instance in the Hierarchy Window and click **Open**.
2. In the inspector window, select the [multiplayer role icon](mutliplayer-roles-icons.md).
3. Select the multiplayer role(s) you want this prefab instance to exist in: **Client** or **Server**.

## Stop Unity from stripping a MonoBehavior from multiplayer roles

To stop Unity from stripping a MonoBehavior from the server or client multiplayer roles, add the `MultiplayerRoleRestricted` attribute to a custom MonoBehavior.
This marks the component as restricted which means that Unity can't strip it from the server or client multiplayer role.
