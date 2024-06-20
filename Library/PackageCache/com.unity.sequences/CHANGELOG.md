# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.1.1] - 2023-11-07

### Fixed
* Prevent unexpected path error when switching the Editor from dark theme to light theme.

## [2.1.0] - 2023-05-01

### Changed
* Improved the documentation structure and removed obsolete information.

## [2.1.0-pre.1] - 2022-12-09

### Added
* New search field in the Sequences window.
* New MasterSequenceRegistry asset to store references to all Master Sequence timelines of the project and their associated Master Scenes.
* New Project Settings property allowing to change the default MasterSequenceRegistry asset (Unity creates one by default on first Master Sequence creation).
* New option to open the Master Scene associated to an editorial structure from the Sequences window.
* Ability to multi-select and bulk-delete Sequence Assets from the Sequence Assets window.
* Ability to clear an Asset Collection (i.e. delete all Sequence Assets it contains) in one single action.
* Use of an empty state message to the Sequence Assembly Window when no sequence is selected.

### Changed
* Unity now preserves the Sequence Assembly's view on domain reload.
* Systematically remove from the Sequences window any Master Sequence that has no corresponding timeline.
* Sequence Assets are now sorted alphabetically in menus and selection lists.
* Sequence Assets and their variants are now sorted alphabetically in the Sequence Assets window.
* Sequences and their children are now sorted alphabetically in the Sequences window.

### Fixed
* Prevent Unity from throwing an exception when going into Play Mode while the Sequence Assembly Window is opened.
* Make sure that the "Unload" button is always disabled in the Scene Activation Track Inspector when no valid Scene is specified.
* Make sure that the Sequence Assets window always displays specific icons for all Asset Collection types.
* When creating a new sequence, make sure that its Timeline clip doesn't overlap with existing sequence clips in the same editorial track.
* After creating a sub-sequence, make sure that the order in the Sequences window is maintained when saving.
* When deleting an invalid sequence, make sure to systematically delete its corresponding GameObject in the Hierarchy.
* Prevent the Sequence Assets window from being in error when the user deletes a Sequence Asset from the Project window.
* Prevent the Sequence Assembly window from clearing after deleting a Sequence Asset.
* Make sure that users can instantiate an editorial structure in multiple scenes without getting issues.

## [2.0.0-pre.2] - 2022-05-13

### Changed
* Systematically remove from the Sequences window any Master Sequence that has no corresponding asset.
* The "Load" button in the Scene Activation Track Inspector is now disabled until the user specifies a valid Scene.

### Fixed
* Adapt the Scene Management Sample to make it properly work according to Sequences 2.0.0-pre.1 feature changes.
* Ensure Sequence timeline opens after clicking an already-selected item in the Sequences window.
* Prevent entering rename mode when clicking on a Sequence in an unfocused Sequences window.
* Substitute invalid filename characters in Sequence and Sequence Asset names.
* Seamlessly ignore any renaming of a Sequence to a whitespace-only name.
* Ensure the Scene Asset picker of the Scene Activation Track only shows Scenes.
* Prevent Unity from throwing an exception when the user clicks on the "Load" button in a Scene Activation Track immediately after assigning a Scene.
* Prevent Unity from throwing an exception when adding a manually-created Sequence to a Master Sequence Timeline.
* Ensure the icons appear in all Sequences window tabs when the Unity Editor opens.

## [2.0.0-pre.1] - 2022-04-18

### Added
* Sequence and Sequence Asset management features:
  * New context menu action available from the empty area of the Sequences window to create a new Master Sequence.
  * New context menu action in the Sequence Assets window to open and edit the Prefabs (base and variants).
  * New context menu action in the Sequences window to instantiate an Editorial structure in the active Scene.
  * Keyboard shortcut to duplicate a Sequence Asset Variant: control-D (Windows) / command-D (macOS).
  * Ability to multi-select and bulk-delete sequences from the Sequences window.
* Help icons with documentation links in the Sequences window, Sequence Assets window and Sequence Assembly window.
* Possibility to use the regular Unity Editor features to manually fix invalid sequences if needed.

### Changed
* Update of the Unity Editor minimum supported version to 2022.1.
* Users can now create Editorial structures with more than 3 levels.
* Split of the Sequences window into two separate windows:
  * The Sequences window is still the main entry point to use Sequences and focuses on managing editorial structures.
  * The new Sequence Assets window replaces the Asset Collections panel and allows managing Sequence Assets and Variants.
* Sequence recording functionality adjustments:
  * A single Record option is now available in the Sequences window context menu to simplify the recording flow.
  * The Record action of the Sequences window no longer automatically sets the output file path in the Recorder window.
* Compliance with Unity standard namings and workflows:
  * The default Sequences folder and file names no longer have spaces.
  * Users can now rename Sequence Asset Variants on creation.
  * Renaming a Sequence GameObject from the Hierarchy no longer renames the Sequence in the Sequences and Project windows.
* UI consolidation and clean-up to streamline the user experience:
  * Prevent users from editing an Editorial structure that is not loaded in the Hierarchy.
  * Remove the Sequence/Master Sequence creation actions from the Hierarchy. The Sequences window is now the unique place to do it.
  * Hide fields of the MasterSequence asset Inspector that don't currently have any effect.
* Public API clean-up: remove unused/deprecated API elements.

### Fixed
* Prevent Unity from throwing an exception when deleting a Sequence after removing all clips from a Master Sequence's timeline.

## [1.1.0] - 2022-01-11

* Update package version to 1.1.0.

## [1.1.0-pre.2] - 2021-12-13

### Added
* New API method overload for `SequenceAssetUtility.RemoveFromSequence()` with an optional parameter `InteractionMode` that allows to skip the user confirmation pop-up.

### Fixed
* Make sure users can swap Sequence Asset Variants from the Sequence Assembly window in any scenario.
* Make sure users can undo the "Match Editorial Content (Ripple)" action.
* Avoid issues when creating a Master Sequence from the Add menu while an existing Sequence is being renamed.
* Prevent the Structure view from showing an empty but selectable entry that might result from a sequence deletion.
* Prevent Unity from throwing an exception on new Sequence creation from the GameObject menu.
* Prevent Unity from throwing an exception when transforming a sequence's GameObject into a Prefab if the GameObject includes a Scene Loading Policy component.
* Prevent Unity from throwing an exception on `Assets/Sequences/` folder deletion.
* Prevent Unity from throwing an exception when finding in SceneManager a registered Scene that was actually unloaded but visible in the Hierarchy.
* Fix the recording end frame of sub-sequence to respect the parent sequence frame end when it is earlier than the sub-sequence's one.

## [1.1.0-pre.1] - 2021-10-11

### Added
* Add a "Rename" context menu option in Sequences window, applicable to any Sequence, Sequence Asset, and Variant.

### Changed
* Make sure Sequence GameObjects are inactive by default at their creation to minimize the use of system resources when Timeline is out of preview mode.

### Fixed
* Preload Sequence and Asset Collection icons to prevent them from flickering.
* Make sure to update the Timeline context according to the current Hierarchy content when opening a Sequence in Prefab isolation mode.
* Fix Timeline Window display when selecting a PlayableDirector inside a Sequence Asset, including related exception thrown in the Console.
* In Play mode, make sure the Sequence Assembly window displays a read-only list of the Sequence Assets contained in the current selected Sequence.
* Make sure users can still edit a Sequence that has been transformed into a Prefab.

## [1.0.3] - 2021-09-09

### Changed
* Change internal code to comply with UIToolkit changes in 2022.1.

## [1.0.2] - 2021-08-25

### Changed
* Update the help URL for the SequenceFilter, SequenceAsset and SceneLoadPolicy components.

### Fixed
* Ignore SequenceAsset components added on GameObjects that are not part of a Prefab instance when listing the actual Sequence Assets included in a Sequence.
* Ignore SequenceAsset renaming from the Sequence Assembly window if the input name is invalid.
* Properly delete a Sequence previously converted to a Prefab.
* Make sure the Structure treeview always keeps its current expanded/collapsed state.
* Ensure that the Timeline Window always displays the correct breadcrumb context and Timeline asset when selecting a Sequence Asset in the Hierarchy.
* Improved the handling of invalid sequence deletion from the Sequences window.
* Make sure to properly rename a Sequence when the user only appends a suffix to the current name.

## [1.0.0] - 2021-06-30

### Changed
* Optimize the Sequence Asset search method to improve the Editor responsiveness when selecting a Sequence or performing simple operations on it.
* Optimize caching to improve the overall responsiveness of the Sequences window.
* Use by default the Timeline framerate value from the Project Settings when creating a MasterSequence with no specified FPS value.

### Fixed
* Make sure the renaming of a MasterSequence works under all conditions and with all supported Unity versions.
* Prevent the Sequences Structure from displaying an invalid Sequence entry after the renaming of a MasterSequence.
* Fix the Sequence Asset renaming process to handle edge cases for whitespace and duplicate names.
* Ensure to always update and refresh the Sequence Assembly window and the Asset Collection view after renaming a Sequence Asset.
* Ensure that the Asset Collection captures all the Sequence Assets when the project imports.
* Fix cases where the Variant dropdowns of the Sequence Assembly window become unresponsive.
* Preserve the Timeline playhead position when updating a Prefab instance.
* Don't prevent users from changing the framerate (FPS) of a Timeline associated to a Sequence via the Timeline Window.
* Make the Sequences Window retain the state of the Structure treeview between domain reloads.
* Avoid throwing exception on prefabized Sequences when setting the Timeline breadcrumb.
* Complete the public API Documentation.


## [1.0.0-pre.6] - 2021-05-18

### Changed
* Consolidate the public API documentation generation.

### Fixed
* Prevent Sequences window treeviews from unfolding their whole hierarchy when they update.
* Prevent Scenes from getting dirty on first selection of a Sequence in the Hierarchy when no change actually occurred.
* Ensure to rename or delete any corresponding folder in the Project window when the user renames or deletes a Sequence from the Sequences window.
* Ensure to get a complete expected behavior when undoing a swap of Sequence Asset Variant done from the Sequence Assembly window.
* Ensure that the Scene Loading Policy component does not load Scenes that are already in memory.
* Ensure to assign the proper default material to the Scene Management sample according to the current render pipeline used in the project.
* Fix an issue that was throwing an exception when canceling the creation of a Sequence Asset.
* Fix an issue that was throwing an exception when opening the Profiler window as a standalone process.

## [1.0.0-pre.5] - 2021-04-27

### Added
* Added a Third Party Notices file in the package

## [1.0.0-pre.4] - 2021-04-26

### Changed
* Minor update in the CHANGELOG.

## [1.0.0-pre.3] - 2021-04-21

### Added
* Added additional tooltips to various fields and buttons in Sequences and SequenceAssembly windows.

### Changed
* Update all icons for Dark and Light themes as per latest UX design and for better visibility of selected items.
* Update the Unity Editor minimum required version to 2020.3.
* Update terminology and tooltips in the SceneLoadingPolicy component.
* Make the Add Scene Loading Policy button more straightforward in Sequence Filter component.

### Fixed
* Ensure to set the Recorder with the correct Start and End frame numbers when recording a Master Sequence.

## [1.0.0-pre.2] - 2021-04-09

### Changed
* Users can now delete the GameObject that corresponds to a Sequence in the Hierarchy (regular Unity workflow).
    * This action does not delete anything Sequence-related from the Project but invalidates the Sequence that remains in the Sequences window.
    * When a Sequence is invalid, the Sequences window now displays a visual cue according to the reason of the error.

### Fixed
* Ensure to preserve the "Match Editorial Content" option results in Timeline after restarting Unity.
* Ensure that the start time of a new sequence corresponds to the end time of the last created sibling sequence.

## [1.0.0-pre.1] - 2021-03-31

### Added
* New sample to show how the Scene Activation track works and introduce users to Scene Management features in general.
* Add confirmation step on deletion of Sequence-related items from the Hierarchy, Sequences, and Sequence Assembly windows.

### Changed
* Change the minimum requirements: Timeline 1.5.2 and Recorder 2.5.5.
* Global naming refactor around Performance, Take and Cinematic concepts:
    * Remove the old Performance/Take objects and functions from the API.
      Rename related functions and UI to match the new Sequence Asset and Collection type terminology.
    * Rename the Cinematic asset to MasterSequence. Rename the whole Cinematic related API to reflect the new Master Sequence/Sequence terminology.
    * Rename the package assemblies and namespaces to replace "CinematicToolbox" by the new package name "Sequences".
* Make the **Sequence Assembly** window read-only in Play mode and show the instance name of the SequenceAssets populating the selected Sequence.
* The Sequence Assembly "Name" field now shows the selected Sequence name instead of the related TimelineAsset name.

### Fixed
* When creating Sequences and Sub-Sequences, they are now always created consecutively in time.

## [1.0.0-exp.2] - 2021-02-26

### Changed
* Make the **Sequence Assembly** windows inaccessible in Play mode to prevent from getting errors.

### Fixed
* Error in Play mode when the Performance clip editor tries to display information about null objects.

## [1.0.0-exp.1] - 2021-02-22

### Changed
* Enable "Clip-in" option on Editorial clips since Timeline 1.5.2.
    * Editorial clips no longer automatically adapt their duration to their sub-editorial total duration.
    * Use the new `Match Editorial Content (Ripple)` action, available on Editorial tracks and clips, to adapt
      all selected clips duration to their respective sub-editorial total duration.
* Make the `Scene Load Policy` component more user friendly:
    * Automatically add needed scene in Build Settings when domain is `Player Build Only` or `Editor and Player Build`.
    * Load scenes in Editor Playmode without extra setup needed.
* Update documentation to reflect the new Sequence terminology and Sequence Asset structure.
* Rename `Sound` Sequence Asset type to `Audio`.
* Replace Asset Collections icons with Prefab and Prefab variant icons.
* Rename the assets in the UpgradeToSequence sample.

### Removed
* Cleaned-up unused icons and UXML / USS files.
* Removed the dummy search bar in Sequences window.

### Fixed
* Fix edge cases related to the upgrade from Performances to SequenceAssets.
* Sequences Structure window better reflect invalid sequences and Master sequences and prevent user to do unwanted actions.

## [0.1.0-preview.17] - 2021-02-12

### Added
* New Scene management features in the context of Sequences:
    * Scene Activation track in Timeline.
    * Scene loading workflow in the Sequences window.
* New sample with UpgradeToSequence script to help users convert their existing Performances into new Sequence Assets.

## [0.1.0-preview.16] - 2021-02-01

_First release under the new name: com.unity.sequences._

### Added
* New SequenceAsset API, which replaces the former Performance/Take API.

### Changed
* Change the package name to **Sequences (com.unity.sequences)**.
* Rename menus and labels according to the new namings.
* Update com.unity.settings-manager dependency to 1.0.3.
* Declare implicit dependencies to allow true isolation.
* Rename the PerformanceTrack and Clip to fit with the new SequenceAsset naming.
* Move the SequenceAsset track and Editorial track into a Sequencing menu (instead of Cinematic).
* Make private functions that shouldn't be exposed in the public API.

### Removed
* Remove former Live Capture integration.
* Remove the `Promote to Performance` action from the GameObject menu.

### Deprecated
* The Performance/Take API is now deprecated, replaced by the new SequenceAsset API.

### Fixed
* Fix serialization error of the Cinematic asset.
* Make the Cinematic.Save function more robust so it can be used in an Asset Postprocessor.

## [0.1.0-preview.15] - 2020-10-26

### Changed
* Organize the cinematic timeline tracks (Editorial, Performance and Storyboard) under a "Cinematic" menu item.
* Update the Performance types list with a new Sound type.
* Performance group and performance take can't be renamed. Enforce this behaviour from the Cinematic Explorer UI to prevent potential confusion.

### Fixed
* Fix NullRef exception occuring when deleting the selected take of a performance. The Manager UI now shows that take as "INVALID".
* Fix added GameObjects and Components that was not taken into account when switching take and checking for unsaved modifications.

## [0.1.0-preview.14] - 2020-09-30

### Changed
* Update UI element styles and made them properly display in Light theme (personal).
* Update Recorder dependency to 2.3.0-preview.3.

### Fixed
* Fix exception when renaming a VCam Performance that is still connected to a device.
* Fix the lost bindings on the EditorialClips when Editor opens.
* Fix the offset on start and end frame values between Recorder and Timeline.

## [0.1.0-preview.13] - 2020-09-14

### Changed
* Update requirement: `com.unity.virtual-production@3.0.0-preview.6`.
* Switch to a new VCam take will set Timeline's time at the beginning of the active shot.

### Fixed
* Fix VCam app not playing the entire active shot context. Only the active shot timeline used to be played.
* Fix VCam broken video stream under certain circumstances.

## [0.1.0-preview.12] - 2020-09-11

### Fixed
* Fix NullRef exceptions that could happen in the Cinematic Manager when manipulating takes.

## [0.1.0-preview.11] - 2020-09-10

### Fixed
* Fix compiler error with runtime builds.

## [0.1.0-preview.10] - 2020-09-09

### Changed
* The Cinematic Manager now displays the selected take in the performance widgets when folded.
* Remove the Scene track until we define the right workflow for scenes manipulations.
* When deleting a GameObject associated with a cinematic, ask user whether the associated asset(s) should be deleted as well or not.
* When swapping Take, ask user what to do with current take if it has unsaved modifications.
* Duplicating a take doesn't append "(Clone)" to its name anymore.
* Add audio listener component on VCam performance.

### Removed
* The `Create new Performance` item has been removed from the GameObject > Cinematic menu.

### Fixed
* Fix Cinematic Explorer not reflecting Cinematic element selection from the Hierarchy view.
* Fix Timeline's playhead jumping back to frame 0 when moving between cinematic contexts.
* Fix Virtual Camera live link toggle not always in sync with the LiveLink internal state.

## [0.1.0-preview.9] - 2020-09-02

### Added
* Add an EditorialTrack used to represent sequences and shots in the Timeline.
* Add a PerformanceTrack used to represent performances in the Timeline.

### Changed
* Enable latent selection in Cinematic Manager. It will stay active on the latest valid Cinematic element (cinematic, sequence, shot).
* Remove the NestedTimelineTrack (replaced by the EditorialTrack and PerformanceTrack, see in "Features").

### Fixed
* Fix Cinematic Manager not showing performances from a prefab instance of a shot.
* Fix the Virtual Camera not responding when trying to record more than one take.
* Fix Cinematic element getting destroyed when leaving Prefab isolation.
* Ensure performances are created and saved with unique name in folder with unique name.
* Fix the `Promote to Performance` menu item that was not available anymore.
* Fix a bug where duplicating a performance take that contains another number than the take number
  was incrementing the other number as well as the take number.
* Fix Cinematic Manager getting focused when a selection happens in the Hierarchy window.
* Fix Cinematic element creation not dirtying the active scene.
* Fix the switch of performance take that was throwing exceptions before and loosing timeline binding.
* Fix deletion of cinematic GameObject that was throwing a null exception when the Cinematic asset was already removed from the Project.
* Fix PlayableDirector's bindings field not properly cleaned up on performance removal.

## [0.1.0-preview.8] - 2020-08-20

### Changed
* Selection of any level of a Cinematic or of a performance sets the Timeline Window in the context - represented by the breadcrumb - of that selection.

### Fixed
* Fix button `New VCam` that would stay enabled regardless of the presence of Virtual Production in the project.
* Fix cinematic element buttons not being grayed out when they are not available in the contextual menu of the Hierarchy window.
* Fix performance renaming where related GameObject, Timeline tracks and clips were not renamed accordingly.
* Fix the cinematic and performance delete operations by also deleting related folders when needed.

## [0.1.0-preview.7] - 2020-08-07

### Fixed
* Clip in a NestedTimelineTrack track with "Match Content Duration" at true will match their content only if it's not empty.
* Fix regression introduced with `0.1.0-preview.6` where the Cinematic Manager would not display performances.

## [0.1.0-preview.6] - 2020-08-06

### Added
* Add a new kind of Performance: Virtual Camera performance. Must have `com.unity.virtual-production` 3.0.0-preview.4 or later installed to unlock this feature.
* Cinematic, sequences and shots can be recorded from the Cinematic Explorer window. Start time, end time, fps and output path are automatically set.
* Add a new type of Timeline Track and Clip: Storyboard Track and Storyboard Clip.
    * Storyboard Clip displays an image in overlay over the Game View.

### Changed
* Improve the editorial workflow:
    * Users can now create cinematic elements (Cinematic/Sequence/Shot) directly from the Hierarchy view using the GameObject menu or right-click.
	* Data and bindings are now serialized in scenes rather than in prefabs.
	* Prefabs, scene lighting and timelines are no longer generated when creating a new cinematic element.
	* Retired the use of Prefab tracks. Everything is now based on the NestedTimelineTrack.
* Cinematic, Sequences and Shots start time and duration are update when new items are added or removed from a Cinematic.
* NestedTimelineTrack now control the activation of the controlled PlayableDirector.
* Cinematic Timelines and Performances created in the context of a Cinematic/Sequence/Shot are organized within the AssetDatabase
  in folders that reflect their creation context.

## [0.1.0-preview.5] - 2020-07-22

### Changed
* Move `Clear Performance User Types` to the Cinematic Explorer menu, under `Reset Performance Types`.
* Move `Cinematic Toolbox` menu under `Window`.

## [0.1.0-preview.4] - 2020-07-15

### Fixed
* Fix bug in Cinematic Manager where PerformanceGroups were not updated when a Performance was deleted from the Explorer.
* Fix bug in Cinematic Explorer where creating a new PerformanceType would immediately throw an exception.
* Fix bug in Cinematic Explorer where pressing the Delete key would delete the selected items in both Cinematics and Performances tree views regardless of the focus.
* Fix bug in Cinematic Explorer where deleting the last take of a performance with the Delete key would execute and leave the Performance in an undefined state. A performance can't be empty.
* Fix name field always showing "filler text" in the Cinematic Manager window.

## [0.1.0-preview.3] - 2020-07-08

### Added
* Add drag and drop in hierarchy for cinematics.
* Add ability to create a Performance from a GameObject selection in Hierarchy window.

### Changed
* Update minimal Unity version to 2019.4.
* Update all icons in the Cinematic Explorer window.
* Streamline selection system with Cinematic's element.
    * Single left click opens the asset in the Cinematic Manager window.
    * Double left clicks executes the Single left click action and select the associated Timeline.
    * Single left click will no longer show the Cinematic Manager content in Inspector.
* Rename the Shot Manager window to Cinematic Manager.

## [0.1.0-preview.2] - 2020-06-18

### Added
* Add new type of Timeline track to load/unload scenes.
* PrefabTrack instantiate Prefab under the GameObject that own the PlayableDirector. Allow edition "in context".
* Add children track naming based on their role (Sequences or Shots).

## [0.1.0-preview.1] - 2020-06-10

### Added
* Add DeleteFolder(), RenameFolder(), IsRenameValid(), GenerateUniqueAssetName() and GetAssetPath() functions to CinematicAssetDatabase.
* Add name validation to Rename() function calls.
* Add ability to remove Performance from Shot Manager.
* Add GetFirstClip() function to TrackAssetExtensions.

### Fixed
* Fix bug in Cinematic/Shot/Performance Rename() where parent folder was not renamed.
* Fix bug in Cinematic/Shot/Performance Delete() where parent folder was not deleted.
* Fix bug where Cinematic Explorer window would not stay when reopening Uni.y.
* Fix refresh issue in the ShotManager's performances add dropdown menu when a Performance is created or deleted.
* Fix refresh issue in the ShotManager's take selection dropdown when a new take is created or renamed.
* Fix Performance take selection not being properly serialized.
* Fix bug in CinematicManager AddPerformance() where adding performance in a shot did not create a new track.
* Fix bug where Cinematic Explorer window would not stay when reopening Unity.
* Fix a bug where Cinematics/Performance Explorers would clip on the bottom when there are too many items and related resizing issues.
* Fix bug where the fps of a Shot's Timeline did not match the fps of the associated TimelineCinematicClip.
* Fix bug where the fps of a Performance's timeline did not match the fps of the associated Shot.
* Fix bug where Performance clip would not display name of associated Take.

## [0.1.0-preview] - 2020-05-28

_This is the first release of com.unity.cinematic-toolbox._

### Added
* Cinematic and Performance Explorer window.
* Shot Manager window.
