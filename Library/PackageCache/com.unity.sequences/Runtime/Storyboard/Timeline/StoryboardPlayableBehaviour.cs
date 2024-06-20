using System;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace UnityEngine.Sequences.Timeline
{
    // Note: The documentation XML are added only to remove warning when validating the package until this class
    //       can be made private. In the meantime, it is explicitly excluded from the documentation, see
    //       Documentation > filter.yml

    /// <summary>
    /// Contains the logic for displaying and scaling the image to the screen size.
    /// </summary>
    class StoryboardPlayableBehaviour : PlayableBehaviour
    {
        Texture m_Board = null;
        /// <summary>
        ///
        /// </summary>
        public Texture board
        {
            set => m_Board = value;
        }

        bool m_ShowBoard = true;
        /// <summary>
        ///
        /// </summary>
        public bool showBoard
        {
            set => m_ShowBoard = value;
        }

        Vector2 m_Position = Vector2.zero;
        /// <summary>
        ///
        /// </summary>
        public Vector2 position
        {
            set => m_Position = value;
        }

        float m_Alpha = 1;
        /// <summary>
        ///
        /// </summary>
        public float alpha
        {
            set => m_Alpha = value;
        }

        Vector3 m_Rotation = Vector3.zero;
        /// <summary>
        ///
        /// </summary>
        public Vector3 rotation
        {
            set => m_Rotation = value;
        }

        Vector2 m_Scale = Vector2.one;
        /// <summary>
        ///
        /// </summary>
        public Vector2 scale
        {
            set => m_Scale = value;
        }

        Canvas m_Canvas;

        /// <inheritdoc cref="PlayableBehaviour.ProcessFrame"/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ScriptPlayable<StoryboardMixerBehaviour> outputPlayable =
                (ScriptPlayable<StoryboardMixerBehaviour>)playable.GetOutput(0);

            var outputPlayableBehaviour = outputPlayable.GetBehaviour();
            m_Canvas = outputPlayableBehaviour.canvas;

            if (m_Canvas == null || m_Board == null || !m_ShowBoard) return;

            if (m_Canvas.transform.childCount == 0)
            {
                // if the board isn't loaded, load it
                var boardGo = new GameObject(m_Board.name);
                boardGo.hideFlags = HideFlags.HideAndDontSave;

                boardGo.transform.parent = m_Canvas.transform;
                boardGo.transform.localPosition = m_Position;
                boardGo.transform.localScale = GetBestFitScale();

                var currentBoard = boardGo.AddComponent<RawImage>();
                currentBoard.texture = m_Board;
                // Using SetNativeSize because otherwise textures are resized to 100x100 by default
                currentBoard.SetNativeSize();

                var color = Color.white;
                color.a = m_Alpha;
                currentBoard.color = color;

                currentBoard.transform.rotation = Quaternion.Euler(m_Rotation);
            }
            else
            {
                // there is a board, rescale it & adjust rotation
                var child = m_Canvas.transform.GetChild(0);
                child.localScale = GetBestFitScale();
                child.rotation = Quaternion.Euler(m_Rotation);
            }
        }

        /// <summary>
        /// Calculates how much a board needs to be scaled in order to fit to the size of the current GameView
        /// Uses BestFit (does not affect original aspect ratio of the image)
        /// </summary>
        /// <returns>Vector2 that indicates how much to scale the orginial image to fit the GameView</returns>
        Vector2 GetBestFitScale()
        {
            // TODO: investigate how to get it to scale with Gameview size and aspect ratio changes
            Rect screen = new Rect((float)Screen.width / 2, (float)Screen.height / 2, Screen.width, Screen.height);

            Vector2 reScale = Vector2.one;
            if (m_Board != null
                && m_Board.width > 0 && m_Board.height > 0
                && screen.width > 0 && screen.height > 0)
            {
                // Fits the image to the size of the GameView without altering the aspect ratio
                float bestFitScale = Math.Min(screen.height / m_Board.height, screen.width / m_Board.width);
                reScale *= bestFitScale;
            }

            reScale.x *= m_Scale.x;
            reScale.y *= m_Scale.y;
            return reScale;
        }

        /// <inheritdoc cref="PlayableBehaviour.OnBehaviourPause"/>
        /// <remarks> When the playhead leaves the clip, remove the board from the canvas</remarks>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_Canvas == null) return;
            if (m_Canvas.transform.childCount <= 0) return;

            var child = m_Canvas.transform.GetChild(0);
            child.SetParent(null);
#if UNITY_EDITOR
            Object.DestroyImmediate(child.gameObject);
#else
            Object.Destroy(child.gameObject);
#endif
        }
    }
}
