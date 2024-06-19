# Changelog
All notable changes to this package will be documented in this file.

## [1.3.0] - 2023-01-30
- Added the `TransformHandle`'s function `GetLocalToParentMatrix` to get the matrix of an animation stream transform in local space.
- Added the `TransformHandle`'s function `GetLocalToWorldMatrix` to get the matrix of an animation stream transform in world space.
- Fixed handling negative scale in the `MultiAimConstraintJob` (case 1366549).
- Fixed transforms in animator hierarchy, but not children of avatar root not resolving properly (case 1373387).
- Fixed MultiAimConstraint evaluation with a world up axis (UM-1936).
- Fixed crash when calling `RigBuilder.Build` by preventing rebuilding the PlayableGraph when in a preview context (case UUM-8599).
- Fixed an issue where a misplaced `BoneHandles.shader` shader would cause the Scene View's Orientation Overlay to no longer render (case UUM-20874).

## [1.2.0] - 2021-12-08
- Updated package icons (case 1361823).
- Fixed BoneRenderer to refresh bones rendered in viewport on a mouse drag and drop event (case 1361819).
- Fixed potential performance issue with rig effectors when used in a deep hierarchy.
- Fixed NullReferenceException when previewing animation and layers were not initialized in `RigBuilder.StartPreview` (case 1367857).
- Added the function RigBuilder.SyncLayers to manually update rigs and constraints prior to PlayableGraph evaluation.
- Added the function RigBuilder.Evaluate(float) to manually update and evaluate the RigBuilder.
- Added the function RigBuilder.Build(PlayableGraph) to build the RigBuilder Playable nodes in an external PlayableGraph.
- Added effector visuals for auto setup of Two-Bone IK.
- Correctly positioned effectors for auto setup of Two-Bone IK.

## [1.1.1] - 2021-08-23
### Patch Update of *Animation Rigging*.
- Added missing tooltips to components (case 1265274).
- Labels on components now support localization in supported regions.
- Upgraded Animation Rigging overlay toolbox to use new Unity overlays.

## [1.1.0] - 2021-04-09
### Patch Update of *Animation Rigging*.
- Fixed Auto Setup on TwoBoneIK throwing an ArgumentNullException instead of a warning message.
- Fixed MultiReferential constraint not writing driver transform to stream.
- Added exceptions in ReadWriteTransformHandle and ReadOnlyTransformHandle to cover invalid Transform references (case 1275002).
- Added functions ConstructConstraintDataPropertyName and ConstructCustomPropertyName to ConstraintsUtils public API.
- Fixed Gizmo icons reimporting when switching platforms.
- Added missing icon to TwistChainConstraint.
- Built-in RigConstraint components now support multi-object editing in the Inspector.
- Fixed bug causing multi-target constraints to add a new target simply by viewing them in the Inspector.
- Fixed collapsing Source Objects and Settings foldouts in the Inspector going on the undo stack.
	- Appearance of foldout groups now matches other Unity components.
- Appearance of WeightedTransformArray in the Inspector now matches default array control.
	- Header now supports dragging and dropping Transforms.
- Validate constraint weights and source object weights are within supported range when deserializing built-in constraints in the Editor.
- Improvements to help when creating custom constraints:
    - RigConstraint m_Data field no longer displays nested under a foldout in the Inspector by default.
    - RigConstraint.OnValidate() is now overridable in sub-classes.
	- Added WeightedTransformArray.OnValidate().
	- Added default PropertyDrawer for WeightedTransformArray that supports multi-select.
	- Added optional WeightRangeAttribute. It should now be used in conjunction with WeightedTransformArray fields instead of RangeAttribute to specify the control should use a slider.
- Updated samples to standard Samples/PackageName/Version/SampleName structure (case 1276329).
- Fixed BoneRenderer and RigEffectors not respecting layer visibility flags (case 1238204).
- Added support for nested struct properties tagged with the SyncSceneToStream attribute.
- Fixed MultiParent and MultiPosition constraints evaluating with an offset when root game object scale is not one (case 1246893).
- Fixed unnormalized rotation in TwoBoneIK constraint when hint weight is lower than one (case 1311769).
- Removed editor only conditional compilation on serialized data (case 1324071).
- Updated Burst to version 1.4.1.

## [1.0.3] - 2020-08-21
### Patch Update of *Animation Rigging*.
- Updated Burst to version 1.3.4.

## [1.0.2] - 2020-07-02
### Patch Update of *Animation Rigging*.
- Updated minimum unity release to 2020.2.0a16.

## [1.0.1] - 2020-06-26
### Verified release of *Animation Rigging*.
- Added world up vector options for MultiAim.
- Removed use of `latest` keyword in links to documentation (case 1233972).
- Removed error log in CustomOverlayAttribute (case 1253862).
- Disabled alt-picking on BoneRenderer in Scene View.  [AAA-102]
- Fixed unecessary GC Alloc in RigBuilder.Update().  [AAA-103]
- Fixed TwoBoneIK inverse constraint not taking dynamic bone lengths into account.
- Fixed ChainIK not applying chain rotation weight properly.

## [0.3.3] - 2020-03-06
### Patch Update of *Animation Rigging*.
- Improved RigEffector picking when using custom shapes.
- Improved RigEffector hover feedback when shape does not have line geometry.
- Adjusted MultiAim calculations to prevent rolling effects (case 1215736).
- Removed cached 'LimbLengths' in TwoBoneIKConstraintJob which lets users modify bone lengths dynamically.  [AAA-97]
- Fixed erroneous link to TwistChain constraint documentation in the ConstraintSamples readme.  [AAA-95]
- Fixed add and remove operations in ReorderableList when constraint is nested in prefab.  [AAA-98]
- Fixed undo operations in ReorderableList.  [AAA-98]
- Removed uses of Resources.Load in Runtime and Editor assemblies.

## [0.3.2] - 2020-01-23
### Patch Update of *Animation Rigging*.
- Updated documentation.

## [0.3.1] - 2020-01-14
### Patch Update of *Animation Rigging*.
- Removed unecessary files BakeUtils.cs.orig and BakeUtils.cs.orig.meta.
- Updated Burst to version 1.2.0-preview.12.

## [0.3.0] - 2019-10-18
### Patch Update of *Animation Rigging*.
- Added support for bidirectional-baking to existing constraints.

## [0.2.5] - 2019-11-20
### Patch Update of *Animation Rigging*.
- Adjusted calculations on MultiAim constrained axes. [AAA-86]

## [0.2.4] - 2019-11-06
### Patch Update of *Animation Rigging*.
- Added support for scene visibility and picking flags on BoneRenderer component and Rig effectors. [AAA-65]
- Fixed preview of disabled RigBuilder component in Animation Window and Timeline. [AAA-37]
- Fixed constrained axes in MultiAim not properly constraining local rotation. [AAA-86]
- Updated Animation Rigging samples and added a Readme file.

## [0.2.3] - 2019-07-24
### Patch Update of *Animation Rigging*.
- Increased the priority index of AnimationPlayableOutput in order for Animation Rigging to always evaluate after State Machine and Timeline.
- Fixed NullReferenceException in RigEffectorRenderer.
- Fixed TwoBoneIK evaluation when used on straight limbs by using hint target to define a valid IK plane.
- Fixed Multi-Parent, Multi-Rotation and Multi-Aim constraints to perform order independent rotation blending. [AAA-17]
- Fixed RigTransform component to work on all objects of an animator hierarchy not only specific to sub rig hierarchies. [AAA-18] 
- Fixed crash in RigSyncSceneToStreamJob when rebuilding jobs after having deleted all valid rigs (case 1167624).
- Fixed undo/redo issues with Rig Effectors set on Prefab instances (case 1162002).
- Fixed missing links to package documentation for MonoBehaviour scripts. [AAA-16]
- Added Vector3IntProperty and Vector3BoolProperty helper structs.
- Updated Burst to version 1.1.1.

## [0.2.2] - 2019-04-29
### Patch Update of *Animation Rigging*.
- Added Rig Effector visualization toolkit for Animation Rigging.
- Fixed Animation Rigging align operations not using the same selection order in Scene View and Hierarchy.
- Updated Burst to version 1.0.4.

## [0.2.1] - 2019-02-28
### Patch Update of *Animation Rigging*.
- Added Burst support to existing constraints.  The Animation Rigging package now depends on com.unity.burst.
- Upgraded weighted transform arrays in order for weights to be animatable.  The following constraints were modified and will require a manual update:
	- MultiAimConstraint
	- MultiParentConstraint
	- MultiPositionConstraint
	- MultiReferentialConstraint
	- TwistCorrection

## [0.2.0] - 2019-02-12

### Keyframing support for *Animation Rigging*.
- Changed RigBuilder to build and update the PlayableGraph for Animation Window.
- Added attribute [NotKeyable] to properties that shouldn't be animated.
- Removed 'sync' property flag on transform fields for constraints. Syncing scene data to the animation stream is now performed by marking a constraint field with [SyncSceneToStream].
- Fixed issue where constraint parameters were evaluated one frame late when animated.
- Added attribute [DisallowMultipleComponent] to constraints to avoid use of multiple constraints per Game Object.
- Updated constraints to use new AnimationStream API to reduce engine to script conversion.
- Added IAnimatableProperty helpers for Bool/Int/Float/Vector2/Vector3/Vector4 properties. 
- Added ReadOnlyTransformHandle and ReadWriteTransformHandle.

## [0.1.4] - 2018-12-21

### Patch Update of *Animation Rigging*.
- Fixed onSceneGUIDelegate deprecation warning in BoneRenderUtils

## [0.1.3] - 2018-12-21

### Patch Update of *Animation Rigging*.
- Fixed stale bone rendering in prefab isolation view.
- Updated constraints to have a transform sync scene to stream toggle only on inputs.
- Fixed Twist Correction component to have twist nodes with weight varying between [-1, 1]
- Added Maintain Offset dropdown to TwoBoneIK, ChainIK, Blend and Multi-Parent constraints
- Added Rig Transform component in order to tag extra objects not specified by constraints to have an influence in the animation stream
- Updated documentation and samples to reflect component changes

## [0.1.2] - 2018-11-29

### Patch Update of *Animation Rigging*.
- Added constraint examples to Sample folder (Samples/ConstraintExamples/AnimationRiggingExamples.unitypackage)
- Fixed links in documentation
- Updated package description

## [0.1.1] - 2018-11-26

### Patch Update of *Animation Rigging*.
- Improved blend constraint UI layout
- Fixed jittering of DampedTransform when constraint weight was in between 0 and 1
- Made generic interface of Get/Set AnimationJobCache functions
- Added separate size parameters for bones and tripods in BoneRenderer.
- Fixed NullReferenceException when deleting skeleton hierarchy while it's still being drawn by BoneRenderer.
- Fixed Reset and Undo operations on BoneRenderer not updating skeleton rendering.
- Reworked multi rig playable graph setup to have one initial scene to stream sync layer followed by subsequent rigs
- Prune duplicate rig references prior to creating sync to stream job
- Added passthrough conditions in animation jobs for proper stream values to be passed downstream when job weights are zero. Fixes a few major issues when character did not have a controller.
- Fixed bug in ChainIK causing chain to not align to full extent when target is out of reach
- Fixed TwoBoneIK bend normal strategy when limbs are collinear
- Reworked AnimationJobData classes to be declared as structs in order for their serialized members to be keyable within the Animation Window. 
- Renamed component section and menu item "Runtime Rigging" to "Animation Rigging"
- Added check in SyncToStreamJob to make sure StreamHandle is still valid prior to reading it's values.
- Adding first draft of package documentation.

## [0.1.0] - 2018-11-01

### This is the first release of *Animation Rigging*.
### Added
- RigBuilder component.
- Rig component.
- The following RuntimeRigConstraint components:
	- BlendConstraint
	- ChainIKConstraint
	- MultiAimConstraint
	- MultiParentConstraint
	- MultiPositionConstraint
	- MultiReferentialConstraint
	- OverrideTransform
	- TwistCorrection
