using UnityEngine.SceneManagement;

namespace UnityEngine.Sequences
{
    /// <summary>
    /// Interface to implement for new scene activation behaviour.
    /// </summary>
    internal interface ISceneActivationBehaviour
    {
        void Execute(Scene scene, bool newState);
    }
}
