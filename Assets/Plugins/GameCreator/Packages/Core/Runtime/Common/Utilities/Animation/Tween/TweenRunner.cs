using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    internal class TweenRunner : MonoBehaviour
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized]
        private readonly List<ITweenInput> m_Inputs = new List<ITweenInput>();
        
        // INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
        {
            this.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        }

        private void OnDisable()
        {
            this.CancelAll();
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        private void Update()
        {
            for (int i = this.m_Inputs.Count - 1; i >= 0; --i)
            {
                if (!this.m_Inputs[i].OnUpdate()) continue;

                this.m_Inputs[i].OnComplete();
                this.m_Inputs.RemoveAt(i);
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void To(ITweenInput input)
        {
            this.Cancel(input.Hash);
            if (input.Duration <= float.Epsilon)
            {
                input.OnUpdate();
                input.OnComplete();
            }
            else
            {
                this.m_Inputs.Add(input);
            }
        }

        public void Cancel(int hash)
        {
            for (int i = this.m_Inputs.Count - 1; i >= 0; --i)
            {
                if (this.m_Inputs[i].Hash != hash) continue;
                
                this.m_Inputs[i].OnCancel();
                this.m_Inputs.RemoveAt(i);
            }
        }

        public void CancelAll()
        {
            for (int i = this.m_Inputs.Count - 1; i >= 0; --i)
            {
                this.m_Inputs[i].OnCancel();
                this.m_Inputs.RemoveAt(i);
            }
        }
    }
}
