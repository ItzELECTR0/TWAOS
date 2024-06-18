using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Json File")]
    [Category("Json File")]
    
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Yellow)]
    [Description("Store all game information in a Json file")]
    
    [Serializable]
    public class StorageJsonFile : IDataStorage
    {
        private const string FILE_NAME = "save.json";
        
        private static Dictionary<string, StoreType> _Data;
        
        #if UNITY_EDITOR

        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode() => _Data = null;

        [UnityEditor.MenuItem("Edit/Reveal 'Persistent Data' folder", false, 270)]
        private static void RevealPersistentDataFolder()
        {
            string path = Application.persistentDataPath;
            UnityEditor.EditorUtility.RevealInFinder(path);
        }
        
        #endif
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private EnablerString m_Encrypt = new EnablerString(false, "Colloportus");

        // PROPERTIES: ----------------------------------------------------------------------------
        
        string IDataStorage.Title => "Json File";
        string IDataStorage.Description => "Store all game information in a Json file";

        // HIERARCHY: -----------------------------------------------------------------------------

        Task IDataStorage.DeleteAll()
        {
            Data.Clear();
            return Task.FromResult(1);
        }

        Task IDataStorage.DeleteKey(string key)
        {
            Data.Remove(key);
            return Task.FromResult(1);
        }

        Task<bool> IDataStorage.HasKey(string key)
        {
            bool hasKey = Data.ContainsKey(key);
            return Task.FromResult(hasKey);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public Task<object> GetBlob(string key, Type type, object value)
        {
            if (!Data.TryGetValue(key, out StoreType storeType)) return Task.FromResult(value);
            if (storeType is not StoreString storeString) return Task.FromResult(value);
            
            string json = storeString.Value;
                
            if (!string.IsNullOrEmpty(json)) value = JsonUtility.FromJson(json, type);
            return Task.FromResult(value);
        }

        Task<string> IDataStorage.GetString(string key, string value)
        {
            if (!Data.TryGetValue(key, out StoreType storeType)) return Task.FromResult(value);
            return Task.FromResult(storeType is StoreString storeString 
                ? storeString.Value 
                : value
            );
        }

        Task<double> IDataStorage.GetDouble(string key, double value)
        {
            if (!Data.TryGetValue(key, out StoreType storeType)) return Task.FromResult(value);
            return Task.FromResult(storeType is StoreDouble storeDouble 
                ? storeDouble.Value 
                : value
            );
        }

        Task<int> IDataStorage.GetInt(string key, int value)
        {
            if (!Data.TryGetValue(key, out StoreType storeType)) return Task.FromResult(value);
            return Task.FromResult(storeType is StoreInt storeInt 
                ? storeInt.Value 
                : value
            );
        }

        // SETTERS: -------------------------------------------------------------------------------

        public Task SetBlob(string key, object value)
        {
            string json = JsonUtility.ToJson(value);
            
            Data[key] = new StoreString(key, json);
            return Task.FromResult(1);
        }

        Task IDataStorage.SetString(string key, string value)
        {
            Data[key] = new StoreString(key, value);
            return Task.FromResult(1);
        }

        Task IDataStorage.SetDouble(string key, double value)
        {
            Data[key] = new StoreDouble(key, value);
            return Task.FromResult(1);
        }

        Task IDataStorage.SetInt(string key, int value)
        {
            Data[key] = new StoreInt(key, value);
            return Task.FromResult(1);
        }
        
        public Task Commit()
        {
            string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            
            try
            {
                string directory = Path.GetDirectoryName(path) ?? string.Empty;
                Directory.CreateDirectory(directory);
                
                Block content = new Block(_Data);
                string json = JsonUtility.ToJson(content, true);
                
                if (this.m_Encrypt.IsEnabled) json = Encrypt(json);

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

        private Dictionary<string, StoreType> Data
        {
            get
            {
                if (_Data != null) return _Data;
                
                _Data = new Dictionary<string, StoreType>();
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
                            
                        if (this.m_Encrypt.IsEnabled) json = Decrypt(json);
                        content = JsonUtility.FromJson<Block>(json);
                    }
                    catch (Exception exception) 
                    {
                        Debug.LogError($"Error trying to load data: {exception}");
                    }
                }

                foreach (StoreType value in content?.Values ?? Array.Empty<StoreType>())
                {
                    _Data[value.Key] = value;
                }

                return _Data;
            }
        }
        
        // ENCRYPTION: ----------------------------------------------------------------------------

        /// <summary>
        /// Encrypts (or rather 'hides') the plain json data using a simple XOR operation with
        /// a secret passcode key chosen by the user.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The text encrypted</returns>
        private string Encrypt(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                int secretIndex = i % this.m_Encrypt.Value.Length;
                output.Append(input[i] ^ this.m_Encrypt.Value[secretIndex]);
            }
            
            return output.ToString();
        }
        
        /// <summary>
        /// Decrypts (or rather 'reveals') the plain json data using a simple XOR operation with
        /// a secret passcode key chosen by the user.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The text decrypted</returns>
        private string Decrypt(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                int secretIndex = i % this.m_Encrypt.Value.Length;
                output.Append(input[i] ^ this.m_Encrypt.Value[secretIndex]);
            }
            
            return output.ToString();
        }

        // CLASSES: -------------------------------------------------------------------------------

        [Serializable]
        private class Block
        {
            [SerializeReference] private StoreType[] m_Values;

            public StoreType[] Values => this.m_Values;

            public Block(Dictionary<string, StoreType> data)
            {
                this.m_Values = new StoreType[data.Count];
                int index = 0;
                
                foreach (KeyValuePair<string, StoreType> entry in data)
                {
                    this.m_Values[index] = entry.Value;
                    index += 1;
                }
            }
        }

        [Serializable]
        private abstract class StoreType
        {
            [SerializeField] private string m_Key;

            public string Key => this.m_Key;

            protected StoreType(string key)
            {
                this.m_Key = key;
            }
        }

        [Serializable]
        private class StoreString : StoreType
        {
            [SerializeField] private string m_Value;

            public string Value => this.m_Value;
            
            public StoreString(string key, string value) : base(key)
            {
                this.m_Value = value;
            }
        }
        
        [Serializable]
        private class StoreDouble : StoreType
        {
            [SerializeField] private string m_Value;

            public double Value => Convert.ToDouble(this.m_Value);
            
            public StoreDouble(string key, double value) : base(key)
            {
                this.m_Value = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        
        [Serializable]
        private class StoreInt : StoreType
        {
            [SerializeField] private string m_Value;

            public int Value => Convert.ToInt32(this.m_Value);
            
            public StoreInt(string key, int value) : base(key)
            {
                this.m_Value = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}