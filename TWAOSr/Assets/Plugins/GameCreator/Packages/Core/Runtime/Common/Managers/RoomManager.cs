using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class RoomManager : Singleton<RoomManager>
    {
        private class Events : Dictionary<int, List<Action>>
        { }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Events m_Events = new Events();

        // ON CREATE: -----------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            SceneManager.sceneLoaded += this.OnLoadScene;
        }

        private void OnLoadScene(Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            int index = scene.buildIndex;
            if (!this.m_Events.TryGetValue(index, out List<Action> events)) return;
            
            foreach (Action action in events) action.Invoke();
            this.m_Events.Remove(index);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        /// <summary>
        /// Upon loading this scene, the Action parameter will be invoked just once
        /// </summary>
        /// <remarks>
        /// This is a useful tool for communication data between scenes. For example
        /// making the player start at a different position depending on the entry point
        /// to the scene
        /// </remarks>
        /// <param name="scene">The scene index in the Build Settings</param>
        /// <param name="action">The callback that will be executed after the scene is loaded</param>
        public void Subscribe(int scene, Action action)
        {
            if (!this.m_Events.TryGetValue(scene, out List<Action> events))
            {
                events = new List<Action>();
                this.m_Events.Add(scene, events);
            }
            
            events.Add(action);
        }
    }
}