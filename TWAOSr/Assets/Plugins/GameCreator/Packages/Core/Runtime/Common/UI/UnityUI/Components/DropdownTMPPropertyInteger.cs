using System;
using TMPro;
using UnityEngine;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Dropdown - TextMeshPro")]
    [Icon(RuntimePaths.GIZMOS + "GizmoUIDropdownTMP.png")]
    public class DropdownTMPPropertyInteger : TMP_Dropdown
    {
        [SerializeField] private bool m_SetFromSource = false;
        [SerializeField] private PropertySetNumber m_OnChangeSet = new PropertySetNumber();

        // MEMBERS: -------------------------------------------------------------------------------

        private Args m_Args;
        
        // INIT METHODS: --------------------------------------------------------------------------
        
        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            
            this.m_Args = new Args(this.gameObject);
            
            if (this.m_SetFromSource) this.SetValueFromProperty();
            this.onValueChanged.AddListener(this.OnChangeValue);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetValueFromProperty()
        {
            this.value = (int) Math.Floor(this.m_OnChangeSet.Get(this.m_Args));
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeValue(int index)
        {
            this.m_OnChangeSet.Set(value, this.m_Args);
        }
    }
}