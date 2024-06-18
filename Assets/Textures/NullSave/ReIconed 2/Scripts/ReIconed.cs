using System.Collections.Generic;
using UnityEngine;

namespace NullSave
{
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Rewired.InputManager))]
    public class ReIconed : MonoBehaviour
    {

        #region Variables

        // List of associated maps
        public List<ControllerMap> controllerMaps;

        // Map lists
        private static List<Rewired.ActionElementMap> axisMaps;
        private static List<Rewired.ActionElementMap> buttonMaps;

        #endregion

        #region Properties

        // Instance of ReIconed
        public static ReIconed Instance;

        #endregion

        #region Unity Methods

        // Instance and create lists
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                axisMaps = new List<Rewired.ActionElementMap>();
                buttonMaps = new List<Rewired.ActionElementMap>();
            }
        }

        #endregion

        #region Public Methods

        public static Rewired.Controller GetActiveController(int playerId)
        {
            return Rewired.ReInput.players.GetPlayer(playerId).controllers.GetLastActiveController();
        }

        public static InputMap GetActionHardwareInput(string action, ReIconedUpdateType updateType)
        {
            return GetActionHardwareInput(action, 0, updateType);
        }

        public static InputMap GetActionHardwareInput(int actionId, ReIconedUpdateType updateType)
        {
            return GetActionHardwareInput(actionId, 0, updateType);
        }

        public static InputMap GetActionHardwareInput(string action, int playerId, ReIconedUpdateType updateType)
        {
            if (Rewired.ReInput.players == null || Rewired.ReInput.players.playerCount <= 0) return null;

            ReIconedModifiers mod = ActionModifiers(ref action);
            int actionId = Rewired.ReInput.mapping.GetActionId(action);

            return GetActionHardwareInput(Rewired.ReInput.players.GetPlayer(playerId), actionId, mod, updateType);
        }


        public static InputMap GetActionHardwareInput(int actionId, int playerId, ReIconedUpdateType updateType)
        {
            if (Rewired.ReInput.players == null || Rewired.ReInput.players.playerCount <= 0) return null;

            return GetActionHardwareInput(Rewired.ReInput.players.GetPlayer(playerId), actionId, ReIconedModifiers.None, updateType);
        }


        #endregion

        #region Private Methods

        private static ReIconedModifiers ActionModifiers(ref string action)
        {
            ReIconedModifiers modifiers = ReIconedModifiers.None;

            if (action[action.Length - 1] == '+')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.Positive;
            }
            else if (action[action.Length - 1] == '-')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.Negative;
            }
            else if (action[action.Length - 1] == '/')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.Dual;
            }
            else if (action[action.Length - 1] == '*')
            {
                action = action.Substring(0, action.Length - 1);
                modifiers = ReIconedModifiers.All;
            }

            return modifiers;
        }

        private static InputMap GetActionHardwareInput(Rewired.Player player, int actionId, ReIconedModifiers modifiers, ReIconedUpdateType updateType)
        {
            InputMap map = null;

            // Get Maps
            player.controllers.maps.GetAxisMapsWithAction(actionId, true, axisMaps);
            player.controllers.maps.GetButtonMapsWithAction(actionId, true, buttonMaps);

            if (updateType == ReIconedUpdateType.PreferKeyboard)
            {
                map = GetKeyboardMouseMapping(player, modifiers);
                if (map != null) return map;
            }

            if (updateType == ReIconedUpdateType.PreferController)
            {
                map = GetJoystickMapping(player, modifiers);
                if (map != null) return map;

                map = GetKeyboardMouseMapping(player, modifiers);
                if (map != null) return map;
            }

            map = GetLastActiveInputMap(player, modifiers);
            if (map != null) return map;

            // finally use keyboad/mouse
            return GetKeyboardMouseMapping(player, modifiers);
        }

        private InputMap GetActionInput(ControllerMap map, string inputName)
        {
            foreach (InputMap input in map.inputMaps)
            {
                if (input.inputName == inputName)
                {
                    input.TMPSpriteAsset = map.tmpSpriteAsset;
                    return input;
                }
            }

            if (inputName[inputName.Length - 1] == '+' || inputName[inputName.Length - 1] == '-' || inputName[inputName.Length - 1] == '/' || inputName[inputName.Length - 1] == '*')
            {
                inputName = inputName.Substring(0, inputName.Length - 1);
                foreach (InputMap input in map.inputMaps)
                {
                    if (input.inputName == inputName)
                    {
                        input.TMPSpriteAsset = map.tmpSpriteAsset;
                        return input;
                    }
                }
            }

            return null;
        }

        private static InputMap GetActionMapJoystick(List<Rewired.ActionElementMap> maps, Rewired.Player player, ReIconedModifiers mod)
        {
            foreach (Rewired.ActionElementMap map in maps)
            {
                if (map.enabled)
                {
                    foreach (Rewired.Controller controller in player.controllers.Joysticks)
                    {
                        if (controller.enabled && controller.isConnected && (controller.type == Rewired.ControllerType.Custom || controller.type == Rewired.ControllerType.Joystick))
                        {
                            foreach (Rewired.ControllerElementIdentifier element in controller.ElementIdentifiers)
                            {
                                if (element.name == map.elementIdentifierName)
                                {
                                    return Instance.GetMapping(controller.hardwareTypeGuid.ToString(), SelectMapName(map, mod));
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static InputMap GetControllerMapping(Rewired.Controller controller, ReIconedModifiers modifiers, bool matchMapModifiers)
        {
            // Button maps
            foreach (var map in buttonMaps)
            {
                if (map.enabled && (!matchMapModifiers || MapMatchesModifiers(map, modifiers)))
                {
                    if (map.controllerMap.controller == controller)
                    {
                        foreach (Rewired.ControllerElementIdentifier element in controller.ElementIdentifiers)
                        {
                            if (element.name == map.elementIdentifierName)
                            {
                                return GetInputMapForController(controller, SelectMapName(map, modifiers));
                            }
                        }
                    }
                }
            }

            // Axis maps
            foreach (var map in axisMaps)
            {
                if (map.enabled && (!matchMapModifiers || MapMatchesModifiers(map, modifiers)))
                {
                    if (map.controllerMap.controller == controller)
                    {
                        foreach (Rewired.ControllerElementIdentifier element in controller.ElementIdentifiers)
                        {
                            if (element.name == map.elementIdentifierName)
                            {
                                return GetInputMapForController(controller, SelectMapName(map, modifiers));
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static InputMap GetInputMapForController(Rewired.Controller controller, string mapName)
        {
            if (controller.type == Rewired.ControllerType.Keyboard || controller.type == Rewired.ControllerType.Mouse)
            {
                return Instance.GetMapping("Keyboard", mapName);
            }

            return Instance.GetMapping(controller.hardwareTypeGuid.ToString(), mapName);
        }

        private static InputMap GetJoystickMapping(Rewired.Player player, ReIconedModifiers modifiers)
        {
            InputMap result = null;

            result = GetActionMapJoystick(axisMaps, player, modifiers);
            if (result != null) return result;

            result = GetActionMapJoystick(buttonMaps, player, modifiers);

            return result;
        }

        private static InputMap GetKeyboardMouseMapping(Rewired.Player player, ReIconedModifiers modifiers)
        {
            InputMap result = null;

            result = GetControllerMapping(player.controllers.Keyboard, modifiers, true);
            if (result != null) return result;

            result = GetControllerMapping(player.controllers.Mouse, modifiers, true);
            return result;
        }

        private static InputMap GetLastActiveInputMap(Rewired.Player player, ReIconedModifiers modifiers)
        {
            InputMap result = null;

            if (player != null)
            {
                // Get active controller
                Rewired.Controller controller = player.controllers.GetLastActiveController();
                if (controller != null)
                {
                    if (controller.type == Rewired.ControllerType.Keyboard || controller.type == Rewired.ControllerType.Mouse)
                    {
                        // These need to be grouped together
                        result = GetKeyboardMouseMapping(player, modifiers);
                    }
                    else
                    {
                        // Get mapping
                        result = GetControllerMapping(controller, modifiers, false);
                    }
                }
            }

            return result;
        }

        private InputMap GetMapping(string hardwareId, string inputName)
        {
            ControllerMap defaultMap = null;
            InputMap actionInput;

            foreach (ControllerMap map in controllerMaps)
            {
                if (map.compatibleDevices.Contains(hardwareId))
                {
                    actionInput = GetActionInput(map, inputName);
                    if (actionInput != null) return actionInput;
                }

                if (map.isFallback)
                {
                    defaultMap = map;
                }
            }

            if (defaultMap != null)
            {
                return GetActionInput(defaultMap, inputName);
            }

            return null;
        }

        private static bool MapMatchesModifiers(Rewired.ActionElementMap map, ReIconedModifiers mod)
        {
            switch (mod)
            {
                case ReIconedModifiers.Negative:
                    return map.axisContribution == Rewired.Pole.Negative;
                case ReIconedModifiers.Positive:
                    return map.axisContribution == Rewired.Pole.Positive;
                default:
                    return true;
            }
        }

        private static string SelectMapName(Rewired.ActionElementMap map, ReIconedModifiers modifiers)
        {
            switch (modifiers)
            {
                case ReIconedModifiers.All:
                    return map.elementIdentifierName + "*";
                case ReIconedModifiers.Dual:
                    return map.elementIdentifierName + "/";
                case ReIconedModifiers.Negative:
                    return map.elementIdentifierName + "-";
                case ReIconedModifiers.Positive:
                    return map.elementIdentifierName + "+";
                default:
                    return map.elementIdentifierName;
            }
        }

        #endregion

    }
}