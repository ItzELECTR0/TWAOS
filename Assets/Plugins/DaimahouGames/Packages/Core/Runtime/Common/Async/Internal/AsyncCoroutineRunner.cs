using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    public class AsyncCoroutineRunner : MonoBehaviour
    {
        private static AsyncCoroutineRunner _instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("AsyncCoroutineRunner").AddComponent<AsyncCoroutineRunner>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            var go = gameObject;
            go.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(go);
        }
    }
}
