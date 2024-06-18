using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
        #if UNITY_EDITOR
        [SerializeField] private UnityEngine.Object m_Scene;
        #endif

        [SerializeField] private string m_SceneName;
        [SerializeField] private int m_SceneIndex;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Name => this.m_SceneName;
        public int Index => this.m_SceneIndex;
        
        // OPERATORS: -----------------------------------------------------------------------------

        public static implicit operator string(SceneReference sceneReference)
        {
            return sceneReference.Name;
        }
        
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? "(none)" : this.Name;
        }
        
        // STATIC METHODS: ------------------------------------------------------------------------

        public static int GetSceneIndex(string target)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                if (target == scenePath || sceneName == target) return i;
            }

            return -1;
        }
        
        // SERIALIZATION CALLBACKS: ---------------------------------------------------------------

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            #if UNITY_EDITOR
            
            if (AssemblyUtils.IsReloading) return;
            this.m_SceneName = this.m_Scene != null 
                ? this.m_Scene.name 
                : string.Empty;
            
            this.m_SceneIndex = GetSceneIndex(this.Name);
            
            #endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }
    }
}