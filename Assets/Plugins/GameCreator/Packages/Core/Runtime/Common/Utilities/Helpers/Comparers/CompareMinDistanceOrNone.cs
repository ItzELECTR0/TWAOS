using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class CompareMinDistanceOrNone
    {
        private enum MinDistance
        {
            None,
            GameObject
        }
        
        private static Color COLOR_EDITOR = new Color(0f, 1f, 0f, 0.25f);
        private static Color COLOR_IN = new Color(0f, 1f, 0f, 0.1f);
        private static Color COLOR_OUT = new Color(1f, 0f, 0f, 0.05f);
    
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private MinDistance m_MinDistance = MinDistance.None;
        [SerializeField] private PropertyGetGameObject m_To = GetGameObjectPlayer.Create();
        
        [SerializeField] private float m_Radius = 2f;
        [SerializeField] private Vector3 m_Offset = Vector3.zero;
    
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool NoDistance => this.m_MinDistance == MinDistance.None;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public CompareMinDistanceOrNone()
        { }

        public CompareMinDistanceOrNone(PropertyGetGameObject to) : this()
        {
            this.m_MinDistance = MinDistance.GameObject;
            this.m_To = to;
        }
    
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Match(Transform self, Args args)
        {
            if (this.NoDistance) return true;
            if (self == null) return false;
            
            GameObject gameObject = this.m_To.Get(args);
            if (gameObject == null) return false;

            Vector3 position = self.TransformPoint(this.m_Offset);
            return Vector3.Distance(position, gameObject.transform.position) <= this.m_Radius;
        }
        
        // GIZMOS: --------------------------------------------------------------------------------

        public void OnDrawGizmos(Transform self, Args args)
        {
            if (this.NoDistance) return;
            
            if (self == null) return;
            Vector3 position = self.TransformPoint(this.m_Offset);
            
            GameObject gameObject = this.m_To.Get(args);

            if (Application.isPlaying)
            {
                float distance = gameObject != null 
                    ? Vector3.Distance(position, gameObject.transform.position)
                    : Mathf.Infinity;
            
                Gizmos.color = distance <= this.m_Radius ? COLOR_IN : COLOR_OUT;   
            }
            else
            {
                Gizmos.color = COLOR_EDITOR;
            }

            GizmosExtension.Octahedron(position, Quaternion.identity, this.m_Radius, 5);
        }
    }
}