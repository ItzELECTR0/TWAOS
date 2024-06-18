using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    internal class PropSkin : IProp
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly GameObject m_Prefab;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Transform Bone => null;
        
        [field: NonSerialized] public GameObject Instance { get; private set; }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PropSkin(GameObject prefab)
        {
            this.m_Prefab = prefab;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Create(Animator animator)
        {
            if (animator == null) return;
            if (this.m_Prefab == null) return;

            this.Instance = SkinMeshUtils.PutOn(this.m_Prefab, animator);
        }

        public void Destroy()
        {
            if (this.Instance == null) return;
            SkinMeshUtils.TakeOff(this.Instance);
        }

        public void Drop()
        {
            Debug.LogError("Skinned Mesh Renderers cannot be dropped. Use Destroy() instead");
            Destroy();
        }
    }
}