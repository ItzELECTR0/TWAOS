using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELECTRIS
{
    public class GameStateManager
    {
        private static GameStateManager _instance;

        public static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameStateManager();
                }

                return _instance;
            }
        }

        public GameState CurrentGameState { get; private set; }
        public delegate void GameStateChageHandler(GameState newGameState);
        public event GameStateChageHandler OnGameStateChanged;

        private GameStateManager()
        {

        }

        public void SetState(GameState newGameState)
        {
            if (newGameState == CurrentGameState)
            {
                return;
            }

            CurrentGameState = newGameState;
            OnGameStateChanged?.Invoke(newGameState);
        }
    }
}
