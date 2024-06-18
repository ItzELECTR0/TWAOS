using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class TIcon : IIcon
    {
        private const TextureFormat FORMAT = TextureFormat.RGBA32;

        private const int WIDTH = 64;
        private const int HEIGHT = 64;
        
        [field: NonSerialized]
        private static readonly Dictionary<int, Texture2D> Cache = new Dictionary<int, Texture2D>();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Color m_Tint;
        [NonSerialized] private readonly IIcon m_Overlay;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected virtual ColorTheme.Type OverlayColor => ColorTheme.Type.Blue;

        protected virtual byte[] Bytes => null;

        public override int GetHashCode()
        {
            int hashKey = this.GetType().GetHashCode() ^ this.m_Tint.GetHashCode();
            if (this.m_Overlay != null)
            {
                hashKey ^= this.m_Overlay.GetHashCode() ^ this.OverlayColor.GetHashCode();
            }

            return hashKey;
        }

        public Texture2D Texture
        {
            get
            {
                int hashKey = this.GetHashCode();

                if (Cache.TryGetValue(hashKey, out Texture2D texture) && texture != null)
                {
                    return texture;
                }
                
                Texture2D overlayTexture = this.m_Overlay?.Texture;
                Color overlayColor = ColorTheme.Get(this.OverlayColor);
                    
                texture = new Texture2D(WIDTH, HEIGHT, FORMAT, false);
                texture.LoadRawTextureData(this.Bytes);

                for (int i = 0; i < WIDTH; ++i)
                {
                    for (int j = 0; j < HEIGHT; ++j)
                    {
                        Color pixel = texture.GetPixel(i, j);
                        Color color = new Color(
                            pixel.r * this.m_Tint.r,
                            pixel.g * this.m_Tint.g,
                            pixel.b * this.m_Tint.b,
                            pixel.a
                        );

                        if (overlayTexture != null)
                        {
                            Color watermark = overlayTexture.GetPixel(i, j);
                            color = Color.Lerp(color, overlayColor, 1f - watermark.r);
                            color.a *= 1f - watermark.g;
                        }
                            
                        texture.SetPixel(i, j, color);
                    }
                }

                texture.Apply();
                Cache[hashKey] = texture;

                return texture;
            }
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected TIcon(Color color, IIcon overlay)
        {
            this.m_Tint = color;
            this.m_Overlay = overlay;
        }
    }
}
