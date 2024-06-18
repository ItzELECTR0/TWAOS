using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [AddComponentMenu("")]
    public class GlobalNameVariablesManager : Singleton<GlobalNameVariablesManager>, IGameSave
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnSubsystemsInit()
        {
            Instance.WakeUp();
        }
        
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] private Dictionary<IdString, NameVariableRuntime> Values { get; set; }

        [field: NonSerialized] private HashSet<IdString> SaveValues { get; set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();

            this.Values = new Dictionary<IdString, NameVariableRuntime>();
            this.SaveValues = new HashSet<IdString>();
            
            GlobalNameVariables[] nameVariables = VariablesRepository.Get.Variables.NameVariables;
            foreach (GlobalNameVariables entry in nameVariables)
            {
                if (entry == null) return;
                Instance.RequireInit(entry);   
            }

            _ = SaveLoadManager.Subscribe(this);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Exists(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(
                asset.UniqueID,
                out NameVariableRuntime runtime
            ) && runtime.Exists(name);
        }
        
        public object Get(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime) 
                ? runtime.Get(name) 
                : null;
        }
        
        public string Title(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime) 
                ? runtime.Title(name) 
                : string.Empty;
        }
        
        public Texture Icon(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime) 
                ? runtime.Icon(name) 
                : null;
        }

        public void Set(GlobalNameVariables asset, string name, object value)
        {
            if (!this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)) return;
            
            runtime.Set(name, value);
            if (asset.Save) this.SaveValues.Add(asset.UniqueID);
        }

        public void Register(GlobalNameVariables asset, Action<string> callback)
        {
            if (this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime))
            {
                runtime.EventChange += callback;
            }
        }
        
        public void Unregister(GlobalNameVariables asset, Action<string> callback)
        {
            if (this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime))
            {
                runtime.EventChange -= callback;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RequireInit(GlobalNameVariables asset)
        {
            if (this.Values.ContainsKey(asset.UniqueID)) return;
            
            NameVariableRuntime runtime = new NameVariableRuntime(asset.NameList);
            runtime.OnStartup();

            this.Values[asset.UniqueID] = runtime;
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string SaveID => "global-name-variables";

        public LoadMode LoadMode => LoadMode.Greedy;
        public bool IsShared => false;

        public Type SaveType => typeof(SaveGroupNameVariables);

        public object GetSaveData(bool includeNonSavable)
        {
            Dictionary<string, NameVariableRuntime> saveValues = new Dictionary<string, NameVariableRuntime>();
                        
            foreach (KeyValuePair<IdString, NameVariableRuntime> entry in this.Values)
            {
                if (includeNonSavable)
                {
                    saveValues[entry.Key.String] = entry.Value;
                    continue;
                }

                GlobalNameVariables asset = VariablesRepository.Get.Variables.GetNameVariablesAsset(entry.Key);
                if (asset == null || !asset.Save) continue;
                
                saveValues[entry.Key.String] = entry.Value;
            }

            SaveGroupNameVariables saveData = new SaveGroupNameVariables(saveValues);
            return saveData;
        }

        public Task OnLoad(object value)
        {
            if (value is not SaveGroupNameVariables saveData) return Task.FromResult(false);
        
            int numGroups = saveData.Count();
            for (int i = 0; i < numGroups; ++i)
            {
                IdString uniqueID = new IdString(saveData.GetID(i));
                List<NameVariable> candidates = saveData.GetData(i).Variables;

                if (!this.Values.TryGetValue(uniqueID, out NameVariableRuntime variables))
                {
                    continue;
                }
                
                foreach (NameVariable candidate in candidates)
                {
                    if (!variables.Exists(candidate.Name)) continue;
                    variables.Set(candidate.Name, candidate.Value);
                }
            }
            
            return Task.FromResult(true);
        }
    }
}