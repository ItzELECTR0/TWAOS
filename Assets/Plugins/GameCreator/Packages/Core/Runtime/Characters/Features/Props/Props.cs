using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Props
    {
        [field: NonSerialized] public static GameObject LastPropAttachedInstance { get; private set; }
        [field: NonSerialized] public static GameObject LastPropAttachedPrefab { get; private set; }
        
        [field: NonSerialized] public static GameObject LastPropDetachedInstance { get; private set; }
        [field: NonSerialized] public static GameObject LastPropDetachedPrefab { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnLoad()
        {
            LastPropAttachedInstance = null;
            LastPropAttachedPrefab = null;
            LastPropDetachedInstance = null;
            LastPropDetachedPrefab = null;
        }
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Dictionary<int, List<IProp>> m_Props;
        [NonSerialized] private Character m_Character;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Transform, GameObject> EventAdd;
        public event Action<Transform> EventRemove;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Props()
        {
            this.m_Props = new Dictionary<int, List<IProp>>();
        }
        
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        internal void OnStartup(Character character)
        {
            this.m_Character = character;
            this.m_Character.EventAfterChangeModel += this.OnChangeModel;
        }
        
        internal void AfterStartup(Character character)
        { }

        internal void OnDispose(Character character)
        {
            this.m_Character = character;
            this.m_Character.EventAfterChangeModel -= this.OnChangeModel;
        }

        internal void OnEnable()
        { }

        internal void OnDisable()
        { }

        // GETTER METHODS: ------------------------------------------------------------------------

        public bool HasInstance(GameObject instance)
        {
            if (instance == null) return false;
            int instanceID = instance.GetInstanceID();
            
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                foreach (IProp prop in entry.Value)
                {
                    if (prop.Instance == null) continue;
                    if (prop.Instance.GetInstanceID() == instanceID) return true;
                }
            }

            return false;
        }
        
        // PROP METHODS: --------------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the prefab at the specified bone location with the right
        /// coordinates.
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject AttachPrefab(IBone bone, GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;
            
            int instanceID = prefab.GetInstanceID();
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props))
            {
                props = new List<IProp>();
                this.m_Props.Add(instanceID, props);
            }
            
            PropPrefab prop = new PropPrefab(bone, prefab, position, rotation);
            prop.Create(this.m_Character.Animim.Animator);
            
            props.Add(prop);
            
            LastPropAttachedInstance = prop.Instance;
            LastPropAttachedPrefab = prefab;
            
            this.EventAdd?.Invoke(prop.Bone, prop.Instance);

            return prop.Instance;
        }
        
        /// <summary>
        /// Attaches an existing game object instance at the specified bone location with the right
        /// coordinates.
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="instance"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject AttachInstance(IBone bone, GameObject instance, Vector3 position, Quaternion rotation)
        {
            if (instance == null) return null;
            
            int instanceID = instance.GetInstanceID();
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props))
            {
                props = new List<IProp>();
                this.m_Props.Add(instanceID, props);
            }
            
            PropInstance prop = new PropInstance(bone, instance, position, rotation);
            prop.Create(this.m_Character.Animim.Animator);
            
            props.Add(prop);
            
            LastPropAttachedInstance = prop.Instance;
            LastPropAttachedPrefab = null;
            
            this.EventAdd?.Invoke(prop.Bone, prop.Instance);

            return prop.Instance;
        }

        /// <summary>
        /// Removes an instance of the prefab. If there are multiple instances, it removes the
        /// oldest one.
        /// </summary>
        /// <param name="prefab"></param>
        public void RemovePrefab(GameObject prefab)
        {
            if (prefab == null) return;
            int instanceID = prefab.GetInstanceID();
            
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props)) return;
            if (props.Count <= 0) return;

            int removeIndex = props.Count - 1;
            Transform bone = props[removeIndex].Bone;
            
            props[removeIndex].Destroy();
            props.RemoveAt(removeIndex);

            LastPropDetachedInstance = null;
            LastPropDetachedPrefab = prefab;
            
            this.EventRemove?.Invoke(bone);
        }

        /// <summary>
        /// Removes the specific instance of an instance of a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="instanceID"></param>
        public void RemovePrefab(GameObject prefab, int instanceID)
        {
            if (prefab == null) return;
            int prefabInstanceID = prefab.GetInstanceID();

            if (!this.m_Props.TryGetValue(prefabInstanceID, out List<IProp> props)) return;
            if (props.Count <= 0) return;

            for (int i = 0; i < props.Count; i++)
            {
                IProp prop = props[i];
                
                if (prop.Instance == null) continue;
                if (prop.Instance.GetInstanceID() != instanceID) continue;

                Transform bone = prop.Bone;

                prop.Destroy();
                props.RemoveAt(i);
                
                LastPropDetachedInstance = null;
                LastPropDetachedPrefab = prefab;

                this.EventRemove?.Invoke(bone);
                return;
            }
        }
        
        /// <summary>
        /// Removes a specific instance.
        /// </summary>
        /// <param name="instance"></param>
        public void RemoveInstance(GameObject instance)
        {
            if (instance == null) return;
            int instanceID = instance.GetInstanceID();
            
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props)) return;
            if (props.Count <= 0) return;

            int removeIndex = props.Count - 1;
            Transform bone = props[removeIndex].Bone;
            
            props[removeIndex].Destroy();
            props.RemoveAt(removeIndex);
            
            LastPropDetachedInstance = null;
            LastPropDetachedPrefab = null;
            
            this.EventRemove?.Invoke(bone);
        }
        
        /// <summary>
        /// Detaches an instance of the prefab from the Character. If there are multiple
        /// instances, it removes the oldest one.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject DropPrefab(GameObject prefab)
        {
            if (prefab == null) return null;
            int instanceID = prefab.GetInstanceID();

            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props)) return null;
            if (props.Count <= 0) return null;

            int removeIndex = props.Count - 1;
            Transform bone = props[removeIndex].Bone;

            GameObject instance = props[removeIndex].Instance;
            
            props[removeIndex].Drop();
            props.RemoveAt(removeIndex);
            
            LastPropDetachedInstance = instance;
            LastPropDetachedPrefab = prefab;
            
            this.EventRemove?.Invoke(bone);
            return instance;
        }

        /// <summary>
        /// Detaches a specific instance of the prefab from the Character
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public GameObject DropPrefab(GameObject prefab, int instanceID)
        {
            if (prefab == null) return null;
            int prefabInstanceID = prefab.GetInstanceID();

            if (!this.m_Props.TryGetValue(prefabInstanceID, out List<IProp> props)) return null;
            if (props.Count <= 0) return null;

            for (int i = 0; i < props.Count; i++)
            {
                IProp prop = props[i];
                
                if (prop.Instance == null) continue;
                if (prop.Instance.GetInstanceID() != instanceID) continue;

                Transform bone = prop.Bone;
                GameObject instance = prop.Instance;

                prop.Drop();
                props.RemoveAt(i);

                LastPropDetachedInstance = instance;
                LastPropDetachedPrefab = prefab;
                
                this.EventRemove?.Invoke(bone);
                return instance;
            }

            return null;
        }
        
        /// <summary>
        /// Drops a specific instance.
        /// </summary>
        /// <param name="instance"></param>
        public void DropInstance(GameObject instance)
        {
            if (instance == null) return;
            int instanceID = instance.GetInstanceID();
            
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props)) return;
            if (props.Count <= 0) return;

            int removeIndex = props.Count - 1;
            Transform bone = props[removeIndex].Bone;
            
            props[removeIndex].Drop();
            props.RemoveAt(removeIndex);
            
            LastPropDetachedInstance = instance;
            LastPropDetachedPrefab = null;
            
            this.EventRemove?.Invoke(bone);
        }

        /// <summary>
        /// Removes all props associated with a bone
        /// </summary>
        /// <param name="bone"></param>
        public void RemoveAtBone(IBone bone)
        {
            Transform boneTransform = bone.GetTransform(this.m_Character.Animim.Animator);
            if (boneTransform == null) return;
            
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                for (int i = entry.Value.Count - 1; i >= 0; --i)
                {
                    IProp prop = entry.Value[i];
                    if (prop.Bone != boneTransform) continue;

                    prop.Destroy();
                    entry.Value.RemoveAt(i);
                    
                    LastPropDetachedInstance = null;
                    LastPropDetachedPrefab = null;
                    
                    this.EventRemove?.Invoke(boneTransform);
                }
            }
        }
        
        /// <summary>
        /// Drops all props associated with a bone
        /// </summary>
        /// <param name="bone"></param>
        public void DropAtBone(IBone bone)
        {
            Transform boneTransform = bone.GetTransform(this.m_Character.Animim.Animator);
            if (boneTransform == null) return;
            
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                for (int i = entry.Value.Count - 1; i >= 0; --i)
                {
                    IProp prop = entry.Value[i];
                    if (prop.Bone != boneTransform) continue;

                    prop.Drop();
                    entry.Value.RemoveAt(i);
                    
                    LastPropDetachedInstance = prop.Instance;
                    LastPropDetachedPrefab = null;
                    
                    this.EventRemove?.Invoke(boneTransform);
                }
            }
        }

        /// <summary>
        /// Removes all props
        /// </summary>
        public void RemoveAll()
        {
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                foreach (IProp prop in entry.Value)
                {
                    prop.Destroy();
                    
                    LastPropDetachedInstance = null;
                    LastPropDetachedPrefab = null;
                    
                    this.EventRemove?.Invoke(prop.Bone);
                }
            }
            
            this.m_Props.Clear();
        }
        
        /// <summary>
        /// Drops all props
        /// </summary>
        public void DropAll()
        {
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                foreach (IProp prop in entry.Value)
                {
                    prop.Drop();
                    
                    LastPropDetachedInstance = prop.Instance;
                    LastPropDetachedPrefab = null;
                    
                    this.EventRemove?.Invoke(prop.Bone);
                }
            }
            
            this.m_Props.Clear();
        }

        /// <summary>
        /// Returns true if the specified bone contains at least one prop
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public bool HasAtBone(IBone bone)
        {
            Transform boneTransform = bone.GetTransform(this.m_Character.Animim.Animator);
            if (boneTransform == null) return false;
            
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                for (int i = entry.Value.Count - 1; i >= 0; --i)
                {
                    IProp prop = entry.Value[i];
                    if (prop?.Bone != boneTransform) continue;

                    return true;
                }
            }

            return false;
        }
        
        // SKINNED MESH METHODS: ------------------------------------------------------------------

        public GameObject AttachSkinMesh(GameObject prefab)
        {
            if (prefab == null) return null;
            
            int instanceID = prefab.GetInstanceID();
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props))
            {
                props = new List<IProp>();
                this.m_Props.Add(instanceID, props);
            }
            
            PropSkin prop = new PropSkin(prefab);
            prop.Create(this.m_Character.Animim.Animator);
            
            props.Add(prop);
            
            LastPropAttachedInstance = prop.Instance;
            LastPropAttachedPrefab = prefab;
            
            this.EventAdd?.Invoke(null, prop.Instance);

            return prop.Instance;
        }

        public void RemoveSkinMesh(GameObject prefab)
        {
            if (prefab == null) return;
            int instanceID = prefab.GetInstanceID();
            
            if (!this.m_Props.TryGetValue(instanceID, out List<IProp> props)) return;
            if (props.Count <= 0) return;

            int removeIndex = props.Count - 1;

            props[removeIndex].Destroy();
            props.RemoveAt(removeIndex);
            
            LastPropDetachedPrefab = prefab;
            LastPropDetachedInstance = null;
            
            this.EventRemove?.Invoke(null);
        }
        
        // CALLBACKS: -----------------------------------------------------------------------------

        private void OnChangeModel()
        {
            foreach (KeyValuePair<int, List<IProp>> entry in this.m_Props)
            {
                foreach (IProp prop in entry.Value)
                {
                    prop.Destroy();
                    this.EventRemove?.Invoke(prop.Bone);
                    
                    prop.Create(this.m_Character.Animim.Animator);
                    this.EventAdd?.Invoke(prop.Bone, prop.Instance);
                }
            }
        }
    }
}