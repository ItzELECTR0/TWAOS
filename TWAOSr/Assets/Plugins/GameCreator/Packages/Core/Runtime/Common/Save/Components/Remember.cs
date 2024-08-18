using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/advanced/save-load-game/remember")]
    [AddComponentMenu("Game Creator/Save & Load/Remember")]
    [Icon(RuntimePaths.GIZMOS + "GizmoRemember.png")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_LAST)]
    [DisallowMultipleComponent]
    public class Remember : MonoBehaviour, IGameSave
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private SaveUniqueID m_SaveUniqueID = new SaveUniqueID(true);

        [SerializeField] 
        private Memories m_Memories = new Memories();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] internal bool IsDestroying { get; private set; }

        internal bool IsSceneLoaded => this.gameObject.scene.isLoaded;
        
        // INIT METHODS: --------------------------------------------------------------------------

        private void Awake()
        {
            _ = SaveLoadManager.Subscribe(this);
        }

        private void OnDestroy()
        {
            this.IsDestroying = true;
            SaveLoadManager.Unsubscribe(this);
        }

        // IGAMESAVE INTERFACE: -------------------------------------------------------------------

        public string SaveID => this.m_SaveUniqueID.Get.String;
        public bool IsShared => false;

        public Type SaveType => this.m_Memories.SaveType;

        public object GetSaveData(bool includeNonSavable)
        {
            return this.m_SaveUniqueID.SaveValue
                ? this.m_Memories.GetTokens(this.gameObject)
                : null;
        }
        
        public LoadMode LoadMode => LoadMode.Lazy;
        
        public Task OnLoad(object value)
        {
            this.m_Memories.OnRemember(
                this.gameObject,
                value as Tokens
            );

            return Task.FromResult(true);
        }
    }
}
