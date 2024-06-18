using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("Game Creator/UI/Touch Stick")]
    [Icon(RuntimePaths.GIZMOS + "GizmoTouchstick.png")]
    
    public class TouchStick : TTouchStick
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private GameObject m_Root;
        [SerializeField] private RectTransform m_Surface;
        [SerializeField] private RectTransform m_Stick;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override GameObject Root => this.m_Root;

        protected internal override RectTransform Surface => this.m_Surface;
        protected internal override RectTransform Stick => this.m_Stick;
    }
}