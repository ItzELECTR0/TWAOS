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
    public class StoragePlayerPrefs : TDataStorage
    {
        // HIERARCHY: -----------------------------------------------------------------------------

        public override Task DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            return Task.FromResult(1);
        }

        public override Task DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            return Task.FromResult(1);
        }

        public override Task<bool> HasKey(string key)
        {
            bool hasKey = PlayerPrefs.HasKey(key);
            return Task.FromResult(hasKey);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public override Task<object> Get(string key, Type type)
        {
            string json = PlayerPrefs.GetString(key, string.Empty);
            json = this.Cryptography.Decrypt(json);
            
            return Task.FromResult(string.IsNullOrEmpty(json) == false
                ? JsonUtility.FromJson(json, type)
                : null
            );
        }

        // SETTERS: -------------------------------------------------------------------------------

        public override Task Set(string key, object value)
        {
            string json = JsonUtility.ToJson(value);
            
            json = this.Cryptography.Encrypt(json);
            PlayerPrefs.SetString(key, json);
            
            return Task.FromResult(1);
        }

        public override Task Commit()
        {
            return Task.FromResult(1);
        }
    }
}