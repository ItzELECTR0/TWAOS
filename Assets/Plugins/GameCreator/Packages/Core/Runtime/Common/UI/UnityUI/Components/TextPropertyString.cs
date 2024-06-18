using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Text")]
    [Icon(RuntimePaths.GIZMOS + "GizmoUIText.png")]
    public class TextPropertyString : Text
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertyGetString m_Value = new PropertyGetString();
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Args m_Args;
        
        // INIT METHODS: --------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;

            this.m_Args = new Args(gameObject);
        }

        private void LateUpdate()
        {
            string newText = this.m_Value.Get(this.m_Args);
            if (newText != this.text) this.text = newText;
        }
    }
}