using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Toggle")]
    [Icon(RuntimePaths.GIZMOS + "GizmoUIToggle.png")]
    public class TogglePropertyBool : Toggle
    {
        [SerializeField] private bool m_SetFromSource = false;
        [SerializeField] private PropertySetBool m_OnChangeSet = new PropertySetBool();

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
            this.isOn = this.m_OnChangeSet.Get(this.m_Args);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeValue(bool value)
        {
            this.m_OnChangeSet.Set(value, this.m_Args);
        }
    }
}