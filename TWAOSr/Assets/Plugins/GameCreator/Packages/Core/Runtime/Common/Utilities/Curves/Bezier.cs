using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
	[Serializable]
	public struct Bezier
	{
		private const float GIZMOS_RESOLUTION = 0.02f;
		
		// MEMBERS: -------------------------------------------------------------------------------
		
		[SerializeField] private Vector3 m_PointA;
		[SerializeField] private Vector3 m_ControlA;
		
		[SerializeField] private Vector3 m_PointB;
		[SerializeField] private Vector3 m_ControlB;
		
		// PROPERTIES: ----------------------------------------------------------------------------

		public Vector3 PointA => this.m_PointA;
		public Vector3 PointB => this.m_PointB;

		public Vector3 ControlA => this.m_ControlA;
		public Vector3 ControlB => this.m_ControlB;

		// PUBLIC METHODS: ------------------------------------------------------------------------
		
		public Vector3 Get(float t)
		{
			return Get(this, t);
		}
		
		// PUBLIC STATIC METHODS: -----------------------------------------------------------------

		public static Vector3 Get(Vector3 pA, Vector3 pB, Vector3 cA, Vector3 cB, float t)
		{
			return Get(new Bezier(pA, pB, cA, cB), t);
		}
		
		public static Vector3 Get(Bezier bezier, float t)
		{
			t = Mathf.Clamp01(t);
			
			float t2 = t * t;
			float t3 = t * t2;
			float u = 1.0f - t;
			float u2 = u * u;
			float u3 = u * u2;

			Vector3 point = u3 * bezier.m_PointA;
			point += 3.0f * u2 * t * (bezier.PointA + bezier.m_ControlA);
			point += 3.0f * u * t2 * (bezier.PointB + bezier.m_ControlB);
			point += t3 * bezier.m_PointB;
			
			return point;
		}

		// CONSTRUCTOR: ---------------------------------------------------------------------------
		
		public Bezier(Vector3 pointA, Vector3 pointB, Vector3 controlA, Vector3 controlB)
		{
			this.m_PointA = pointA;
			this.m_PointB = pointB;
			
			this.m_ControlA = controlA;
			this.m_ControlB = controlB;
		}
		
		// GIZMOS: --------------------------------------------------------------------------------

		public void DrawGizmos(Transform transform)
		{
			Vector3 prevPosition = transform.TransformPoint(this.m_PointA);
			int partitions = Mathf.FloorToInt(1f / GIZMOS_RESOLUTION);

			for (int i = 1; i <= partitions; i++)
			{
				float t = i * GIZMOS_RESOLUTION;

				Vector3 currPosition = transform.TransformPoint(this.Get(t));
				Gizmos.DrawLine(prevPosition, currPosition);

				prevPosition = currPosition;
			}

			Color prevColor = Gizmos.color;
			Gizmos.color = new Color(1, 0f, 0f, prevColor.a);

			Gizmos.DrawLine(
				transform.TransformPoint(this.m_PointA), 
				transform.TransformPoint(this.m_PointA + this.m_ControlA)
			);
            
			Gizmos.DrawLine(
				transform.TransformPoint(this.m_PointB), 
				transform.TransformPoint(this.m_PointB + this.m_ControlB)
			);

			Gizmos.color = prevColor;
		}
	}
}