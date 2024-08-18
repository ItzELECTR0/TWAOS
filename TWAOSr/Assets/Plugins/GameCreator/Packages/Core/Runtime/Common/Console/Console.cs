using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public static class Console
    {
        [field: NonSerialized] private static ConsoleUI ConsoleUI { get; set; }

        private const string PATH_CONSOLE_UI = "GameCreator/Console/Console";
        private const string MENU_ITEM_OPEN = "Game Creator/Console #%c";
        
        #if UNITY_EDITOR
        
        [UnityEditor.MenuItem(MENU_ITEM_OPEN, false, priority = 10)]
        public static void OpenConsole()
        {
            Open();
        }
        
        [UnityEditor.MenuItem(MENU_ITEM_OPEN, true, priority = 10)]
        public static bool OpenConsoleValidate()
        {
            return Application.isPlaying;
        }
        
        #endif
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool IsOpen => ConsoleUI != null && ConsoleUI.IsOpen;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnLoad()
        {
            ConsoleUI = null;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Submit(Input input)
        {
            IEnumerable<Output> outputs = Commands.Run(input);

            ConsoleUI console = RequestConsoleUI();
            foreach (Output output in outputs)
            {
                console.Print(output);   
            }
        }

        public static void Print(string text)
        {
            ConsoleUI console = RequestConsoleUI();
            console.Print(text);
        }

        public static void Clear()
        {
            ConsoleUI console = RequestConsoleUI();
            console.Clear();
        }

        public static void Open()
        {
            ConsoleUI console = RequestConsoleUI();
            console.Open();
        }

        public static void Close()
        {
            if (ConsoleUI == null) return;
            
            ConsoleUI console = RequestConsoleUI();
            console.Close();
        }

        public static void Toggle()
        {
            switch (IsOpen)
            {
                case true: Close(); break;
                case false: Open(); break;
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static ConsoleUI RequestConsoleUI()
        {
            if (ConsoleUI == null)
            {
                GameObject prefabConsoleUI = Resources.Load<GameObject>(PATH_CONSOLE_UI);
                GameObject instanceConsoleUI = UnityEngine.Object.Instantiate(prefabConsoleUI);

                Resources.UnloadUnusedAssets();
                
                ConsoleUI = instanceConsoleUI.GetComponent<ConsoleUI>();
                UnityEngine.Object.DontDestroyOnLoad(instanceConsoleUI);
            }

            return ConsoleUI;
        }
    }
}
