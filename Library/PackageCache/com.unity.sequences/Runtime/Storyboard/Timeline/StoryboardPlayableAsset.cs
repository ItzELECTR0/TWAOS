using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.Sequences.Timeline
{
    /// <summary>
    /// The StoryboardPlayableAsset represents a Storyboard frame to use in the StoryboardTrack.
    /// </summary>
    public class StoryboardPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// Whether to display this specific board image or not.
        /// </summary>
        [Tooltip("If checked, the specified board will be displayed as an overlay over the virtual camera's output")]
        [SerializeField]
        public bool showBoard = true;

        /// <summary>
        /// The board image to display.
        /// </summary>
        [Tooltip("The board to be displayed")]
        [SerializeField]
        public Texture board;

        /// <summary>
        /// The opacity of the board image. By default it is 50% transparent.
        /// </summary>
        [Tooltip("The opacity of the board.  0 is transparent, 1 is opaque")]
        [Range(0, 1)]
        [SerializeField]
        public float alpha = 0.5f;

        /// <summary>
        /// The board image position. By default it is centered.
        /// </summary>
        [Tooltip("The screen-space position at which to display the board. Zero is center")]
        [SerializeField]
        public Vector2 position = Vector2.zero;

        /// <summary>
        /// The board image Z rotation (tilt).
        /// </summary>
        [Tooltip("The z-axis rotation to apply to the board")]
        [SerializeField]
        [Range(-180, 180)]
        public float zRotation = 0;

        /// <summary>
        /// Whether to horizontally flip the board image or not.
        /// </summary>
        // TODO: Move functionality to UI
        [Tooltip("If checked, the board will be flipped horizontally")]
        public bool horizontalFlip = false;

        /// <summary>
        /// Whether to vertically flip the board image or not.
        /// </summary>
        [Tooltip("If checked, the board will be flipped vertically")]
        public bool verticalFlip = false;

        /// <summary>
        /// Whether to lock the aspect ratio of the board image or not. By default, the aspect ratio is locked to
        /// preserve the original image proportions.
        /// </summary>
        [Tooltip("If checked, X and Y scale are synchronized")]
        public bool syncScale = true;

        /// <summary>
        /// The board image scale.
        /// </summary>
        // TODO: Move scale range limit to UI
        [Tooltip("The screen-space scaling to apply to the board")]
        [Min(0)]
        public Vector2 scale = Vector2.one;

        /// <summary>
        /// Get the clip capabilities. For StoryboardPlayableAsset, no extra Clip caps are available.
        /// </summary>
        // TODO: Add blending
        public ClipCaps clipCaps => ClipCaps.None;

        // [doc-replica] Unity doc: UnityEngine.Playables.PlayableAsset.CreatePlayable
        /// <summary>
        /// Implement this method to have your asset inject playables into the given graph.
        /// </summary>
        /// <param name="graph">The graph to inject playables into.</param>
        /// <param name="owner">The game object which initiated the build.</param>
        /// <returns>The playable injected into the graph, or the root playable if multiple playables are injected.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<StoryboardPlayableBehaviour>.Create(graph);

            if (playable.GetBehaviour() != null)
                SetAttributes(playable.GetBehaviour());

            return playable;
        }

        void SetAttributes(StoryboardPlayableBehaviour storyboardBehaviour)
        {
            storyboardBehaviour.showBoard = showBoard;
            storyboardBehaviour.board = board;
            storyboardBehaviour.position = position;
            storyboardBehaviour.alpha = alpha;
            storyboardBehaviour.rotation = new Vector3(0, 0, zRotation);

            // To move to custom UI
            if (syncScale)
                scale.y = scale.x;

            var newScale = scale;

            if (horizontalFlip) newScale.x *= -1;
            if (verticalFlip) newScale.y *= -1;

            storyboardBehaviour.scale = newScale;
        }
    }
}
