using UnityEngine;

namespace GameCreator.Runtime.Common
{
	public static class Vector3Plane
	{
		public static Vector3 NormalUp { get; } = new Vector3(1, 0, 1);
		public static Vector3 NormalRight { get; } = new Vector3(0, 1, 1);
		public static Vector3 NormalForward { get; } = new Vector3(1, 1, 0);
	}
}