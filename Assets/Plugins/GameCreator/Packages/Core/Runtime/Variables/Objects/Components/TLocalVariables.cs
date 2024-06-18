using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public abstract class TLocalVariables : MonoBehaviour, IGameSave
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        protected SaveUniqueID m_SaveUniqueID = new SaveUniqueID();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void Awake()
        {
            _ = SaveLoadManager.Subscribe(this);
        }
        
        protected virtual void OnDestroy()
        {
            SaveLoadManager.Unsubscribe(this);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void ChangeId(IdString nextId)
        {
            if (this.m_SaveUniqueID.SaveValue)
            {
                Debug.LogError("Unable to change the Local Variable ID of a 'savable' component");
                return;
            }
            
            this.m_SaveUniqueID.Set = nextId;
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string SaveID => this.m_SaveUniqueID.Get.String;

        public bool IsShared => false;
        public LoadMode LoadMode => LoadMode.Lazy;
        
        public abstract Type SaveType { get; }

        public abstract object GetSaveData(bool includeNonSavable);
        public abstract Task OnLoad(object value);
    }
}