using System.Collections;
using System.Collections.Generic;
using ELECTRIS;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;

namespace ELECTRIS
{
    public class PauseManager : MonoBehaviour
    {
        [Header("Script Control")]
        [SerializeField] private bool reInput;

        [Header("Legacy Keybinds")]
        KeyCode pauseKey = KeyCode.Escape;

        [Header("Player Variables")]
        private Player player;
        [SerializeField] private int playerId = 1;

        [Header("Pause System Variables")]
        public static bool isPaused = false;
        public static bool isCinematic = false;
        public static bool isPlaying = true;
        public static bool isCombating = false;
        [SerializeField] private PausedGame paused;


        void Start()
        {
            player = ReInput.players.GetPlayer(playerId);
        }

        private void RewiredInput()
        {
            if (player.GetButtonDown("Pause"))
            {
                GameState currentGameState = GameStateManager.Instance.CurrentGameState;
                if (GameStateManager.Instance.CurrentGameState != GameState.Paused)
                {
                    paused.lastGameState = currentGameState;
                }
                GameState newGameState = currentGameState == GameState.Gameplay
                    ? GameState.Paused
                    : GameState.Gameplay;

                GameStateManager.Instance.SetState(newGameState);
                paused.Pause();
            }
        }

        private void DefaultInput()
        {
            if (Input.GetKey(pauseKey))
            {
                GameState currentGameState = GameStateManager.Instance.CurrentGameState;
                if (GameStateManager.Instance.CurrentGameState != GameState.Paused)
                {
                    paused.lastGameState = currentGameState;
                }
                GameState newGameState = currentGameState == GameState.Gameplay
                    ? GameState.Paused
                    : GameState.Gameplay;

                GameStateManager.Instance.SetState(newGameState);
                paused.Pause();
            }
        }

        void Update()
        {
            if (reInput)
            {
                RewiredInput();
            }
            else if (!reInput)
            {
                DefaultInput();
            }

            if (GameStateManager.Instance.CurrentGameState == GameState.Paused)
            {
                isPlaying = false;
                isCombating = false;
                isCinematic = false;
                isPaused = true;
            }
            else if (GameStateManager.Instance.CurrentGameState == GameState.Cinematic)
            {
                isPlaying = false;
                isCombating = false;
                isPaused = false;
                isCinematic = true;
            }else if (GameStateManager.Instance.CurrentGameState == GameState.Gameplay)
            {
                isCombating = false;
                isPaused = false;
                isCinematic = false;
                isPlaying = true;
            }
            else if (GameStateManager.Instance.CurrentGameState == GameState.Combat)
            {
                isPlaying = true;
                isPaused = false;
                isCinematic = false;
                isCombating = true;
            }
        }
    }
}
