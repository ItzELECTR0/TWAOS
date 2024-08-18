using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    internal static class WelcomeInternalTextures
    {
        private class Data
        {
            public readonly string path;
            public Texture2D texture;

            private Data()
            {
                this.path = string.Empty;
                this.texture = null;
            }
            
            public Data(string path) : this()
            {
                this.path = path;
            }
        }
        
        // CONSTANTS: -----------------------------------------------------------------------------
        
        private const string PATH_TEXTURES = EditorPaths.COMMON + "Settings/Data/";
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        private static readonly Dictionary<string, Data> _Textures;

        // STATIC CONSTRUCTOR: --------------------------------------------------------------------

        static WelcomeInternalTextures()
        {
            _Textures = new Dictionary<string, Data>
            {
                {
                    "https://gamecreator.io/assets/api/welcome/images/welcome.png",
                    new Data(PATH_TEXTURES + "Welcome.txt")
                },
                {
                    "https://gamecreator.io/assets/api/welcome/images/videos.png",
                    new Data(PATH_TEXTURES + "Videos.txt")
                },
                {
                    "https://gamecreator.io/assets/api/welcome/images/documentation.png",
                    new Data(PATH_TEXTURES + "Documentation.txt")
                },
                {
                    "https://gamecreator.io/assets/api/welcome/images/examples.png",
                    new Data(PATH_TEXTURES + "Examples.txt")
                },
                {
                    "https://gamecreator.io/assets/api/welcome/images/hub.png",
                    new Data(PATH_TEXTURES + "Hub.txt")
                },
                {
                    "https://gamecreator.io/assets/api/welcome/images/discord.png",
                    new Data(PATH_TEXTURES + "Discord.txt")
                }
            };
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static bool Exists(string path) => _Textures.ContainsKey(path);
        
        public static Texture2D Get(string path)
        {
            if (!_Textures.TryGetValue(path, out Data data)) return null;
            
            if (data.texture == null)
            {
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(data.path);
                data.texture = new Texture2D(1, 1);
                data.texture.LoadImage(Convert.FromBase64String(textAsset.text));
            }

            return data.texture;

        }
    }
}