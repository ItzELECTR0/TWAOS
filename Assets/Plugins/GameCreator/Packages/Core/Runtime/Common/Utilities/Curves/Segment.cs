using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct Segment
    {
	    public static readonly Segment NONE = new Segment(Vector3.zero, Vector3.zero);
	    
		// MEMBERS: -------------------------------------------------------------------------------
		
        [SerializeField] private Vector3 m_PointA;
        [SerializeField] private Vector3 m_PointB;
		
		// PROPERTIES: ----------------------------------------------------------------------------

		public Vector3 PointA => this.m_PointA;
		public Vector3 PointB => this.m_PointB;

		// PUBLIC METHODS: ------------------------------------------------------------------------
		
		public Vector3 Get(float t)
		{
			return Get(this, t);
		}
		
		public float LargeDistance(Segment other)
		{
			float distanceA = Vector3.Distance(this.PointA, other.PointA);
			float distanceB = Vector3.Distance(this.PointB, other.PointB);
            
			return distanceA > distanceB ? distanceA : distanceB;
		}
        
		public float SmallDistance(Segment other)
		{
			float distanceA = Vector3.Distance(this.PointA, other.PointA);
			float distanceB = Vector3.Distance(this.PointB, other.PointB);
            
			return distanceA < distanceB ? distanceA : distanceB;
		}

		public Segment Lerp(Segment other, float t)
		{
			return new Segment(
				Vector3.Lerp(this.PointA, other.PointA, t),
				Vector3.Lerp(this.PointB, other.PointB, t)
			);
		}
		
		// PUBLIC STATIC METHODS: -----------------------------------------------------------------

		public static Vector3 Get(Vector3 pointA, Vector3 pointB, float t)
		{
			return Get(new Segment(pointA, pointB), t);
		}
		
		public static Vector3 Get(Segment segment, float t)
		{
			return Vector3.Lerp(segment.m_PointA, segment.m_PointB, t);
		}

		// CONSTRUCTOR: ---------------------------------------------------------------------------
		
		public Segment(Vector3 pointA, Vector3 pointB)
		{
			this.m_PointA = pointA;
			this.m_PointB = pointB;
		}

		// GIZMOS: --------------------------------------------------------------------------------

		public void DrawGizmos(Transform transform)
		{
			Gizmos.DrawLine(
				transform.TransformPoint(this.m_PointA), 
				transform.TransformPoint(this.m_PointB)
			);
		}
    }
}