# Changelog

## [1.1.0] - 2024-04-24

### Fixed

- Fixed the content selection icon overlapping with the Cinemachine for the camera in the hierarchy view.
- Fixed multiplayer roles settings not being synced to multiplayer playmode virtual players.

### Changed

- Multiplayer Roles can be now assigned to build profiles. The old `EditorMultiplayerRolesManager.[Get|Set]MultiplayerRoleForBuildTarget` API has been deprecated in favor of the new `EditorMultiplayerRolesManager.[Get|Set]MultiplayerRoleForBuildProfile` API.

## [1.0.0] - 2024-03-12

### Added

- CLI Arguments Defaults: Provides an UI in the build window for defining default values for the CLI arguments used to launch the game server.
- Multiplayer Roles: Allows to decide which multiplayer role (Client, Server) to use in each build target.
- Content Selection: Provides UI and API for selecting which content (GameObjects, Components) should be present/removed in the different multiplayer roles.
- Automatic Selection: Provides UI and API for selecting which component types should be automatically removed in the different multiplayer roles.
- Safety Checks: Activates warnings that helps detecting potential null reference exceptions caused by stripping objects for a multiplayer role.
