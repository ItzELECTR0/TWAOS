using UnityEngine;

namespace ELECTRIS
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField] private AutoPlayVideo startVideo;
        [SerializeField] private LevelLoader levelLoader;
        
        void Update()
        {
            if (startVideo.HasPlayedVideo)
            {
                levelLoader.LoadLevel(1);
            }
        }
    }
}