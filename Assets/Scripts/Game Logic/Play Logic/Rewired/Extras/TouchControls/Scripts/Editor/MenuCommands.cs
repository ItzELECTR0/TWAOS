namespace Rewired.Editor.TouchControls {
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.Reflection;

    public static class MenuCommands {

        private const string classPath = "Rewired.Editor.MenuItems, Rewired_Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        
        [MenuItem(Consts.menuRoot + "/Create/Controls/Touch Controls/Touch Canvas")]
        [MenuItem("GameObject/UI/Rewired/Touch Controls/Touch Canvas", false)]
        [MenuItem("GameObject/Create Other/Rewired/Touch Controls/Touch Canvas", false)]
        public static void CreateTouchCanvas(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTouchCanvas", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Touch Controls/Touch Controller")]
        [MenuItem("GameObject/UI/Rewired/Touch Controls/Touch Controller", false)]
        [MenuItem("GameObject/Create Other/Rewired/Touch Controls/Touch Controller", false)]
        public static void CreateTouchController(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTouchController", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Touch Controls/Touch Button")]
        [MenuItem("GameObject/UI/Rewired/Touch Controls/Touch Button", false)]
        [MenuItem("GameObject/Create Other/Rewired/Touch Controls/Touch Button", false)]
        public static void CreateTouchButton(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTouchButton", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Touch Controls/Touch Joystick")]
        [MenuItem("GameObject/UI/Rewired/Touch Controls/Touch Joystick", false)]
        [MenuItem("GameObject/Create Other/Rewired/Touch Controls/Touch Joystick", false)]
        public static void CreateTouchJoystick(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTouchJoystick", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Touch Controls/Touch Pad")]
        [MenuItem("GameObject/UI/Rewired/Touch Controls/Touch Pad", false)]
        [MenuItem("GameObject/Create Other/Rewired/Touch Controls/Touch Pad", false)]
        public static void CreateTouchPad(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTouchPad", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Touch Controls/Touch Region")]
        [MenuItem("GameObject/UI/Rewired/Touch Controls/Touch Region", false)]
        [MenuItem("GameObject/Create Other/Rewired/Touch Controls/Touch Region", false)]
        public static void CreateTouchRegion(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTouchRegion", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Other Controls/Tilt Control")]
        [MenuItem("GameObject/Create Other/Rewired/Other Controls/Tilt Control", false)]
        public static void CreateTiltControl(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateTiltControl", menuCommand);
        }

        [MenuItem(Consts.menuRoot + "/Create/Controls/Other Controls/Custom Controller")]
        [MenuItem("GameObject/Create Other/Rewired/Other Controls/Custom Controller", false)]
        public static void CreateCustomController(MenuCommand menuCommand) {
            InvokeMenuCommand("CreateCustomController", menuCommand);
        }
        
        // Private

        private static void InvokeMenuCommand(string methodName, MenuCommand command) {
            if(string.IsNullOrEmpty(methodName)) throw new ArgumentNullException("methodName");
            try {
                InvokeMethod(classPath, methodName, null, new object[] { command }, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            } catch(Exception ex) {
                Debug.LogError("There was an exception running the command:\n" + ex);
            }
        }

        private static object InvokeMethod(string classPath, string methodName, object target, object[] parameters, BindingFlags bindingFlags) {
            Type type = Type.GetType(classPath, false, false);
            if(type == null) throw new Exception("Class not found: " + classPath);

            MethodInfo mi = type.GetMethod(methodName, bindingFlags);
            if(mi == null) throw new Exception("Method not found: " + methodName);

            return mi.Invoke(target, parameters);
        }
    }
}