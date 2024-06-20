# Scene management

## Setting up Scene activation in a Sequence

To create a new Scene and have it activated during the time of a specific Sequence:

1. Right-click on the Sequence, and select Create Scene.
2. Select the location to store the Scene file and specify a name for it.

This action creates an empty Scene that you can further edit. It also creates a Scene Activation track in the selected Sequence in Timeline, and binds the new Scene to this track. Finally, it also additively loads the new Scene in the Hierarchy.

>**Note:** If you want to bind an existing Scene to the track instead of creating a new Scene, you need to create the Scene Activation track from the [Timeline window](timeline-window.md#scene-activation-track).

You can add Scene Activation tracks at any level of your Sequence structure, and you can use as many Scene activation tracks as you need in a single Sequence.

>**Important:** To see a Scene in your Scene and Game views when Unity activates it through a Scene Activation track, you must previously load it in addition to the Scene that contains your Sequences structure. Depending on your current task, you can:
>* [Load Scenes contextually](#loading-scenes-contextually) in Edit mode for any Sequence, directly from the Sequences window.
>* [Set up a Scene Loading Policy](#setting-up-a-scene-loading-policy) to automatically load Scenes in runtime.

## Loading Scenes contextually

If you have set up Scene Activation tracks in your Sequences structure, you can load any of the corresponding Scenes directly from the Sequences window, according to the context of the Sequence you are working on, in Edit mode.

To do this, right-click on the targeted Sequence and select one of the following actions:

| **Action** | **Description** |
|------------|-----------------|
| **Load Scenes** | Additively loads all the Scenes that are bound with a Scene Activation track at any level of the Sequences structure within the time range of the selected Sequence. |
| **Load specific Scene** | Allows you to additively load any specific Scene among the ones that are bound with a Scene Activation track at any level of the Sequences structure within the time range of the selected Sequence. |

## Setting up a Scene Loading Policy

If you have set up Scene Activation tracks in your Sequences structure, you might want Unity to automatically load the corresponding Scenes when you enter the Play mode or when you run a Player build of your project, to ensure that the result includes all of them whatever their current status in Edit mode.

To do this:
1. In the Sequences window, select the Master Sequence.
2. In the Inspector, in the **Sequence Filter** component, click on **Add Scene Loading Policy**.
3. Set up the [Scene Loading Policy](ref-components.md#scene-loading-policy) component according to your needs.
