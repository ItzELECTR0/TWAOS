using System.Collections;
using UnityEngine;
using ELECTRIS;
using UnityEngine.SceneManagement;

namespace ELECTRIS
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject LoadingScreen;

        public void LoadLevel(int sceneIndex)
        {
            LoadingScreen.SetActive(true);
            StartCoroutine(LoadAsync(sceneIndex));
        }

        IEnumerator LoadAsync(int sceneIndex)
        {

            AsyncOperation load = SceneManager.LoadSceneAsync(sceneIndex);

            while (!load.isDone)
            {
                float progress = Mathf.Clamp01(load.progress / .9f);
                Debug.Log(progress);

                yield return null;
            }
        }
    }
}