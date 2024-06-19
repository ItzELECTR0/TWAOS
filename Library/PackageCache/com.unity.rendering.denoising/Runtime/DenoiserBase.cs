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
    /// Enumeration of the available denoiser types.
    /// </summary>
    public enum DenoiserType
    {
        /// <summary>
        /// Do not perform any denoising.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the Intel Open Image Denoise backend.
        /// </summary>
        [InspectorName("Intel Open Image Denoise")]
        OpenImageDenoise,

        /// <summary>
        /// Use the NVIDIA Optix Denoiser backend.
        /// </summary>
        [InspectorName("NVIDIA Optix Denoiser")]
        Optix
    }

    public abstract class DenoiserBase
    {

        #region Public API

        /// <summary>
        /// The type of filter that denoises the input data.
        /// </summary>
        public enum FilterHint
        {
            /// <summary>
            /// Denoise image buffers with the default filter. 
            /// </summary>
            Default,
            /// <summary>
            /// Denoise lightmaps with the lightmap filter.
            /// </summary>
            Lightmap
        }

        /// <summary>
        /// The state of a denoising operation.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// The denoising operation failed.
            /// </summary>
            Failure = -1,
            /// <summary>
            /// The denoising operation was successful.
            /// </summary>
            Success = 0,
            /// <summary>
            /// The denoising operation is still executing.
            /// </summary>
            Executing = 1
        }

        /// <summary>
        /// The denoiser backend type that is used for the denoising operations.
        /// </summary>
        public DenoiserType type { get => m_Type;}

        /// <summary>
        /// The denoising filter you have selected.
        /// </summary>
        public FilterHint filterHint { get => m_FilterHint; }

        /// <summary>
        /// Checks if a denoiser backend type is available on the current hardware configuration.
        /// </summary>
        /// <param name="type">The denoiser backend type to check for.</param>
        /// <returns>Returns __true__ if the denoiser type is available, __false__ otherwise.</returns>
        public static bool IsDenoiserTypeSupported(DenoiserType type)
        {
            if (s_SupportTable.ContainsKey(type))
            {
                return s_SupportTable[type];
            }

            string backEndName = type.ToString();
            var denoiser = udnCreateDenoiser(new StringBuilder(backEndName, backEndName.Length), 128, 128, 0, 0, 0, FilterHint.Default);
            if (denoiser != IntPtr.Zero)
            {
                udnDisposeDenoiser(denoiser);
                s_SupportTable.Add(type, true);
                return true;
            }
            s_SupportTable.Add(type, false);
            return false;
        }

        /// <summary>
        /// Disposes a denoiser
        /// </summary>
        public void DisposeDenoiser()
        {
            udnDisposeDenoiser(m_DenoiserBackend);
            m_DenoiserBackend = IntPtr.Zero;
            m_ActiveBacked = DenoiserType.None;
            m_ActiveWidth = -1;
            m_ActiveHeight = -1;
            m_TileWidth = 0;
            m_TileHeight = 0;
            m_ActiveAOV = false;
        }

        #endregion // Public API

        #region Internal Implementation

        internal State InternalInit(DenoiserType type, int width, int height, int tileX = 0, int tileY = 0, FilterHint filterHint = FilterHint.Default)
        {
            m_RequestWidth = width;
            m_RequestHeight = height;
            m_TileWidth = tileX;
            m_TileHeight = tileY;
            m_Type = type;
            m_FilterHint = filterHint;
            return State.Success;
        }

        internal State CreateDenoiser()
        {
            string backEndName = m_Type.ToString();
            m_DenoiserBackend = udnCreateDenoiser(new StringBuilder(backEndName, backEndName.Length), m_RequestWidth, m_RequestHeight, m_TileWidth, m_TileHeight, m_Temporal ? 1 : 0, m_FilterHint);

            if (m_DenoiserBackend == IntPtr.Zero)
                return State.Failure;

            m_ActiveBacked = m_Type;
            m_ActiveWidth = m_RequestWidth;
            m_ActiveHeight = m_RequestHeight;
            m_ActiveAOV = m_UseAOV;

            // Set Path to resources
            string absolutePath = System.IO.Path.GetFullPath("Packages/com.unity.rendering.denoising/Runtime/Resources");
            udnSetPath(m_DenoiserBackend, new StringBuilder(absolutePath, absolutePath.Length));
            return State.Success;
        }

        static private Dictionary<DenoiserType, bool> s_SupportTable = new Dictionary<DenoiserType, bool>();

        protected internal int m_RequestWidth = -1;
        protected internal int m_RequestHeight = -1;
        protected internal FilterHint m_FilterHint = FilterHint.Default;
        protected internal DenoiserType m_Type = DenoiserType.None;
        protected internal State m_InternalState = State.Success;

        [DllImport("UnityDenoisingPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        protected internal static extern IntPtr udnCreateDenoiser(StringBuilder backEnd, int width, int height, int tilex, int tiley, int temporal, FilterHint mode);

        [DllImport("UnityDenoisingPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        protected internal static extern void udnDisposeDenoiser(IntPtr denoiser);

        [DllImport("UnityDenoisingPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        protected internal static extern unsafe int udnGetResult(IntPtr denoiser, void* data);

        [DllImport("UnityDenoisingPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        protected internal static extern unsafe int udnDenoiseBuffer(IntPtr denoiser, StringBuilder bufferType, void* data, GraphicsFormat format);

        [DllImport("UnityDenoisingPlugin", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        protected internal static extern unsafe int udnSetPath(IntPtr denoiser, StringBuilder bufferType);

        protected internal IntPtr m_DenoiserBackend = IntPtr.Zero;
        protected internal bool m_UseAOV = false;
        protected internal bool m_Temporal = false;

        protected internal DenoiserType m_ActiveBacked = DenoiserType.None;
        protected internal bool m_ActiveAOV = false;
        protected internal int  m_ActiveWidth = -1;
        protected internal int  m_ActiveHeight = -1;
        protected internal int  m_TileWidth = 0;
        protected internal int  m_TileHeight = 0;

        #endregion // Internal Implementation
    }
}
