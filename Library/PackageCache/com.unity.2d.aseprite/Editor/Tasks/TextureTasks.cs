using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace UnityEditor.U2D.Aseprite
{
#if UNITY_2022_2_OR_NEWER
    [BurstCompile]
#endif
    internal static class TextureTasks
    {
        [BurstCompile]
        public static void AddOpacity(ref NativeArray<Color32> texture, float opacity)
        {
            for (var i = 0; i < texture.Length; ++i)
            {
                var color = texture[i];
                color.a = (byte)(color.a * opacity);
                texture[i] = color;
            }
        }

        [BurstCompile]
        public static void FlipTextureY(ref NativeArray<Color32> texture, int width, int height)
        {
            if (width == 0 || height == 0)
                return;

            var outputTexture = new NativeArray<Color32>(texture.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (var y = 0; y < height; ++y)
            {
                var inRow = ((height - 1) - y) * width;
                var outRow = y * width;

                for (var x = 0; x < width; ++x)
                {
                    var inIndex = x + inRow;
                    var outIndex = x + outRow;
                    outputTexture[outIndex] = texture[inIndex];
                }
            }

            texture.DisposeIfCreated();
            texture = outputTexture;
        }

        public struct MergeOutput
        {
            public RectInt rect;
            public NativeArray<Color32> image;
        }

        [BurstCompile]
        public static unsafe void MergeTextures(in NativeArray<IntPtr> textures, in NativeArray<RectInt> textureSizes, in NativeArray<BlendModes> blendModes, out MergeOutput output)
        {
            GetCombinedRect(in textureSizes, out var combinedRect);
            var outputTexture = new NativeArray<Color32>(combinedRect.width * combinedRect.height, Allocator.Persistent);

            var outStartX = combinedRect.x;
            var outStartY = combinedRect.y;
            var outWidth = combinedRect.width;
            var outHeight = combinedRect.height;
            for (var i = 0; i < textures.Length; ++i)
            {
                var inputColor = (Color32*)textures[i];
                var inputBlend = blendModes[i];
                var inX = textureSizes[i].x;
                var inY = textureSizes[i].y;
                var inWidth = textureSizes[i].width;
                var inHeight = textureSizes[i].height;

                for (var y = 0; y < inHeight; ++y)
                {
                    var outPosY = (y + inY) - outStartY;
                    // If pixel is outside of output texture's Y, move to the next pixel.
                    if (outPosY < 0 || outPosY >= outHeight)
                        continue;

                    // Flip Y position on the input texture, because
                    // Aseprite textures are stored "upside-down"
                    var inRow = ((inHeight - 1) - y) * inWidth;
                    var outRow = outPosY * outWidth;

                    for (var x = 0; x < inWidth; ++x)
                    {
                        var outPosX = (x + inX) - outStartX;
                        // If pixel is outside of output texture's X, move to the next pixel.
                        if (outPosX < 0 || outPosX >= outWidth)
                            continue;

                        var inBufferIndex = inRow + x;
                        var outBufferIndex = outRow + outPosX;
                        if (outBufferIndex < 0 || outBufferIndex > (outWidth * outHeight))
                            continue;

                        var inColor = inputColor[inBufferIndex];
                        var prevOutColor = outputTexture[outBufferIndex];

                        Color32 outColor;
                        switch (inputBlend)
                        {
                            case BlendModes.Darken:
                                PixelBlends.Darken(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Multiply:
                                PixelBlends.Multiply(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.ColorBurn:
                                PixelBlends.ColorBurn(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Lighten:
                                PixelBlends.Lighten(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Screen:
                                PixelBlends.Screen(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.ColorDodge:
                                PixelBlends.ColorDodge(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Addition:
                                PixelBlends.Addition(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Overlay:
                                PixelBlends.Overlay(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.SoftLight:
                                PixelBlends.SoftLight(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.HardLight:
                                PixelBlends.HardLight(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Difference:
                                PixelBlends.Difference(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Exclusion:
                                PixelBlends.Exclusion(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Subtract:
                                PixelBlends.Subtract(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Divide:
                                PixelBlends.Divide(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Hue:
                                PixelBlends.Hue(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Saturation:
                                PixelBlends.Saturation(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Color:
                                PixelBlends.ColorBlend(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Luminosity:
                                PixelBlends.Luminosity(in prevOutColor, in inColor, out outColor);
                                break;
                            case BlendModes.Normal:
                            default:
                                PixelBlends.Normal(in prevOutColor, in inColor, out outColor);
                                break;
                        }

                        outputTexture[outBufferIndex] = outColor;
                    }
                }
            }

            output = new MergeOutput()
            {
                rect = combinedRect,
                image = outputTexture
            };
        }

        [BurstCompile]
        static void GetCombinedRect(in NativeArray<RectInt> rects, out RectInt combinedRect)
        {
            combinedRect = rects[0];
            for (var i = 1; i < rects.Length; ++i)
            {
                var rectToFitIn = rects[i];
                FitRectInsideRect(ref combinedRect, in rectToFitIn);
            }
        }

        [BurstCompile]
        static void FitRectInsideRect(ref RectInt baseRect, in RectInt rectToFitIn)
        {
            if (baseRect.xMin > rectToFitIn.xMin)
                baseRect.xMin = rectToFitIn.xMin;
            if (baseRect.yMin > rectToFitIn.yMin)
                baseRect.yMin = rectToFitIn.yMin;
            if (baseRect.xMax < rectToFitIn.xMax)
                baseRect.xMax = rectToFitIn.xMax;
            if (baseRect.yMax < rectToFitIn.yMax)
                baseRect.yMax = rectToFitIn.yMax;
        }
    }
}
