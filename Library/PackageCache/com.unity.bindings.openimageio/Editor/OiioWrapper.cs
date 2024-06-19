using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

namespace UnityEditor.Bindings.OpenImageIO
{
    /// <summary>
    /// Class containing bindings for IO operations using OpenImageIO library
    /// </summary>
    public class OiioWrapper
    {
        #if UNITY_EDITOR_WIN
        const string LibraryPath = "OIIOBindings";
        #elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        const string LibraryPath = "libOIIOBindings";
        #else
        const string LibraryPath = "Undefined";
        #endif

        /// <summary>
        /// Writes an image to the specified file name.
        /// </summary>
        /// <param name="filePath">Path of the file to write.</param>
        /// <param name="nImages">Number of sub-images in this file.</param>
        /// <param name="headers">Pointer to ImageHeaders for the file to write.</param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        public static extern unsafe int WriteImage(FixedString4096Bytes filePath,
            uint nImages,
            ImageHeader* headers);

        /// <summary>
        /// Reads an image from its file name.
        /// </summary>
        /// <param name="filePath">Path of the file to read.</param>
        /// <returns> A SubImagesList with the information of the file.</returns>
        [DllImport(LibraryPath)]
        public static extern unsafe SubImagesList ReadImage(FixedString4096Bytes filePath);

        /// <summary>
        /// Frees the memory allocated for the SubImagesList passed.
        /// </summary>
        /// <param name="list">SubImagesList representing information to clear.</param>
        /// <returns>Zero (0) if successful.</returns>
        [DllImport(LibraryPath)]
        public static extern unsafe int FreeAllocatedMemory(SubImagesList list);

        /// <summary>
        /// A struct (key value pair) used to encode metadata in an image.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Attribute
        {
            /// <summary>
            /// Attribute key (its unique name as expected by OIIO)
            /// </summary>
            public FixedString4096Bytes key;

            /// <summary>
            /// Attribute value
            /// </summary>
            public FixedString4096Bytes value;
        }

        /// <summary>
        /// A struct representing an Image Header
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ImageHeader
        {
            /// <summary>
            /// Pointer to image data structure
            /// </summary>
            public IntPtr data;

            /// <summary>
            /// Number of channels in the image
            /// </summary>
            public uint channelsCount;

            /// <summary>
            /// Image width
            /// </summary>
            public uint width;

            /// <summary>
            /// Image height
            /// </summary>
            public uint height;

            /// <summary>
            /// Number of attributes
            /// </summary>
            public uint attributesCount;

            /// <summary>
            /// Pointer to attributes data structure
            /// </summary>
            public IntPtr attributes;
        }

        /// <summary>
        /// A struct representing the sub-images in an image.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SubImagesList
        {
            /// <summary>
            /// Number of sub-images in this list.
            /// </summary>
            public uint subImagesCount;

            /// <summary>
            /// Pointer to sub-image data.
            /// </summary>
            public IntPtr data;
        }

        /// <summary>
        /// Reads an IntPtr into an Array of structs of length arrayCount.
        /// </summary>
        /// <param name="pointerToArr">IntPtr pointing to the data to read.</param>
        /// <param name="arrayCount">Number of items in the array.</param>
        /// <typeparam name="T">Type of struct for the array.</typeparam>
        /// <returns>An array of type T structs.</returns>
        public static T[] IntPtrToManagedArray<T>(IntPtr pointerToArr, uint arrayCount) where T : struct
        {
            T[] managedArray = new T[arrayCount];
            var sizeElement = Marshal.SizeOf(typeof(T));

            for (int i = 0; i < arrayCount; i++)
            {
                IntPtr elemPtr = new IntPtr(pointerToArr.ToInt64() + i * sizeElement);
                managedArray[i] = Marshal.PtrToStructure<T>(elemPtr);
            }

            return managedArray;
        }

        /// <summary>
        /// Reads an ImageHeader and returns it as a Texture2D.
        /// </summary>
        /// <param name="subImgHeader">ImageHeader describing image to read.</param>
        /// <returns>A Texture2D based on the passed ImageHeader.</returns>
        /// <exception cref="Exception">Throws if number of channels is not supported.</exception>
        public static Texture2D Tex2DFromImageHeader(ImageHeader subImgHeader)
        {
            int numChannels = (int)subImgHeader.channelsCount;
            int w = (int)subImgHeader.width;
            int h = (int)subImgHeader.height;

            float[] rawData = new float[w * h * numChannels];
            unsafe
            {
                var sourcePtr = (ushort*)subImgHeader.data;
                for (int i = 0; i < 0 + w * h * numChannels; ++i)
                {
                    rawData[i] = Mathf.HalfToFloat(*sourcePtr++);
                }
            }

            Color[] pixels = new Color[h * w];

            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    int currPixel = r * w + c;
                    int currOutPixel = (h - 1 - r) * w + c;
                    switch (numChannels)
                    {
                        case 1: // only red
                            pixels[currOutPixel] = new Color(rawData[currPixel * numChannels], 0, 0);
                            break;
                        case 2: // rg
                            pixels[currOutPixel] = new Color(rawData[currPixel * numChannels],
                                rawData[currPixel * numChannels + 1], 0);
                            break;
                        case 3: //rgb
                            pixels[currOutPixel] = new Color(rawData[currPixel * numChannels],
                                rawData[currPixel * numChannels + 1], rawData[currPixel * numChannels + 2]);
                            break;
                        case 4: //rgba
                            pixels[currOutPixel] = new Color(rawData[currPixel * numChannels],
                                rawData[currPixel * numChannels + 1], rawData[currPixel * numChannels + 2],
                                rawData[currPixel * numChannels + 3]);
                            break;
                        default:
                            throw new Exception(
                                $"Unsupported number of channels - Should be 1, 2, 3 or 4 but was {numChannels}");
                    }
                }
            }

            var tex = new Texture2D(w, h, TextureFormat.RGBAHalf, false);
            tex.SetPixels(pixels);
            return tex;
        }
    }
}
