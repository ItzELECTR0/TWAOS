# About the Dedicated Server package

Use the Dedicated Server package when you use the [Dedicated Server build target](https://docs.unity3d.com/Manual/dedicated-server-introduction.html) to switch a project between the server and client role without the need to create another project. To do this, use Multiplayer roles to distribute GameObjects and components accross the client and server.

This package contains optimizations and workflow improvements for developing Dedicated Server platform. For example, you can use Dedicated Server package to mark all render components of a scene so they're present only on the Standalone builds and removed in the Dedicated Server ones.

## Installation

To install this package, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Manual/upm-ui-quick.html).

## Requirements

This version of Dedicated Server has the following requirements:

* Unity Editor 6 and later.
* Dedicated Server Build Support module installed.

## Compatibility

The Dedicated Server package is compatible with the following multiplayer packages:

* [Multiplayer play mode](https://docs-multiplayer.unity3d.com/mppm/current/dedicated-server/multiplayer-role/): Use this package with the Dedicated Server package to determine whether each virtual player acts as a client or server.
* [Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/): Use this package with the Dedicated Server package to define which GameObjects and Components exist in the different multiplayer roles.
* [Netcode for Entities](https://docs.unity3d.com/Packages/com.unity.netcode@1.2/manual/index.html): When you use this package with the Dedicated Server package, the multiplayer roles of GameObjects and Components don't affect how Unity bakes entities and components in subscenes. To perform Client and Server stripping on entities, refer to [Ghost snapshots](https://docs.unity3d.com/Packages/com.unity.netcode@1.2/manual/ghost-snapshots.html).

## Features list

* [CLI Arguments Defaults](cli-arguments.md): Provides an UI in the build window for defining default values for the CLI arguments used to launch the game server.
* [Multiplayer Roles](multiplayer-roles.md): Allows to decide which multiplayer role (Client, Server) to use in each build target.
    * [Content Selection](content-selection.md): Provides UI and API for selecting which content (GameObjects, Components) should be present/removed in the different multiplayer roles.
    * [Automatic Selection](automatic-selection.md): Provides UI and API for selecting which component types should be automatically removed in the different multiplayer roles.
    * [Safety Checks](safety-checks.md): Activates warnings that helps detecting potential null reference exceptions caused by stripping objects for a multiplayer role.
