using UnityEngine.Playables;

namespace UnityEngine.Sequences.Timeline
{
    // Note: The documentation XML are added only to remove warning when validating the package until this class
    //       can be made private. In the meantime, it is explicitly excluded from the documentation, see
    //       Documentation > filter.yml

    /// <summary>
    ///
    /// </summary>
    class StoryboardMixerBehaviour : PlayableBehaviour
    {
        Canvas m_Canvas;

        /// <summary>
        ///
        /// </summary>
        public Canvas canvas => m_Canvas;

        /// <inheritdoc cref="PlayableBehaviour.OnPlayableCreate"/>
        public override void OnPlayableCreate(Playable playable)
        {
            var canvasGo = new GameObject("Storyboard");
            canvasGo.hideFlags = HideFlags.HideAndDontSave;
            m_Canvas = canvasGo.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        /// <inheritdoc cref="PlayableBehaviour.OnPlayableDestroy"/>
        public override void OnPlayableDestroy(Playable playable)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(m_Canvas.gameObject);
#else
            Object.Destroy(m_Canvas.gameObject);
#endif
        }
    }
}
