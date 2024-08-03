using UnityEngine;
using UnityEngine.Video;

namespace ELECTRIS
{

    public class AutoPlayVideo : MonoBehaviour
    {
        // Reference to the VideoPlayer component
        private VideoPlayer videoPlayer;

        // Flag to track if the video has finished playing
        private bool playedVideo = false;

        private void Awake()
        {
            // Get the VideoPlayer component attached to the same GameObject
            videoPlayer = GetComponent<VideoPlayer>();

            // Subscribe to the videoPlayer's loopPointReached event
            videoPlayer.loopPointReached += OnVideoFinished;

            // Start playing the video
            videoPlayer.Play();
        }

        // Event handler for when the video finishes playing
        private void OnVideoFinished(VideoPlayer vp)
        {
            // Set the flag to true
            playedVideo = true;
            HasPlayedVideo = playedVideo;

            // Disable the VideoPlayer component
            videoPlayer.enabled = false;
        }

        // Optional method to check if the video has finished playing
        public bool HasPlayedVideo;
}

}
