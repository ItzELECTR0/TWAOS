#define RECORDER_TESTS_DELETE_FILES
using System;
using System.IO;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Bindings.OpenImageIO;

namespace UnityEngine.Bindings.OpenImageIO.Tests
{
    class OIIOBindingsTests
    {
        string m_OutputFilePath;

        NativeArray<OiioWrapper.ImageHeader> m_Headers;
        NativeArray<OiioWrapper.Attribute> m_Attributes;
        NativeArray<ushort> m_ImageBuffer;

        private OiioWrapper.SubImagesList? allocatedMemoryForAOVs;

        unsafe OiioWrapper.ImageHeader GenerateImageHeader(uint width, uint height, uint channelsCount, NativeArray<ushort> imageBuffer, string extension, int compressionLevel = 100)
        {
            m_Attributes = new NativeArray<OiioWrapper.Attribute>(extension == "jpg" ? 2 : 1, Allocator.Persistent);
            m_Attributes[0] = new OiioWrapper.Attribute()
            {
                key = "oiio:imageName",
                value = "One"
            };

            if (extension == "jpg")
            {
                m_Attributes[1] = new OiioWrapper.Attribute()
                {
                    key = "compression",
                    value = $"jpeg:{compressionLevel}"
                };
            }

            return new OiioWrapper.ImageHeader
            {
                width = width,
                height = height,
                channelsCount = channelsCount,
                data = new IntPtr(imageBuffer.GetUnsafeReadOnlyPtr()),
                attributesCount = (uint)m_Attributes.Length,
                attributes = new IntPtr(m_Attributes.GetUnsafeReadOnlyPtr())
            };
        }

        Texture2D LoadImageFromPath(string path)
        {
            Texture2D outImg = new Texture2D(0, 0);
            outImg.LoadImage(File.ReadAllBytes(path));
            return outImg;
        }

        void AssertImagesAreTheSame(Texture2D refImage, Texture2D actualImage)
        {
            Assert.AreEqual(refImage.width, actualImage.width, $"Expected width to be {refImage.width} but got {actualImage.width}.");
            Assert.AreEqual(refImage.height, actualImage.height,  $"Expected height to be {refImage.height} but got {actualImage.height}.");
            Assert.AreEqual(refImage.GetPixels(), actualImage.GetPixels(), $"Image content is different from expected result.");
        }

        [TearDown]
        public void DeleteGeneratedFiles()
        {
            if (m_Headers.IsCreated)
                m_Headers.Dispose();

            if (m_Attributes.IsCreated)
                m_Attributes.Dispose();

            if (m_ImageBuffer.IsCreated)
                m_ImageBuffer.Dispose();

            if (allocatedMemoryForAOVs.HasValue)
            {
                OiioWrapper.FreeAllocatedMemory(allocatedMemoryForAOVs.Value);
                allocatedMemoryForAOVs = null;
            }
# if RECORDER_TESTS_DELETE_FILES
            if (File.Exists($"{m_OutputFilePath}.meta"))
                File.Delete($"{m_OutputFilePath}.meta");

            if (File.Exists(m_OutputFilePath))
                File.Delete(m_OutputFilePath);
#endif
        }

        [Test]
        public void OIIOBindings_SuccessfullyLoaded_WriteEXRImageFile()
        {
            uint width = 10;
            uint height = 10;
            uint channelCount = 3;

            m_Headers = new NativeArray<OiioWrapper.ImageHeader>(1, Allocator.Persistent);
            m_ImageBuffer = new NativeArray<ushort>((int)(width * height * channelCount), Allocator.Persistent);

            for (var i = 0; i < width * height * channelCount; ++i)
            {
                m_ImageBuffer[i] = Mathf.FloatToHalf(0.5f);
            }
            unsafe
            {
                m_Headers[0] = GenerateImageHeader(width, height, channelCount, m_ImageBuffer, "exr");

                m_OutputFilePath = "Assets/" + Path.GetRandomFileName() + ".exr";
                var returnCode = OiioWrapper.WriteImage(m_OutputFilePath, 1,
                    (OiioWrapper.ImageHeader*)m_Headers.GetUnsafeReadOnlyPtr());

                Assert.AreEqual(0, returnCode, $"WriteImage() returned with code: {returnCode}");
                Assert.IsTrue(File.Exists(m_OutputFilePath), $"Output file \"{m_OutputFilePath}\" does not exist.");
            }
        }

        [Test]
        [TestCase("exr", 0, "f2ff785a344874d429edb7be0065d286", Description = "EXR File Output")]
        [TestCase("png", 0, "11249b7f3bcc3594084e04d5f69f5163", Description = "PNG File Output")]
        [TestCase("jpg", 70, "bca35194ecf29384eb80cf4666af720e", Description = "JPG File Output")]
        public void OIIOBindings_WriteFilesAsExpected(string extension, int jpgCompression, string referenceImageGUID)
        {
            unsafe
            {
                int width = 17;
                int height = 31;
                int channelCount = 3;

                m_Headers = new NativeArray<OiioWrapper.ImageHeader>(1, Allocator.Persistent);
                m_ImageBuffer = new NativeArray<ushort>((width * height * channelCount), Allocator.Persistent);

                FillBuffer(width, height, channelCount);
                m_Headers[0] = GenerateImageHeader((uint)width, (uint)height, (uint)channelCount, m_ImageBuffer, extension, jpgCompression);

                m_OutputFilePath = $"Assets/{Path.GetRandomFileName()}.{extension}";
                var returnCode = OiioWrapper.WriteImage(m_OutputFilePath, 1,
                    (OiioWrapper.ImageHeader*)m_Headers.GetUnsafeReadOnlyPtr());
                Assert.AreEqual(0, returnCode, $"WriteImage() returned with code: {returnCode}");
                Assert.IsTrue(File.Exists(m_OutputFilePath), $"Output file \"{m_OutputFilePath}\" does not exist.");

                var path = AssetDatabase.GUIDToAssetPath(referenceImageGUID);
                AssertImagesAreTheSame(LoadImageFromPath(path), LoadImageFromPath(m_OutputFilePath));
            }
        }

        [Test]
        [TestCase("f2ff785a344874d429edb7be0065d286", Description = "EXR File Input")]
        public void OIIOBindings_ReadFilesAsExpected(string referenceImageGUID)
        {
            var path = AssetDatabase.GUIDToAssetPath(referenceImageGUID);
            Assert.IsTrue(File.Exists(path), $"Reference file '{path}' does not exist.");

            allocatedMemoryForAOVs = OiioWrapper.ReadImage(Path.GetFullPath(path));
            var subImagesArr = OiioWrapper.IntPtrToManagedArray<OiioWrapper.ImageHeader>(allocatedMemoryForAOVs.Value.data, allocatedMemoryForAOVs.Value.subImagesCount);

            Assert.AreEqual(1, subImagesArr.Length, "OIIOBindings read wrong number of subImages.");
            foreach (var subImage in subImagesArr)
            {
                var tex = OiioWrapper.Tex2DFromImageHeader(subImage);
                AssertImagesAreTheSame(LoadImageFromPath(path), tex);
            }
        }

        private void FillBuffer(int width, int height, int channelCount)
        {
            for (var row = 0; row < height; row++)
            {
                ushort red, green, blue;

                // Alternating lines (red, green, blue, red, green, blue ...)
                red = Mathf.FloatToHalf(row % 3 == 0 ? 1 : 0);
                green = Mathf.FloatToHalf(row % 3 == 1 ? 1 : 0);
                blue = Mathf.FloatToHalf(row % 3 == 2 ? 1 : 0);

                for (var col = 0; col < width; col++)
                {
                    int currPixelStartIdx = (row * width + col) * channelCount;
                    m_ImageBuffer[currPixelStartIdx] = red;
                    m_ImageBuffer[currPixelStartIdx + 1] = green;
                    m_ImageBuffer[currPixelStartIdx + 2] = blue;
                }
            }
        }
    }
}
