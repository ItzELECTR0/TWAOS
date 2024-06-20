using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityEngine.Sequences
{
    /// <summary>
    /// This class implements a basic Scene activation behaviour that activates or deactivated a Scene's root
    /// GameObjects.
    /// </summary>
    sealed class BasicSceneActivation : ISceneActivationBehaviour
    {
        /// <summary>
        /// A buffer used to get the root GameObjects from Scene objects.
        /// This helps you avoid some allocation.
        /// </summary>
        List<GameObject> s_DynamicBuffer = new List<GameObject>();

        public void Execute(Scene scene, bool newState)
        {
            s_DynamicBuffer.Clear();
            scene.GetRootGameObjects(s_DynamicBuffer);

            foreach (GameObject root in s_DynamicBuffer)
                root.SetActive(newState);
        }
    }
}
