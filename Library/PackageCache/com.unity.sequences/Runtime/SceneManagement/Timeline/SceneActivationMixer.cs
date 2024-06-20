using UnityEngine.Playables;

namespace UnityEngine.Sequences.Timeline
{
    class SceneActivationMixer : PlayableBehaviour
    {
        SceneReference m_Scene;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (string.IsNullOrEmpty(m_Scene.path))
                return;

            int inputsCount = playable.GetInputCount();

            float totalWeight = 0;
            for (int i = 0; i < inputsCount; ++i)
            {
                totalWeight += playable.GetInputWeight(i);
            }

            if (totalWeight > 0f)
                SceneActivationManager.RequestActivateScene(m_Scene.path);
            else
                SceneActivationManager.RequestDeactivateScene(m_Scene.path);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (!string.IsNullOrEmpty(m_Scene.path))
                SceneActivationManager.Unregister(m_Scene.path);
        }

        public void SetData(SceneReference scene)
        {
            m_Scene = scene;
            if (!string.IsNullOrEmpty(m_Scene.path))
                SceneActivationManager.Register(m_Scene.path);
        }
    }
}
