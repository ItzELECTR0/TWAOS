using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using GameCreator.Runtime.Common.SaveSystem;

namespace GameCreator.Runtime.Common
{
    public enum LoadMode
    {
        /// <summary>
        /// Lazy loading disables firing the OnLoad interface method when the
        /// SaveLoadSystem.Load() is executed. Instead, relies on the object's Start method
        /// to subscribe and load its data when the object is instantiated. This is the most
        /// commonly configuration used for the wide majority of situations. 
        /// </summary>
        Lazy,

        /// <summary>
        /// Greedy loading requires a persistent target (set as DontDestroyOnLoad) and forces
        /// its loading whenever the SaveLoadSystem.Load() method is executed. Commonly
        /// used with objects that follow the Singleton pattern.
        /// </summary>
        Greedy
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////

    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_LAST_LATER)]
    [AddComponentMenu("")]
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private const int SLOT_MIN = 1;
        private const int SLOT_MAX = 9999;

        private const string DB_KEY_FORMAT = "data-{0:D4}-{1}";

        // STRUCTS: -------------------------------------------------------------------------------

        private struct Reference
        {
            public IGameSave reference;
            public int priority;
        }

        private struct Value
        {
            public object value;
            public bool isShared;
        }

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Scenes m_Scenes;
        [NonSerialized] private Slots m_Slots;

        [NonSerialized] private Dictionary<string, Reference> m_Subscriptions;
        
        [NonSerialized] private Dictionary<string, Value> m_Values;
        [NonSerialized] private Dictionary<string, Value> m_ResetGreed;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public int SlotLoaded { get; private set; } = -1;
        public bool IsGameLoaded => this.SlotLoaded > 0;

        [field: NonSerialized] public bool IsSaving { get; private set; }
        [field: NonSerialized] public bool IsLoading { get; private set; }
        [field: NonSerialized] public bool IsDeleting { get; private set; }

        [field: NonSerialized] public IDataEncryption DataEncryption { get; private set; }
        [field: NonSerialized] public IDataStorage DataStorage { get; private set; }

        public float Progress => this.m_Scenes.Progress;
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action<int> EventBeforeSave;
        public event Action<int> EventAfterSave;

        public event Action<int> EventBeforeLoad;
        public event Action<int> EventAfterLoad;

        public event Action<int> EventBeforeDelete;
        public event Action<int> EventAfterDelete;

        // INITIALIZE: ----------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void InitializeOnLoad()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this.DataStorage = GeneralRepository.Get.Save?.Storage ?? new StoragePlayerPrefs();
            this.DataEncryption = GeneralRepository.Get.Save?.Encryption ?? new EncryptionNone();

            this.DataStorage.WithEncryption(this.DataEncryption);
            
            this.m_Subscriptions = new Dictionary<string, Reference>();
            this.m_Values = new Dictionary<string, Value>();
            this.m_ResetGreed = new Dictionary<string, Value>();
            
            this.m_Scenes = new Scenes();
            this.m_Slots = new Slots();

            _ = Subscribe(this.m_Scenes, 100);
            _ = Subscribe(this.m_Slots, 100);
        }

        // REGISTRY METHODS: ----------------------------------------------------------------------

        public static async Task Subscribe(IGameSave reference, int priority = 0)
        {
            if (ApplicationManager.IsExiting) return;
            
            Instance.m_Subscriptions[reference.SaveID] = new Reference
            {
                reference = reference,
                priority = priority
            };

            if (reference.LoadMode == LoadMode.Greedy && !reference.IsShared)
            {
                Instance.m_ResetGreed[reference.SaveID] = new Value
                {
                    value = reference.GetSaveData(true),
                    isShared = false
                };
            }

            switch (reference.LoadMode)
            {
                case LoadMode.Lazy:
                    if (Instance.m_Values.TryGetValue(reference.SaveID, out Value value))
                    {
                        await reference.OnLoad(value.value);
                    }
                    else if (Instance.IsGameLoaded)
                    {
                        await Instance.LoadItem(reference, Instance.SlotLoaded);
                    }
                    break;
                
                case LoadMode.Greedy:
                    if (reference.IsShared)
                    {
                        await Instance.LoadItem(reference, 0);
                    }
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static void Unsubscribe(IGameSave reference)
        {
            if (ApplicationManager.IsExiting) return;
            Instance.m_Subscriptions.Remove(reference.SaveID);
            
            if (Instance.IsLoading) return;
            Instance.m_Values[reference.SaveID] = new Value
            {
                value = reference.GetSaveData(false),
                isShared = reference.IsShared
            };
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool HasSave()
        {
            return this.m_Slots.Count > 0;
        }

        public bool HasSaveAt(int slot)
        {
            return this.m_Slots.ContainsKey(slot);
        }

        public string GetSaveDate(int slot)
        {
            return this.m_Slots.TryGetValue(slot, out Slots.Data data) ? data.date : string.Empty;
        }
        
        public async Task Save(int slot)
        {
            if (this.IsSaving || this.IsLoading || this.IsDeleting) return;

            this.EventBeforeSave?.Invoke(slot);

            this.IsSaving = true;

            foreach (KeyValuePair<string, Reference> item in this.m_Subscriptions)
            {
                if (item.Value.reference == null) continue;
                this.m_Values[item.Value.reference.SaveID] = new Value
                {
                    value = item.Value.reference.GetSaveData(false),
                    isShared = item.Value.reference.IsShared
                };
            }

            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, Value> entry in this.m_Values)
            {
                if (entry.Value.isShared) continue;
                keys.Add(entry.Key);
            }
            
            this.m_Slots.Update(slot, keys.ToArray());

            foreach (KeyValuePair<string, Value> item in this.m_Values)
            {
                string key = DatabaseKey(slot, item.Value.isShared, item.Key);
                await this.DataStorage.Set(key, item.Value.value);
            }

            await this.DataStorage.Commit();
            this.IsSaving = false;

            this.EventAfterSave?.Invoke(slot);
        }

        public async Task Load(int slot, Action callback = null)
        {
            if (this.IsSaving || this.IsLoading || this.IsDeleting) return;
            if (!this.HasSaveAt(slot)) return;

            this.EventBeforeLoad?.Invoke(slot);
            
            this.IsLoading = true;
            this.SlotLoaded = slot;

            this.m_Values.Clear();

            List<Reference> references = this.m_Subscriptions.Values.ToList();
            references.Sort((a, b) => b.priority.CompareTo(a.priority));

            for (int i = 0; i < references.Count; ++i)
            {
                IGameSave item = references[i].reference;
                if (item == null) continue;
                if (item.LoadMode == LoadMode.Lazy) continue;
                
                await this.ResetItem(references[i].reference);
                await this.LoadItem(references[i].reference, slot);
            }
            
            this.IsLoading = false;

            callback?.Invoke();
            this.EventAfterLoad?.Invoke(slot);
        }

        public async Task LoadLatest(Action callback = null)
        {
            int slot = this.m_Slots.LatestSlot;
            
            if (slot < 0) return;
            await this.Load(slot, callback);
        }

        public async Task Delete(int slot)
        {
            if (this.IsSaving || this.IsLoading || this.IsDeleting) return;

            this.EventBeforeDelete?.Invoke(slot);
            this.IsDeleting = true;

            if (this.m_Slots.TryGetValue(slot, out Slots.Data data))
            {
                for (int i = data.keys.Length - 1; i >= 0; --i)
                {
                    string dataKey = DatabaseKey(slot, false, data.keys[i]);
                    await this.DataStorage.DeleteKey(dataKey);
                }

                this.m_Slots.Remove(slot);

                string key = DatabaseKey(slot, this.m_Slots.IsShared, this.m_Slots.SaveID);
                await this.DataStorage.Set(key, this.m_Slots.GetSaveData(false));
            }

            await this.DataStorage.Commit();
            this.IsDeleting = false;
            
            this.EventAfterDelete?.Invoke(slot);
        }

        public async Task Restart(int sceneIndex, Action callback = null)
        {
            if (this.IsSaving || this.IsLoading || this.IsDeleting) return;
            
            this.EventBeforeLoad?.Invoke(-1);
            
            this.IsLoading = true;
            this.SlotLoaded = -1;
            
            this.m_Values.Clear();
            
            List<Reference> references = this.m_Subscriptions.Values.ToList();
            references.Sort((a, b) => b.priority.CompareTo(a.priority));

            await Scenes.LoadScene(sceneIndex);
            
            for (int i = 0; i < references.Count; ++i)
            {
                IGameSave item = references[i].reference;
                
                if (item == null) continue;
                if (item.SaveID == Scenes.ID) continue;
                
                if (item.LoadMode == LoadMode.Lazy) continue;
                if (item.IsShared) continue;
                
                await this.ResetItem(references[i].reference);
            }
            
            this.IsLoading = false;
            
            callback?.Invoke();
            this.EventAfterLoad?.Invoke(-1);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private async Task LoadItem(IGameSave reference, int slot)
        {
            string key = DatabaseKey(slot, reference.IsShared, reference.SaveID);

            object blob = await this.DataStorage.Get(key, reference.SaveType);
            await reference.OnLoad(blob);
        }
        
        private async Task ResetItem(IGameSave reference)
        {
            if (!this.m_ResetGreed.TryGetValue(reference.SaveID, out Value value)) return;
            if (reference.SaveID == Scenes.ID) return;
            
            await reference.OnLoad(value.value);
        }

        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static string DatabaseKey(int slot, bool isShared, string key)
        {
            slot = isShared ? 0 : Mathf.Clamp(slot, SLOT_MIN, SLOT_MAX);
            return string.Format(DB_KEY_FORMAT, slot, key);
        }
    }
}