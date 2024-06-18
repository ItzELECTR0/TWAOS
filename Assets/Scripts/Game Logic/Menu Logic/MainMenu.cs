using ELECTRIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELECTRIS
{
    public class MainMenu : MonoBehaviour
    {
        public LevelLoader loader;

        public void PlayGame()
        {
            loader.LoadLevel(1);
        }

        public void QuiteGame()
        {
            Debug.Log("Quitting Game!");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}