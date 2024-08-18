using System;
using System.IO;
using GameCreator.Runtime.Common.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class GeneralSave
    {
        [SerializeReference] private TDataEncryption m_Encryption = new EncryptionNone();
        [SerializeReference] private TDataStorage m_Storage = new StoragePlayerPrefs();
        
        [SerializeField] private LoadSceneMode m_Load = LoadSceneMode.AllSavedScenes;
        [SerializeField] private PropertyGetScene m_Scene = GetSceneActive.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public IDataEncryption Encryption => this.m_Encryption;
        public IDataStorage Storage => this.m_Storage;
        
        public LoadSceneMode Load => this.m_Load;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public string GetSceneName(Args args)
        {
            int index = this.m_Scene.Get(args);
            string path = SceneUtility.GetScenePathByBuildIndex(index);
            string name = Path.GetFileNameWithoutExtension(path);
            
            if (name == string.Empty) Debug.LogError("No Load Scene was specified");
            return name;
        }
    }
}