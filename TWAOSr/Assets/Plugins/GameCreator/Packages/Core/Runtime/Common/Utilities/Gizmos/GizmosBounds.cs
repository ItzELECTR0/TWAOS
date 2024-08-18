using UnityEngine;

namespace GameCreator.Runtime.Common
{
	public static partial class GizmosExtension
	{
        public static void Bounds(Bounds bounds)
        {
            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            Vector3 rhtTopFwd = bounds.center + new Vector3(x, y, z);
            Vector3 rhtTopBck = bounds.center + new Vector3(x, y, -z);
            Vector3 lftTopFwd = bounds.center + new Vector3(-x, y, z);
            Vector3 lftTopBck = bounds.center + new Vector3(-x, y, -z);

            Vector3 rhtBtmFwd = bounds.center + new Vector3(x, -y, z);
            Vector3 rhtBtmBck = bounds.center + new Vector3(x, -y, -z);
            Vector3 lftBtmFwd = bounds.center + new Vector3(-x, -y, z);
            Vector3 lftBtmBck = bounds.center + new Vector3(-x, -y, -z);

            Gizmos.DrawLine(rhtTopFwd, lftTopFwd);
            Gizmos.DrawLine(rhtTopFwd, rhtTopBck);
            Gizmos.DrawLine(lftTopFwd, lftTopBck);
            Gizmos.DrawLine(rhtTopBck, lftTopBck);
            
            Gizmos.DrawLine(rhtTopFwd, rhtBtmFwd);
            Gizmos.DrawLine(rhtTopBck, rhtBtmBck);
            Gizmos.DrawLine(lftTopFwd, lftBtmFwd);
            Gizmos.DrawLine(lftTopBck, lftBtmBck);
            
            Gizmos.DrawLine(rhtBtmFwd, lftBtmFwd);
            Gizmos.DrawLine(rhtBtmFwd, rhtBtmBck);
            Gizmos.DrawLine(lftBtmFwd, lftBtmBck);
            Gizmos.DrawLine(lftBtmBck, rhtBtmBck);
        }
    }
}