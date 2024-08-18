using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class RunnerPool
    {
        private const HideFlags INSTANCE_FLAGS = HideFlags.None;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly GameObject m_Template;
        
        [NonSerialized] private readonly List<GameObject> m_ReadyList;
        [NonSerialized] private readonly HashSet<GameObject> m_RunningList;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public RunnerPool(GameObject template, int prewarmCounter)
        {
            this.m_Template = template;
            this.m_Template.SetActive(false);
            
            this.m_ReadyList = new List<GameObject>(prewarmCounter);
            this.m_RunningList = new HashSet<GameObject>(prewarmCounter);

            for (int i = 0; i < prewarmCounter; ++i)
            {
                GameObject instance = this.CreateInstance();
                this.m_ReadyList.Add(instance);
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public TRunnerType Pick<TRunnerType>() where TRunnerType : Runner
        {
            GameObject instance = null;
            for (int i = this.m_ReadyList.Count - 1; i >= 0; --i)
            {
                instance = this.m_ReadyList[i];
                this.m_ReadyList.RemoveAt(i);
                
                if (instance != null) 
                {
                    this.m_RunningList.Add(instance);
                    break;
                }
            }

            if (instance == null)
            {
                instance = this.CreateInstance();
                this.m_RunningList.Add(instance);
            }

            return instance != null ? instance.Get<TRunnerType>() : null;
        }

        public void Restore(GameObject instance)
        {
            if (instance == null) return;
            if (this.m_RunningList.Remove(instance))
            {
                instance.transform.SetParent(this.m_Template.transform.parent);
                instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                
                instance.SetActive(false);
                this.m_ReadyList.Add(instance);
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private GameObject CreateInstance()
        {
            GameObject instance = UnityEngine.Object.Instantiate(
                this.m_Template,
                Vector3.zero,
                Quaternion.identity,
                this.m_Template.transform.parent
            );
            
            instance.name = $"{this.m_Template.name} (Runner)";
            instance.Get<Runner>().Template = this.m_Template;
            
            instance.hideFlags = INSTANCE_FLAGS;
            instance.SetActive(false);
            
            return instance;
        }
    }
}