using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
	[AddComponentMenu("")]
	public class GlobalListVariablesManager : Singleton<GlobalListVariablesManager>, IGameSave
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnSubsystemsInit()
		{
			Instance.WakeUp();
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		[field: NonSerialized]
		private Dictionary<IdString, ListVariableRuntime> Values { get; set; }

		[field: NonSerialized] private HashSet<IdString> SaveValues { get; set; }

		// INITIALIZERS: --------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			this.Values = new Dictionary<IdString, ListVariableRuntime>();
			this.SaveValues = new HashSet<IdString>();

			GlobalListVariables[] listVariables = VariablesRepository.Get.Variables.ListVariables;
			foreach (GlobalListVariables entry in listVariables)
			{
				if (entry == null) return;
				Instance.RequireInit(entry);
			}

			_ = SaveLoadManager.Subscribe(this);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public object Get(GlobalListVariables asset, IListGetPick pick, Args args)
		{
			int count = this.Count(asset);
			int index = pick?.GetIndex(count, args) ?? -1;

			return this.Get(asset, index);
		}

		public object Get(GlobalListVariables asset, IListSetPick pick, Args args)
		{
			int count = this.Count(asset);
			int index = pick?.GetIndex(count, args) ?? -1;

			return this.Get(asset, index);
		}

		public object Get(GlobalListVariables asset, int index)
		{
			return this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)
				? runtime.Get(index)
				: null;
		}

		public string Title(GlobalListVariables asset, int index)
		{
			return this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)
				? runtime.Title(index)
				: string.Empty;
		}

		public Texture Icon(GlobalListVariables asset, int index)
		{
			return this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)
				? runtime.Icon(index)
				: null;
		}

		public void Set(GlobalListVariables asset, IListSetPick pick, object value, Args args)
		{
			int count = this.Count(asset);
			if (!this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)) return;

			int index = pick?.GetIndex(runtime, count, args) ?? 0;
			this.Set(asset, index, value);
		}

		public void Set(GlobalListVariables asset, int index, object value)
		{
			if (!this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)) return;

			runtime.Set(index, value);
			if (asset.Save) this.SaveValues.Add(asset.UniqueID);
		}

		public void Insert(GlobalListVariables asset, IListGetPick pick, object value, Args args)
		{
			int count = this.Count(asset);
			int index = pick?.GetIndex(count, args) ?? 0;

			this.Insert(asset, index, value);
		}

		public void Insert(GlobalListVariables asset, int index, object value)
		{
			if (!this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)) return;

			runtime.Insert(index, value);
			if (asset.Save) this.SaveValues.Add(asset.UniqueID);
		}

		public void Push(GlobalListVariables asset, object value)
		{
			this.Insert(asset, this.Count(asset), value);
		}

		public void Remove(GlobalListVariables asset, IListGetPick pick, Args args)
		{
			int count = this.Count(asset);
			int index = pick?.GetIndex(count, args) ?? 0;

			this.Remove(asset, index);
		}

		public void Remove(GlobalListVariables asset, int index)
		{
			if (!this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)) return;

			runtime.Remove(index);
			if (asset.Save) this.SaveValues.Add(asset.UniqueID);
		}

		public void Clear(GlobalListVariables asset)
		{
			for (int i = this.Count(asset) - 1; i >= 0; --i)
			{
				this.Remove(asset, i);
			}
		}

		public void Move(GlobalListVariables asset, IListGetPick pickA, IListGetPick pickB,
			Args args)
		{
			int count = this.Count(asset);

			int indexA = pickA?.GetIndex(count, args) ?? 0;
			int indexB = pickB?.GetIndex(count, args) ?? 0;

			this.Move(asset, indexA, indexB);
		}

		public void Move(GlobalListVariables asset, int source, int destination)
		{
			if (!this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)) return;

			runtime.Move(source, destination);
			if (asset.Save) this.SaveValues.Add(asset.UniqueID);
		}

		public int Count(GlobalListVariables asset)
		{
			return this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime)
				? runtime.Count
				: 0;
		}

		public bool Contains(GlobalListVariables asset, object value)
		{
			if (!this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime))
			{
				return false;
			}

			int count = this.Count(asset);
			for (int i = 0; i < count; ++i)
			{
				object entry = runtime.Get(i);
				if (entry != null && entry == value) return true;
			}

			return false;
		}

		public void Register(GlobalListVariables asset,
			Action<ListVariableRuntime.Change, int> callback)
		{
			if (this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime))
			{
				runtime.EventChange += callback;
			}
		}

		public void Unregister(GlobalListVariables asset,
			Action<ListVariableRuntime.Change, int> callback)
		{
			if (this.Values.TryGetValue(asset.UniqueID, out ListVariableRuntime runtime))
			{
				runtime.EventChange -= callback;
			}
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void RequireInit(GlobalListVariables asset)
		{
			if (this.Values.ContainsKey(asset.UniqueID)) return;

			ListVariableRuntime runtime = new ListVariableRuntime(asset.IndexList);
			runtime.OnStartup();

			this.Values[asset.UniqueID] = runtime;
		}

		// IGAMESAVE: -----------------------------------------------------------------------------

		public string SaveID => "global-list-variables";

		public LoadMode LoadMode => LoadMode.Greedy;
		public bool IsShared => false;

		public Type SaveType => typeof(SaveGroupListVariables);

		public object GetSaveData(bool includeNonSavable)
		{
			Dictionary<string, ListVariableRuntime> saveValues = new Dictionary<string, ListVariableRuntime>();

			foreach (KeyValuePair<IdString, ListVariableRuntime> entry in this.Values)
			{
				if (includeNonSavable)
				{
					saveValues[entry.Key.String] = entry.Value;
					continue;
				}

				GlobalListVariables asset = VariablesRepository.Get.Variables.GetListVariablesAsset(entry.Key);
				if (asset == null || !asset.Save) continue;
				
				saveValues[entry.Key.String] = entry.Value;
			}

			SaveGroupListVariables saveData = new SaveGroupListVariables(saveValues);
			return saveData;
		}

		public Task OnLoad(object value)
		{
			if (value is not SaveGroupListVariables saveData) return Task.FromResult(false);

			int numGroups = saveData.Count();
			for (int i = 0; i < numGroups; ++i)
			{
				IdString uniqueID = new IdString(saveData.GetID(i));
				
				IndexVariable[] variables = saveData.GetData(i).Variables.ToArray();
				ListVariableRuntime data = new ListVariableRuntime(
					saveData.GetData(i).TypeID,
					variables
				);

				this.Values[uniqueID] = data;
				data.OnStartup();
			}

			return Task.FromResult(true);
		}
	}
}