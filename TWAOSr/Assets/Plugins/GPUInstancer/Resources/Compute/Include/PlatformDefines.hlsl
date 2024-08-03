#ifndef __platformDefines_hlsl_
#define __platformDefines_hlsl_

#if SHADER_API_METAL
    #define GPUI_THREADS 256
    #define GPUI_THREADS_2D 16
#elif SHADER_API_GLES3
    #define GPUI_THREADS 128
    #define GPUI_THREADS_2D 8
#elif SHADER_API_VULKAN
    #define GPUI_THREADS 128
    #define GPUI_THREADS_2D 8
#elif SHADER_API_GLCORE
    #define GPUI_THREADS 256
    #define GPUI_THREADS_2D 16
#elif SHADER_API_PS4
    #define GPUI_THREADS 512
    #define GPUI_THREADS_2D 16
#else
    #define GPUI_THREADS 512
    #define GPUI_THREADS_2D 16
#endif

#endif