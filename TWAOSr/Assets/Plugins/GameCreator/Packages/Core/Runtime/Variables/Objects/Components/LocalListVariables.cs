using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/variables/local-list-variables")]
    [Icon(RuntimePaths.GIZMOS + "GizmoLocalListVariables.png")]
    
    [AddComponentMenu("Game Creator/Variables/Local List Variables")]
    [DisallowMultipleComponent]
    
    [Serializable]
    public class LocalListVariables : TLocalVariables, IListVariable
    {
        // MEMBERS: -------------------------------------------------------------------------------
    
        [SerializeReference] private ListVariableRuntime m_Runtime = new ListVariableRuntime();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Count => this.m_Runtime.Count;

        public IdString TypeID => this.m_Runtime.TypeID;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action<ListVariableRuntime.Change, int> EventChange;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            this.m_Runtime.OnStartup();
            this.m_Runtime.EventChange += this.OnRuntimeChange;
            
            base.Awake();
        }
        
        public static LocalListVariables Create(GameObject target, ListVariableRuntime variables)
        {
            LocalListVariables instance = target.Add<LocalListVariables>();
            instance.m_Runtime = variables;
            instance.m_Runtime.OnStartup();

            instance.m_Runtime.EventChange += instance.OnRuntimeChange;
            return instance;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(IListGetPick pick, Args args)
        {
            int index = pick?.GetIndex(this.Count, args) ?? -1;
            return this.Get(index);
        }
        
        public object Get(IListSetPick pick, Args args)
        {
            int index = pick?.GetIndex(this.Count, args) ?? -1;
            return this.Get(index);
        }
        
        public object Get(int index)
        {
            return this.m_Runtime.Get(index);
        }

        public void Set(IListSetPick pick, object value, Args args)
        {
            int index = pick?.GetIndex(this.m_Runtime, this.Count, args) ?? 0;
            this.Set(index, value);
        }
        
        public void Set(int index, object value)
        {
            this.m_Runtime.Set(index, value);
        }

        public void Insert(IListGetPick pick, object value, Args args)
        {
            int index = pick?.GetIndex(this.Count, args) ?? 0;
            this.Insert(index, value);
        }
        
        public void Insert(int index, object value)
        {
            this.m_Runtime.Insert(index, value);
        }

        public void Push(object value)
        {
            this.m_Runtime.Push(value);
        }

        public void Remove(IListGetPick pick, Args args)
        {
            int index = pick?.GetIndex(this.Count, args) ?? 0;
            this.Remove(index);
        }
        
        public void Remove(int index)
        {
            this.m_Runtime.Remove(index);
        }

        public void Clear()
        {
            for (int i = this.Count - 1; i >= 0; --i)
            {
                this.Remove(i);
            }
        }

        public void Move(IListGetPick pickA, IListGetPick pickB, Args args)
        {
            int indexA = pickA?.GetIndex(this.Count, args) ?? 0;
            int indexB = pickB?.GetIndex(this.Count, args) ?? 0;
            
            this.Move(indexA, indexB);
        }

        public void Move(int source, int destination)
        {
            this.m_Runtime.Move(source, destination);
        }
        
        public void Register(Action<ListVariableRuntime.Change, int> callback)
        {
            this.EventChange += callback;
        }
        
        public void Unregister(Action<ListVariableRuntime.Change, int> callback)
        {
            this.EventChange -= callback;
        }

        public bool Contains(object value)
        {
            int count = this.Count;
            for (int i = 0; i < count; ++i)
            {
                object entry = this.m_Runtime.Get(i);
                if (entry != null && entry == value) return true;
            }

            return false;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnRuntimeChange(ListVariableRuntime.Change change, int index)
        {
            this.EventChange?.Invoke(change, index);
        }
        
        // IGAMESAVE: -----------------------------------------------------------------------------

        public override Type SaveType => typeof(SaveSingleListVariables);

        public override object GetSaveData(bool includeNonSavable)
        {
            return this.m_SaveUniqueID.SaveValue
                ? new SaveSingleListVariables(this.m_Runtime)
                : null;
        }

        public override Task OnLoad(object value)
        {
            SaveSingleListVariables saveData = value as SaveSingleListVariables;
            if (saveData != null && this.m_SaveUniqueID.SaveValue)
            {
                IndexVariable[] variables = saveData.Variables.ToArray();
                this.m_Runtime = new ListVariableRuntime(
                    saveData.TypeID, 
                    variables
                );
            }

            this.m_Runtime.OnStartup();
            return Task.FromResult(saveData != null || !this.m_SaveUniqueID.SaveValue);
        }
    }
}