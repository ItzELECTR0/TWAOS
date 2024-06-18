using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace ELECTRIS
{
    public class PausedGame : MonoBehaviour
    {
        [SerializeField] private float timeRate = 0.5f;
        public GameObject pauseScreen;
        public GameState lastGameState;

        public void Pause()
        {
            if (GameStateManager.Instance.CurrentGameState == GameState.Paused)
            {
                StartCoroutine(SlowTimeToZero());
            }
        }

        public void UnPause()
        {
            if (GameStateManager.Instance.CurrentGameState == GameState.Paused)
            {
                GameStateManager.Instance.SetState(lastGameState);

                if (GameStateManager.Instance.CurrentGameState != GameState.Paused)
                {
                    StartCoroutine(SpeedTimeToOne());
                }
            }
        }

        public IEnumerator SlowTimeToZero()
        {
            while (Time.timeScale > 0)
            {
                Time.timeScale -= timeRate * Time.unscaledDeltaTime;
                yield return null;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseScreen.SetActive(true);
        }

        public IEnumerator SpeedTimeToOne()
        {
            while (Time.timeScale < 1)
            {
                Time.timeScale += timeRate * Time.unscaledDeltaTime;
                yield return null;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseScreen.SetActive(false);
        }
    }
}
