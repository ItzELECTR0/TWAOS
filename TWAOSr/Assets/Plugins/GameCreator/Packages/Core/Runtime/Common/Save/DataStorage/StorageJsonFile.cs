using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Json File")]
    [Category("Json File")]
    
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Yellow)]
    [Description("Stores all game information in a JSON file")]
    
    [Serializable]
    public class StorageJsonFile : TDataStorage
    {
        private const string FILE_NAME = "save.json";
        
        private static Dictionary<string, Entry> CacheData;
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode() => CacheData = null;
        
        [UnityEditor.MenuItem("Edit/Reveal 'Persistent Data' folder", false, 270)]
        private static void RevealPersistentDataFolder()
        {
            string path = Application.persistentDataPath;
            UnityEditor.EditorUtility.RevealInFinder(path);
        }
        
        #endif

        // HIERARCHY: -----------------------------------------------------------------------------

        public override Task DeleteAll()
        {
            Data.Clear();
            return Task.FromResult(1);
        }

        public override Task DeleteKey(string key)
        {
            Data.Remove(key);
            return Task.FromResult(1);
        }

        public override Task<bool> HasKey(string key)
        {
            bool hasKey = Data.ContainsKey(key);
            return Task.FromResult(hasKey);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public override Task<object> Get(string key, Type type)
        {
            Data.TryGetValue(key, out Entry entry);
            string json = entry?.Value ?? string.Empty;
            
            return Task.FromResult(string.IsNullOrEmpty(json) == false
                ? JsonUtility.FromJson(json, type)
                : null
            );
        }

        // SETTERS: -------------------------------------------------------------------------------

        public override Task Set(string key, object value)
        {
            string json = JsonUtility.ToJson(value, false);
            
            Data[key] = new Entry(key, json);
            return Task.FromResult(1);
        }
        
        public override Task Commit()
        {
            string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            
            try
            {
                string directory = Path.GetDirectoryName(path) ?? string.Empty;
                Directory.CreateDirectory(directory);
                
                Block content = new Block(CacheData);
                string json = JsonUtility.ToJson(content, false);

                json = this.Cryptography.Encrypt(json);
                
                using FileStream stream = new FileStream(path, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                
                writer.Write(json);
            }
            catch (Exception exception) 
            {
                Debug.LogError($"Error trying to save data: {exception}");
            }
            
            return Task.FromResult(1);
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // FILE PROPERTIES: -----------------------------------------------------------------------

        private Dictionary<string, Entry> Data
        {
            get
            {
                if (CacheData != null) return CacheData;
                
                CacheData = new Dictionary<string, Entry>();
                Block content = null;
                    
                string path = Path.Combine(Application.persistentDataPath, FILE_NAME);

                if (File.Exists(path)) 
                {
                    try 
                    {
                        string json;
                        using (FileStream stream = new FileStream(path, FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                json = reader.ReadToEnd();
                            }
                        }

                        json = this.Cryptography.Decrypt(json);
                        content = JsonUtility.FromJson<Block>(json);
                    }
                    catch (Exception exception) 
                    {
                        Debug.LogError($"Error trying to load data: {exception}");
                    }
                }

                foreach (Entry value in content?.Entries ?? Array.Empty<Entry>())
                {
                    CacheData[value.Key] = value;
                }

                return CacheData;
            }
        }

        // CLASSES: -------------------------------------------------------------------------------

        [Serializable]
        private class Block
        {
            [SerializeField] private Entry[] m_Entries;

            public Entry[] Entries => this.m_Entries;
            
            public Block(Dictionary<string, Entry> data)
            {
                this.m_Entries = new Entry[data.Count];
                int index = 0;
                
                foreach (KeyValuePair<string, Entry> entry in data)
                {
                    this.m_Entries[index] = entry.Value;
                    index += 1;
                }
            }
        }

        [Serializable]
        private class Entry
        {
            [SerializeField] private string m_Key;
            [SerializeField] private string m_Value;

            public string Key => this.m_Key;
            public string Value => this.m_Value;
            
            public Entry(string key, string value)
            {
                this.m_Key = key;
                this.m_Value = value;
            }
        }
    }
}