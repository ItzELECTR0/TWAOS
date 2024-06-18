using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Common.SaveSystem
{
    [Title("Player Prefs")]
    [Category("Player Prefs")]
    
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Blue)]
    [Description("Store all game information using Unity Player Prefs")]
    
    [Serializable]
    public class StoragePlayerPrefs : IDataStorage
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        string IDataStorage.Title => "Player Prefs";
        string IDataStorage.Description => "Store all game information using Unity Player Prefs";
        
        // HIERARCHY: -----------------------------------------------------------------------------

        Task IDataStorage.DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            return Task.FromResult(1);
        }

        Task IDataStorage.DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            return Task.FromResult(1);
        }

        Task<bool> IDataStorage.HasKey(string key)
        {
            bool hasKey = PlayerPrefs.HasKey(key);
            return Task.FromResult(hasKey);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public Task<object> GetBlob(string key, Type type, object value)
        {
            string json = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(json)) value = JsonUtility.FromJson(json, type);

            return Task.FromResult(value);
        }

        Task<string> IDataStorage.GetString(string key, string value)
        {
            value = PlayerPrefs.GetString(key, value);
            return Task.FromResult(value);
        }

        Task<double> IDataStorage.GetDouble(string key, double value)
        {
            value = PlayerPrefs.GetFloat(key, (float) value);
            return Task.FromResult(value);
        }

        Task<int> IDataStorage.GetInt(string key, int value)
        {
            value = PlayerPrefs.GetInt(key, value);
            return Task.FromResult(value);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public Task SetBlob(string key, object value)
        {
            string json = JsonUtility.ToJson(value);

            PlayerPrefs.SetString(key, json);
            return Task.FromResult(1);
        }

        Task IDataStorage.SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            return Task.FromResult(1);
        }

        Task IDataStorage.SetDouble(string key, double value)
        {
            PlayerPrefs.SetFloat(key, (float) value);
            return Task.FromResult(1);
        }

        Task IDataStorage.SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            return Task.FromResult(1);
        }

        public Task Commit()
        {
            return Task.FromResult(1);
        }
    }
}