using TMPro;
using UnityEngine;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Input Field - TextMeshPro")]
    [Icon(RuntimePaths.GIZMOS + "GizmoUIInputFieldTMP.png")]
    public class InputFieldTMPPropertyString : TMP_InputField
    {
        [SerializeField] private bool m_SetFromSource;
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