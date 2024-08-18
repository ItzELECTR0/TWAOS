using System;
using System.Collections;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    internal class PoolInstance : MonoBehaviour
    {
        [NonSerialized] private int m_PrefabId;
        [NonSerialized] private IEnumerator m_Coroutine;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        private void OnDisable()
        {
            this.CancelInvoke();
            
            if (ApplicationManager.IsExiting) return;
            PoolManager.Instance.OnDisableInstance(this.m_PrefabId, this);

            if (this.m_Coroutine == null) return;
            this.StopCoroutine(this.m_Coroutine);
        }

        private void OnDestroy()
        {
            if (ApplicationManager.IsExiting) return;
            PoolManager.Instance.OnDestroyInstance(this.m_PrefabId, this);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void OnCreate(int prefabId)
        {
            this.m_PrefabId = prefabId;
        }
        
        public void SetDuration(float duration)
        {
            this.m_Coroutine = this.TimeoutDisable(duration);
            this.StartCoroutine(this.m_Coroutine);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private IEnumerator TimeoutDisable(float duration)
        {
            WaitForSeconds wait = new WaitForSeconds(duration);
            yield return wait;

            this.gameObject.SetActive(false);
        }
    }
}