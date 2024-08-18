using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GameCreator.Editor.Common
{
    internal static class WelcomeManager
    {
        private static readonly WelcomeData DEFAULT_PAGES;

        private static readonly CultureInfo CULTURE = CultureInfo.InvariantCulture;

        private const string DOMAIN = "https://gamecreator.io/";
        private const string URI = DOMAIN + "assets/api/welcome/welcome.json";
        private const int CHECK_FREQUENCY = 6;
        
        private const string KEY_CHECK_DATE = "gc:welcome-check-date";
        private const string KEY_EDITOR_OPEN = "gc:welcome-editor-open";
        private const string KEY_WELCOME_DATA = "gc:welcome-data";
        private const string KEY_WELCOME_TEXTURES = "gc:welcome-textures";

        private static UnityWebRequest REQUEST;

        // PROPERTIES: ----------------------------------------------------------------------------

        public static WelcomeData Data
        {
            get
            {
                string welcomeDataJson = EditorPrefs.GetString(KEY_WELCOME_DATA, string.Empty);
                WelcomeData welcomeData = new WelcomeData();
            
                if (!string.IsNullOrEmpty(welcomeDataJson))
                {
                    EditorJsonUtility.FromJsonOverwrite(welcomeDataJson, welcomeData);
                }

                if (welcomeData.pages.Length == 0)
                {
                    welcomeData = new WelcomeData(DEFAULT_PAGES);
                }

                return welcomeData;
            }
        }

        public static WelcomeTextures Textures
        {
            get
            {
                string json = EditorPrefs.GetString(KEY_WELCOME_TEXTURES, string.Empty);
                WelcomeTextures welcomeTextures = new WelcomeTextures();
                if (string.IsNullOrEmpty(json)) return welcomeTextures;
                EditorJsonUtility.FromJsonOverwrite(json, welcomeTextures);
                return welcomeTextures;
            }
            private set
            {
                string json = EditorJsonUtility.ToJson(value, false);
                EditorPrefs.SetString(KEY_WELCOME_TEXTURES, json);
            }
        }

        // INIT METHODS: --------------------------------------------------------------------------

        static WelcomeManager()
        {
            DEFAULT_PAGES = new WelcomeData(
                new WelcomePage(
                    "https://gamecreator.io/assets/api/welcome/images/welcome.png", 
                    "#22232d"
                ),
                new WelcomePage(
                    "https://gamecreator.io/assets/api/welcome/images/videos.png", 
                    "#e03131", 
                    "link", 
                    "https://www.youtube.com/channel/UCDFnEqD5mHGwiHK22olcDaw"
                ),
                new WelcomePage(
                    "https://gamecreator.io/assets/api/welcome/images/documentation.png", 
                    "#ffd8a8", 
                    "link", 
                    "https://docs.gamecreator.io"
                ),
                new WelcomePage(
                    "https://gamecreator.io/assets/api/welcome/images/hub.png", 
                    "#2196f3", 
                    "link", 
                    "https://gamecreator.io/hub"
                ),
                new WelcomePage(
                    "https://gamecreator.io/assets/api/welcome/images/examples.png", 
                    "#d0bfff", 
                    "examples-manager"
                ),
                new WelcomePage(
                    "https://gamecreator.io/assets/api/welcome/images/discord.png", 
                    "#5865f2", 
                    "link", 
                    "https://gamecreator.link/discord"
                )
            );
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            InitializeDataFetch();
            SettingsWindow.InitRunners.Add(new InitRunner(
                SettingsWindow.INIT_PRIORITY_HIGH,
                CanInitializeWelcomeScreen,
                InitializeWelcomeScreen
            ));
        }

        private static void InitializeDataFetch()
        {
            string minDate = DateTime.MinValue.ToString(CULTURE);
            
            DateTime currentDate = DateTime.Now;
            DateTime checkDate = DateTime.Parse(
                EditorPrefs.GetString(KEY_CHECK_DATE, minDate), 
                CULTURE
            );

            TimeSpan timeDifference = currentDate - checkDate;
            if (timeDifference.TotalHours < CHECK_FREQUENCY) return;
            
            EditorPrefs.SetString(KEY_CHECK_DATE, currentDate.ToString(CULTURE));
            EditorApplication.delayCall += UpdateWelcomeData;
        }

        private static bool CanInitializeWelcomeScreen()
        {
            return !SessionState.GetBool(KEY_EDITOR_OPEN, false) && 
                   Settings.From<WelcomeRepository>().OpenOnStartup;
        }
        
        private static void InitializeWelcomeScreen()
        {
            SessionState.SetBool(KEY_EDITOR_OPEN, true);
            SettingsWindow.OpenWindow(WelcomeRepository.REPOSITORY_ID);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static string HashFromPath(string path)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(path);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte hash in hashBytes)
            {
                sb.Append(hash.ToString("X2"));
            }
            
            return sb.ToString();
        }

        // WELCOME DATA: --------------------------------------------------------------------------
        
        private static void UpdateWelcomeData()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            
            REQUEST = UnityWebRequest.Get(URI);
            REQUEST.SetRequestHeader("ContentType", "application/json");

            UnityWebRequestAsyncOperation operation = REQUEST.SendWebRequest();
            operation.completed += OnDataReceive;
        }

        private static void OnDataReceive(AsyncOperation asyncOperation)
        {
            if (!REQUEST.isDone) return;
            if (REQUEST.responseCode == 200)
            {
                WelcomeData welcomeData = new WelcomeData();
                EditorJsonUtility.FromJsonOverwrite(REQUEST.downloadHandler.text, welcomeData);

                string welcomeDataJson = EditorJsonUtility.ToJson(welcomeData, false);
                EditorPrefs.SetString(KEY_WELCOME_DATA, welcomeDataJson);
                
                WelcomeTextures welcomeTextures = Textures;

                List<string> currentTextures = new List<string>(welcomeTextures.Keys);
                List<string> updateTextures = new List<string>();
                
                foreach (WelcomePage welcomePage in welcomeData.pages)
                {
                    if (WelcomeInternalTextures.Exists(welcomePage.image)) continue;
                    
                    string imageID = HashFromPath(welcomePage.image);
                    if (currentTextures.Contains(imageID))
                    {
                        currentTextures.Remove(imageID);
                    }
                    else
                    {
                        updateTextures.Add(welcomePage.image);
                    }
                }
                
                foreach (string imageID in currentTextures) welcomeTextures.Remove(imageID);
                Textures = welcomeTextures;
                
                foreach (string image in updateTextures) UpdateTexture(image);
            }
            else 
            {
                Debug.LogWarning(REQUEST.error);
            }
        }

        private static void UpdateTexture(string path)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            operation.completed += _ =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                    return;
                }

                if (request.downloadHandler is not DownloadHandlerTexture handle)
                {
                    Debug.LogError("Unknown Welcome texture handle");
                    return;
                }

                WelcomeTextures textures = Textures;

                string imageID = HashFromPath(path);
                textures[imageID] = Convert.ToBase64String(handle.data);
                Textures = textures;
            };
        }
    }
}