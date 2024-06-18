using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Marker by ID")]
    [Category("Navigation/Marker by ID")]
    
    [Image(typeof(IconID), ColorTheme.Type.TextNormal)]
    [Description("Reference to a scene Marker game object by its ID")]

    [Serializable]
    public class GetGameObjectNavigationMarkerID : PropertyTypeGetGameObject
    {
        [SerializeField] 
        private PropertyGetString m_ID = new PropertyGetString();

        public override GameObject Get(Args args) => this.GetObject(args);

        private GameObject GetObject(Args args)
        {
            string id = this.m_ID.Get(args);
            Marker marker = Marker.GetMarkerByID(id);
            
            return marker != null ? marker.gameObject : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectNavigationMarkerID instance = new GetGameObjectNavigationMarkerID();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"Marker ID:{this.m_ID}";
        
        public override GameObject EditorValue
        {
            get
            {
                Marker[] instances = UnityEngine.Object.FindObjectsOfType<Marker>();
                
                string id = this.m_ID.ToString();
                if (string.IsNullOrEmpty(id)) return null;
                
                int hash = id.GetHashCode();
                foreach (Marker instance in instances)
                {
                    ISpatialHash spatialHash = instance;
                    if (spatialHash.UniqueCode == hash) return instance.gameObject;
                }

                return null;
            }
        }
    }
}