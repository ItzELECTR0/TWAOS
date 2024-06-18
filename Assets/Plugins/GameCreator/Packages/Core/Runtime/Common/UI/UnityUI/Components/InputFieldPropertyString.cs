using UnityEngine;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Input Field")]
    [Icon(RuntimePaths.GIZMOS + "GizmoUIInputField.png")]
    public class InputFieldPropertyString : UnityEngine.UI.InputField
    {
        [SerializeField] private bool m_SetFromSource = false;
        [SerializeField] private PropertySetString m_OnChangeSet = new PropertySetString();
        
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
            this.text = this.m_OnChangeSet.Get(this.m_Args);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeValue(string value)
        {
            this.m_OnChangeSet.Set(value, this.m_Args);
        }
    }
}