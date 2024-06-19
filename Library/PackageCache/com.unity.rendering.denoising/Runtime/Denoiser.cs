using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Denoising
{
    /// <summary>
    /// The Denoiser class exposes a public API for denoising images that are stored in a NativeArray.
    /// </summary>
    public class Denoiser : DenoiserBase
    {
        #region Public API

        /// <summary>
        /// Create a new denoiser.
        /// </summary>
        /// <param name="type">The type of denoiser backend to create.</param>
        /// <param name="width">The width of the image buffer to be denoised.</param>
        /// <param name="height">The height of the image buffer to be denoised.</param>
        /// <param name="tileX">Determines the width of tiles in pixels. 0 disables tiling. Use this parameter to divide the image buffer into tiles that match the amount of memory available on your GPU.</param>
        /// <param name="tileY">Determines the height of tiles in pixels. 0 disables tiling. Use this parameter to divide the image buffer into tiles that match the amount of memory available on your GPU.</param>
        /// <param name="filterHint">The type of filter to use in the denoising operations (default or lightmap)</param>
        /// <returns>Returns the state of the operation, whether __Success__ or __Failure__.</returns>
        public State Init(DenoiserType type, int width, int height, int tileX = 0, int tileY = 0, FilterHint filterHint = FilterHint.Default)
        {
            if (InternalInit(type, width, height, tileX, tileY, filterHint) == State.Success)
            {
                return CreateDenoiser();
            }
            return State.Failure;
        }

        /// <summary>
        /// Create a denoise request using a native array texture as input.
        /// </summary>
        /// <param name="type">The type of data passed in the native array. Supported options are __color__, __albedo__, __normal__, or __flow__.</param>
        /// <param name="src">The input data used for the denoising.</param>
        /// <returns>Returns the state of the operation, whether __Success__ or __Failure__.</returns>
        unsafe public State DenoiseRequest(string type, NativeArray<Vector4> src)
        {
            if (m_DenoiserBackend == IntPtr.Zero)
            {
                State ret = CreateDenoiser();
                if (ret < 0)
                {
                    m_InternalState = State.Failure;
                    return State.Failure;
                }
            }
            void* ptr = NativeArrayUnsafeUtility.GetUnsafePtr(src);
            var format = GraphicsFormat.R32G32B32A32_SFloat;
            int status = udnDenoiseBuffer(m_DenoiserBackend, new StringBuilder(type), ptr, format);
            if (status < 0)
            {
                m_InternalState = State.Failure;
                return State.Failure;
            }
            return State.Success;
        }

        /// <summary>
        /// Retrieve the results of a denoise request.
        /// </summary>
        /// <param name="dst">The destination texture that receives the denoise results.</param>
        /// <returns>Returns the state of the operation, whether __Success__ or __Failure__.</returns>
        unsafe public State GetResults(NativeArray<Vector4> dst)
        {
            void* ptr = NativeArrayUnsafeUtility.GetUnsafePtr(dst);
            int status = udnGetResult(m_DenoiserBackend, ptr);
            if (status < 0)
            {
                m_InternalState = State.Failure;
                return State.Failure;
            }
            return State.Success;
        }

        #endregion // Public API
    }
}
