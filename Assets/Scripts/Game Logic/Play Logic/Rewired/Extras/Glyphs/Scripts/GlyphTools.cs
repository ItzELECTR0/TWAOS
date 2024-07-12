// Copyright (c) 2024 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

namespace Rewired.Glyphs {
    using System.Collections.Generic;

    public static class GlyphTools {

        public static bool TryGetActionElementMaps(
                int playerId,
                int actionId,
                AxisRange actionRange,
                ControllerElementGlyphSelectorOptions options,
                List<ActionElementMap> workingActionElementMaps,
                out ActionElementMap aemResult1,
                out ActionElementMap aemResult2
            ) {

            aemResult1 = null;
            aemResult2 = null;

            if (!ReInput.isReady) return false;
            if (options == null) return false;
            if (workingActionElementMaps == null) return false;

            InputAction action = ReInput.mapping.GetAction(actionId);
            if (action == null) return false;

            Player player = ReInput.players.GetPlayer(playerId);
            if (player == null) return false;

            // Get Player's last active controller
            Controller lastActiveController = player.controllers.GetLastActiveController();

            workingActionElementMaps.Clear();

            // Get all action element maps for the Action

            if (options.useLastActiveController && lastActiveController != null) {

                Controller otherController = null;

                // Treat keyboard and mouse as a single controller because most people want it to work this way.
                // Override last active controller with either the keyboard or mouse, whichever is higher priority.
                // This avoids glyphs switching as keyboard and mouse are used alternately.
                if (lastActiveController.type == ControllerType.Keyboard || lastActiveController.type == ControllerType.Mouse) {
                    if (IsMousePrioritizedOverKeyboard(options)) {
                        if (ReInput.controllers.Mouse.enabled && player.controllers.hasMouse) {
                            lastActiveController = ReInput.controllers.Mouse;
                            otherController = ReInput.controllers.Keyboard;
                        }
                    } else {
                        if (ReInput.controllers.Keyboard.enabled && player.controllers.hasKeyboard) {
                            lastActiveController = ReInput.controllers.Keyboard;
                            otherController = ReInput.controllers.Mouse;
                        }
                    }
                }

                // Prioritize last active controller
                if (player.controllers.maps.GetElementMapsWithAction(lastActiveController.type, lastActiveController.id, actionId, true, workingActionElementMaps) > 0) {
                    if (TryGetActionElementMaps(action, actionRange, workingActionElementMaps, out aemResult1, out aemResult2)) {
                        return true;
                    }
                }

                // Fallback to secondary for keyboard/mouse
                if (otherController != null) {
                    if (player.controllers.maps.GetElementMapsWithAction(otherController.type, otherController.id, actionId, true, workingActionElementMaps) > 0) {
                        if (TryGetActionElementMaps(action, actionRange, workingActionElementMaps, out aemResult1, out aemResult2)) {
                            return true;
                        }
                    }
                }

                // Fall back to last-known active controller type without the controller id.
                // This allows falling back to other Joysticks if binding is not found in the active one.
                if (player.controllers.maps.GetElementMapsWithAction(lastActiveController.type, actionId, true, workingActionElementMaps) > 0) {
                    if (TryGetActionElementMaps(action, actionRange, workingActionElementMaps, out aemResult1, out aemResult2)) {
                        return true;
                    }
                }
            }

            // Fall back to other controller types in order of priority
            {
                ControllerType controllerType;
                for (int i = 0; options.TryGetControllerTypeOrder(i, out controllerType); i++) {
                    if (player.controllers.maps.GetElementMapsWithAction(controllerType, actionId, true, workingActionElementMaps) > 0) {
                        if (TryGetActionElementMaps(action, actionRange, workingActionElementMaps, out aemResult1, out aemResult2)) {
                            return true;
                        }
                    }
                }
            }

            // Fall back to any controller type
            if (player.controllers.maps.GetElementMapsWithAction(actionId, true, workingActionElementMaps) > 0) {
                if (TryGetActionElementMaps(action, actionRange, workingActionElementMaps, out aemResult1, out aemResult2)) {
                    return true;
                }
            }

            return false;
        }
        public static bool TryGetActionElementMaps(
            InputAction action,
            AxisRange actionRange,
            List<ActionElementMap> tempAems,
            out ActionElementMap aemResult1,
            out ActionElementMap aemResult2
            ) {

            aemResult1 = null;
            aemResult2 = null;

            // An axis-type Action may be bound to multiple buttons / keys (hotzontal = dpad left, dpad right),
            // so this must be taken into account. Prioritize full-axis bindings. If none found, try to find binding pairs.
            bool isAxisAction = action.type == InputActionType.Axis;

            ActionElementMap aem;

            int aemCount = tempAems.Count;
            for (int i = 0; i < aemCount; i++) {

                // For axis-type Actions, must be able to support displaying split axis bindings to the Action (positive / negative).
                if (isAxisAction) {

                    if (actionRange == AxisRange.Full) {

                        // Try to get full binding first
                        aem = FindFirstFullAxisBinding(tempAems);
                        if (aem != null) {
                            aemResult1 = aem;
                            return true;
                        }

                        // Fall back to split bindings
                        ActionElementMap aem2;
                        if (FindFirstSplitAxisBindingPair(tempAems, out aem, out aem2)) {
                            aemResult1 = aem;
                            aemResult2 = aem2;
                            return true;
                        }

                    } else {
                        aem = FindFirstBinding(tempAems, actionRange);
                        if (aem != null) {
                            aemResult1 = aem;
                            return true;
                        }
                    }

                } else { // button type Action
                    aem = FindFirstBinding(tempAems, actionRange);
                    if (aem != null) {
                        aemResult1 = aem;
                        return true;
                    }
                }
            }

            return false;
        }

        public static ActionElementMap FindFirstFullAxisBinding(List<ActionElementMap> actionElementMaps) {
            int aemCount = actionElementMaps.Count;
            ActionElementMap aem;
            for (int i = 0; i < aemCount; i++) {
                aem = actionElementMaps[i];
                if (aem.elementType == ControllerElementType.Axis &&
                    aem.axisType == AxisType.Normal) {
                    return aem;
                }
            }
            return null;
        }

        public static ActionElementMap FindFirstBinding(List<ActionElementMap> actionElementMaps, AxisRange actionRange) {
            if (actionElementMaps.Count == 0) return null;
            int aemCount = actionElementMaps.Count;
            ActionElementMap aem;
            for (int i = 0; i < aemCount; i++) {
                aem = actionElementMaps[i];
                switch (actionRange) {
                    case AxisRange.Full:
                        if (aem.axisRange == AxisRange.Full) return aem;
                        break;
                    case AxisRange.Positive:
                        if (aem.axisType == AxisType.Split || aem.axisType == AxisType.None) {
                            if (aem.axisContribution == Pole.Positive) return aem;
                        }
                        break;
                    case AxisRange.Negative:
                        if (aem.axisType == AxisType.Split || aem.axisType == AxisType.None) {
                            if (aem.axisContribution == Pole.Negative) return aem;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Special case for a full range query to show a positive binding if available.
            // This is used for button bindings so the user doesn't have to explicitly specify
            // a positive Action Range to get a result.
            if (actionRange == AxisRange.Full) {
                for (int i = 0; i < aemCount; i++) {
                    aem = actionElementMaps[i];
                    if (aem.axisType == AxisType.Split || aem.axisType == AxisType.None) {
                        if (aem.axisContribution == Pole.Positive) return aem;
                    }
                }
            }

            return null;
        }

        public static bool FindFirstSplitAxisBindingPair(List<ActionElementMap> actionElementMaps, out ActionElementMap negativeAem, out ActionElementMap positiveAem) {
            negativeAem = null;
            positiveAem = null;
            ActionElementMap aem;
            int aemCount = actionElementMaps.Count;
            for (int i = 0; i < aemCount; i++) {
                aem = actionElementMaps[i];
                if (aem.elementType == ControllerElementType.Axis) {
                    if (aem.axisType == AxisType.Normal || aem.axisType == AxisType.None) {
                        continue;
                    }
                } else if (aem.elementType != ControllerElementType.Button) {
                    continue;
                }
                if (aem.axisContribution == Pole.Positive) {
                    if (positiveAem == null) positiveAem = aem;
                } else {
                    if (negativeAem == null) negativeAem = aem;
                }
            }
            return negativeAem != null || positiveAem != null;
        }

        public static bool IsMousePrioritizedOverKeyboard(ControllerElementGlyphSelectorOptions options) {
            if (options == null) return false;
            ControllerType controllerType;
            for (int i = 0; options.TryGetControllerTypeOrder(i, out controllerType); i++) {
                if (controllerType == ControllerType.Mouse) return true;
                if (controllerType == ControllerType.Keyboard) return false;
            }
            return false;
        }
    }
}
