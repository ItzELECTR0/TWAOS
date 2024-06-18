using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common.SaveSystem
{
    public class Scenes : IGameSave
    {
        [Serializable]
        public class Token
        {
            // MEMBERS: ---------------------------------------------------------------------------
            
            [SerializeField] private string[] m_Names;

            // PROPERTIES: ------------------------------------------------------------------------

            public int Count => this.m_Names.Length;
            public string[] Names => this.m_Names;

            // CONSTRUCTOR: -----------------------------------------------------------------------

            private Token()
            {
                int scenesLength = SceneManager.sceneCount;
                this.m_Names = new string[scenesLength];

                for (int i = 0; i < scenesLength; ++i)
                {
                    this.m_Names[i] = SceneManager.GetSceneAt(i).name;
                }
            }

            public static Token Create() => new Token();
        }
        
        // CONSTANTS: -----------------------------------------------------------------------------
        
        public const string ID = "scenes";
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private float[] m_ScenesProgress = Array.Empty<float>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Progress
        {
            get
            {
                if (this.m_ScenesProgress.Length == 0) return 1f;
                
                float amount = 0f;
                foreach (float value in this.m_ScenesProgress) amount += value;
                
                return amount / this.m_ScenesProgress.Length;
            }
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string SaveID => ID;
        public bool IsShared => false;
        
        public Type SaveType => typeof(Token);
        
        public object GetSaveData(bool includeNonSavable)
        {
            return Token.Create();
        }

        public LoadMode LoadMode => LoadMode.Greedy;

        public async Task OnLoad(object value)
        {
            if (value is not Token token)
            {
                throw new Exception("Cannot convert 'token' to 'Scenes.Token'");
            }

            if (token.Count == 0) return;

            string[] scenes = GeneralRepository.Get.Save.Load switch
            {
                LoadSceneMode.AllSavedScenes => token.Names,
                LoadSceneMode.MainSavedScene => new []{ token.Names[0] },
                LoadSceneMode.Scene => new []{ GeneralRepository.Get.Save.GetSceneName(Args.EMPTY) },
                _ => throw new ArgumentOutOfRangeException()
            };
            
            this.m_ScenesProgress = new float[scenes.Length];
            
            await this.LoadScene(
                0, scenes,
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );

            for (int i = 1; i < scenes.Length; ++i)
            {
                await this.LoadScene(
                    i, scenes,
                    UnityEngine.SceneManagement.LoadSceneMode.Additive
                );
            }
            
            await Task.Yield();
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static async Task LoadScene(int index)
        {
            SceneManager.LoadScene(index);
            await Task.Yield();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private async Task LoadScene(
            int index, 
            IReadOnlyList<string> names, 
            UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(names[index], mode);
            while (!AsyncManager.ExitRequest && !async.isDone)
            {
                this.m_ScenesProgress[index] = async.progress;
                await Task.Yield();
            }
        }
    }
}