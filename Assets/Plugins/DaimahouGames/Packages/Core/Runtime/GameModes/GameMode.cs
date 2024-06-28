using System.Collections.Generic;
using DaimahouGames.Runtime.Core;
using UnityEngine;

namespace DaimahouGames.Runtime.Core
{
    public static class GameMode
    {
        //============================================================================================================||
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        
        private static readonly Dictionary<int, Controller> m_Controllers = new();
        
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reload()
        {
            Debug.Log("Reload: Clearing controllers");
            m_Controllers.Clear();
        }
        
        // ※  Public Methods: --------------------------------------------------------------------------------------|※

        public static bool IsRegistered(Controller controller)
        {
            return m_Controllers.ContainsKey(controller.ID.Hash);
        }

        public static bool TryRegister(Controller controller)
        {
            if (IsRegistered(controller)) return false;
            AddController(controller);
            return true;
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        
        private static void AddController(Controller controller)
        {
            Debug.Log($"Registering Controller {controller.name}");
            m_Controllers.Add(controller.ID.Hash, controller);
        }
        
        //============================================================================================================||
    }
}