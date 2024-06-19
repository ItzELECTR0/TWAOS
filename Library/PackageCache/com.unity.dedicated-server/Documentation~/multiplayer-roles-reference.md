# Multiplayer Roles reference

This document describes the properties in the Project Settings window in the **Multiplayer** > **Multiplayer Roles** section.

|**Property**| **Description**|
|-|-|
|**Enable Multiplayer Roles**|Select the checkbox to set the [multiplayer role](multiplayer-roles.md) of GameObjects and components in this project.|
|**Enable safety check**|Select the checkbox to identify null reference exceptions that can happen when Unity strips GameObjects from a build.|
| **Strip Rendering Components:** | Removes the following components from the server or client:<br/>`Camera` component.<br/>`Light` component.<br/>All components that inherit from `Render`. |
| **Strip UI Components:**        | Removes the following components from the server or client.:<br/>All `UIElements` components.<br/>All components in the `UnityEngine.UI` namespace.<br/>All components in the `UnityEngine.EventSystems`namespace<br/>`UnityEngine.UIElements.PanelEventHandler` component<br/>`UnityEngine.UIElements.PanelRaycaster` component.<br/> |
| **Strip Audio Components:**     | Removes the following components from the server or client:<br/>`UnityEngine.AudioSource`component.<br/>`UnityEngine.AudioLowPassFilter`component<br/>`UnityEngine.AudioHighPassFilter`component<br/>`UnityEngine.AudioReverbFilter`component<br/>`UnityEngine.AudioListener`component<br/>`UnityEngine.AudioReverbZone`component<br/>`UnityEngine.AudioDistortionFilter`component<br/>`UnityEngine.AudioEchoFilter`component<br/>`UnityEngine.AudioChorusFilter`component. |
| **Strip Custom Components** | Select the **Add** icon (**+**) to add specific components to strip from the server or client. |
