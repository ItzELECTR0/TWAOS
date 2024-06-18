using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class GameObjectUtils
    {
        /// <summary>
        /// Returns the bounding box of the game object taking into account all its children's
        /// renderers.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Bounds GetBoundingBox(this GameObject gameObject)
        {
            Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);

            return bounds;
        }
    }
}
