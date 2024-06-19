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
    /// The Denoiser class exposes a public API for denoising images that are stored as render textures. The operations are asynchronous and are recorded in a command buffer.
    /// </summary>
    public class CommandBufferDenoiser : DenoiserBase
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
            return InternalInit(type, width, height, tileX, tileY, filterHint);
        }

        /// <summary>
        /// Create a denoise request using a render texture as input.
        /// </summary>
        /// <param name="cmd">The command buffer where Unity records the denoise request.</param>
        /// <param name="type">The type of data passed in the render texture. Supported options are __color__, __albedo__, __normal__, or __flow__.</param>
        /// <param name="src">The input texture used for denoising.</param>
        /// <returns>The state of the denoising operation. The valid states are __Success__ and __Failure__.</returns>
        public State DenoiseRequest(CommandBuffer cmd, string type, RenderTexture src)
        {
            if (src == null)
            {
                return State.Failure;
            }

            switch (type.ToLower())
            {
                case "color":
                    DoReadBack(cmd, src, ref m_ColorTexture, ColorReadBack);
                    break;
                case "albedo":
                    DoReadBack(cmd, src, ref m_AlbedoTexture, AlbedoReadBack);
                    m_UseAOV = true;
                    break;
                case "normal":
                    DoReadBack(cmd, src, ref m_NormalsTexture, NormalsReadBack);
                    m_UseAOV = true;
                    break;
                case "flow":
                    DoReadBack(cmd, src, ref m_FlowTexture, MotionVectorReadBack);
                    m_Temporal = true;
                    break;
                default:
                    Debug.Log("Unknown AOV type in denoising request");
                    return State.Failure;
            }

            if (m_DenoisedHistory == null || m_DenoisedHistory.width != m_RequestWidth || m_DenoisedHistory.height != m_RequestHeight)
            {
                var format = GraphicsFormat.R32G32B32A32_SFloat;
                InternalDestroy(m_DenoisedHistory);
                m_DenoisedHistory = new Texture2D(m_RequestWidth, m_RequestHeight, format, TextureCreationFlags.None);
            }

            return State.Success;
        }

        /// <summary>
        /// Query the completion of a denoise request.
        /// </summary>
        /// <returns>Returns __Executing__ if the denoising operation is not yet completed. If denoising is completed, returns __Success__ or __Failure__.</returns>
        public State QueryCompletion()
        {
            bool isReadbackDone = m_AsyncBeautyDone;

            if (m_UseAOV)
                isReadbackDone = isReadbackDone && m_AsyncNormalDone && m_AsyncAlbedoDone;

            if (m_Temporal)
                isReadbackDone = isReadbackDone && m_AsyncMVDone;

            if (m_RequestWidth == -1 || m_RequestHeight == -1)
                isReadbackDone = false;

            if (isReadbackDone && m_WorkerThread == null)
            {
                m_InternalState = State.Executing;
                m_WorkerThread = new Thread(ThreadWorker);
                m_WorkerThread.Start();
            }

            if (m_InternalState != State.Executing)
            {
                // We are done, clear the thread;
                m_WorkerThread = null;
            }

            if (!isReadbackDone)
            {
                return State.Executing;
            }

            return m_InternalState;
        }

        /// <summary>
        /// Retrieve the results of a denoise request.
        /// </summary>
        /// <param name="cmd">The command buffer where Unity records this operation.</param>
        /// <param name="dst">The destination texture that receives the denoise results.</param>
        /// <returns>Returns the state of the operation, whether __Success__ or __Failure__.</returns>
        unsafe public State GetResults(CommandBuffer cmd, RenderTexture dst)
        {
            m_DenoisedHistory.SetPixelData<Vector4>(m_BeautyData, 0);
            m_DenoisedHistory.Apply(false);
            cmd.CopyTexture(m_DenoisedHistory, 0, 0, 0, 0, m_RequestWidth, m_RequestHeight, dst, 0, 0, 0, 0);
            InternalReset();
            return State.Success;
        }

        /// <summary>
        /// Wait until the denoise request has finished executing.
        /// </summary>
        /// <param name="renderContext">The scriptable render context used for rendering.</param>
        /// <param name="cmd">The command buffer where Unity records this operation.</param>
        /// <returns>Returns the state of the operation, whether __Success__ or __Failure__.</returns>
        public State WaitForCompletion(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            cmd.WaitAllAsyncReadbackRequests();

            renderContext.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            renderContext.Submit();

            m_InternalState = ExecuteDenoiseRequest();
            return m_InternalState;
        }

        #endregion // Public API

        #region Internal Implementation

        internal static void InternalDestroy(Object obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
                    Object.Destroy(obj);
                else
                    Object.DestroyImmediate(obj);
#else
                Object.Destroy(obj);
#endif
            }
        }

        private void ThreadWorker()
        {
            ExecuteDenoiseRequest();
        }

        unsafe internal State ExecuteDenoiseRequest()
        {
            if (m_RequestWidth == -1 || m_RequestHeight == -1)
            {
                m_InternalState = State.Failure;
                return m_InternalState;
            }

            if (!IsActiveDenoiserValid() || (!m_Temporal && m_DenoiserBackend != IntPtr.Zero))
            {
                DisposeDenoiser();
            }

            if (m_DenoiserBackend == IntPtr.Zero)
            {
                State ret = CreateDenoiser();
                if (ret < 0)
                {
                    m_InternalState = State.Failure;
                    return m_InternalState;
                }
            }

            void* ptrColor = NativeArrayUnsafeUtility.GetUnsafePtr(m_BeautyData);
            var format = GraphicsFormat.R32G32B32A32_SFloat;
            int status = udnDenoiseBuffer(m_DenoiserBackend, new StringBuilder("color", 7), ptrColor, format);
            if (status < 0)
            {
                m_InternalState = State.Failure;
                return m_InternalState;
            }

            if (m_UseAOV)
            {
                void* ptrNormals = null;
                ptrNormals = NativeArrayUnsafeUtility.GetUnsafePtr(m_NormalData);
                status = udnDenoiseBuffer(m_DenoiserBackend, new StringBuilder("normal", 7), ptrNormals, format);
                if (status < 0)
                {
                    m_InternalState = State.Failure;
                    return m_InternalState;
                }

                void* ptrAlbedo = null;
                ptrAlbedo = NativeArrayUnsafeUtility.GetUnsafePtr(m_AlbedoData);
                status = udnDenoiseBuffer(m_DenoiserBackend, new StringBuilder("albedo", 7), ptrAlbedo, format);
                if (status < 0)
                {
                    m_InternalState = State.Failure;
                    return m_InternalState;
                }
            }

            if (m_Temporal)
            {
                void* ptrMV = null;
                ptrMV = NativeArrayUnsafeUtility.GetUnsafePtr(m_MVData);
                status = udnDenoiseBuffer(m_DenoiserBackend, new StringBuilder("flow", 5), ptrMV, format);
                if (status < 0)
                {
                    m_InternalState = State.Failure;
                    return m_InternalState;
                }
            }

            status = udnGetResult(m_DenoiserBackend, ptrColor);
            if (status < 0)
            {
                m_InternalState = State.Failure;
                return m_InternalState;
            }

            if (!m_Temporal)
            {
                DisposeDenoiser();
            }

            m_InternalState = 0;
            return State.Success;
        }

        internal bool IsActiveDenoiserValid()
        {
            return m_ActiveBacked == m_Type && m_ActiveWidth == m_RequestWidth && m_ActiveHeight == m_RequestHeight && m_ActiveAOV == m_UseAOV;
        }

        internal void InternalReset()
        {
            m_RequestWidth = -1;
            m_RequestHeight = -1;
            m_AsyncBeautyDone = false;
            m_AsyncAlbedoDone = false;
            m_AsyncNormalDone = false;
            m_AsyncMVDone = false;
            m_Temporal = false;
            m_UseAOV = false;

            if (m_BeautyData.IsCreated)
            {
                m_BeautyData.Dispose();
            }
            if (m_AlbedoData.IsCreated)
            {
                m_AlbedoData.Dispose();
            }
            if (m_NormalData.IsCreated)
            {
                m_NormalData.Dispose();
            }
            if (m_MVData.IsCreated)
            {
                m_MVData.Dispose();
            }
        }

        internal void ColorReadBack(AsyncGPUReadbackRequest request)
        {
            if (!request.hasError)
            {
                var src = request.GetData<Vector4>();
                if (m_BeautyData.IsCreated)
                {
                    m_BeautyData.Dispose();
                }
                m_BeautyData = new NativeArray<Vector4>(src.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_BeautyData.CopyFrom(src);
                m_AsyncBeautyDone = true;
            }
        }

        internal void AlbedoReadBack(AsyncGPUReadbackRequest request)
        {
            if (!request.hasError)
            {
                var src = request.GetData<Vector4>();
                if (m_AlbedoData.IsCreated)
                {
                    m_AlbedoData.Dispose();
                }
                m_AlbedoData = new NativeArray<Vector4>(src.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_AlbedoData.CopyFrom(src);
                m_AsyncAlbedoDone = true;
            }
        }

        internal void NormalsReadBack(AsyncGPUReadbackRequest request)
        {
            if (!request.hasError)
            {
                var src = request.GetData<Vector4>();
                if (m_NormalData.IsCreated)
                {
                    m_NormalData.Dispose();
                }
                m_NormalData = new NativeArray<Vector4>(src.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_NormalData.CopyFrom(src);
                m_AsyncNormalDone = true;
            }
        }

        internal void MotionVectorReadBack(AsyncGPUReadbackRequest request)
        {
            if (!request.hasError)
            {
                var src = request.GetData<Vector4>();
                if (m_MVData.IsCreated)
                {
                    m_MVData.Dispose();
                }
                m_MVData = new NativeArray<Vector4>(src.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_MVData.CopyFrom(src);
                m_AsyncMVDone = true;
            }
        }

        internal void DoReadBack(CommandBuffer cmd, RenderTexture src, ref RenderTexture stagingTexture, Action<AsyncGPUReadbackRequest> callback)
        {
            // If denoiser size and texture size does not match, then make a copy of the data in a new texture
            if (m_RequestWidth != src.width || m_RequestHeight != src.height)
            {
                if (stagingTexture == null || stagingTexture.width != m_RequestWidth || stagingTexture.height != m_RequestHeight)
                {
                    InternalDestroy(stagingTexture);
                    stagingTexture = new RenderTexture(m_RequestWidth, m_RequestHeight, 0, src.format);
                }
                cmd.CopyTexture(src, 0, 0, 0, 0, m_RequestWidth, m_RequestHeight, stagingTexture, 0, 0, 0, 0);
                cmd.RequestAsyncReadback(stagingTexture, 0, callback);
            }
            else
            {
                cmd.RequestAsyncReadback(src, 0, callback);
            }
        }

        // The denoising runs in a worker thread when the user is using the async API (QueryCompletion instead of WaitForCompletion )
        Thread m_WorkerThread;
        NativeArray<Vector4> m_BeautyData;
        bool m_AsyncBeautyDone = false;

        NativeArray<Vector4> m_AlbedoData;
        bool m_AsyncAlbedoDone = false;

        NativeArray<Vector4> m_NormalData;
        bool m_AsyncNormalDone = false;

        NativeArray<Vector4> m_MVData;
        bool m_AsyncMVDone = false;

        RenderTexture m_ColorTexture;
        RenderTexture m_AlbedoTexture;
        RenderTexture m_NormalsTexture;
        RenderTexture m_FlowTexture;

        Texture2D m_DenoisedHistory;
    }

    #endregion //Internal Implementation
}
