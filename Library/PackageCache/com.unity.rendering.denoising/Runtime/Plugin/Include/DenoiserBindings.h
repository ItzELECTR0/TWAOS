#ifndef _DENOISER_BINDINGS_H_
#define _DENOISER_BINDINGS_H_

#include <stdint.h>
#include "DllExport.h"

enum DenoiseFilterHint
{
	Default,
	Lightmap
};

typedef void* DenoiserHandle;

#ifdef __cplusplus
extern "C"
{
#endif

	DLL_EXPORT
	int32_t udnGetNumberOfAvailableDenoiserTypes();

	DLL_EXPORT
	const char* udnGetDenoiserTypeName(int32_t i);

	// denoiserType: one of "Optix", "OpenImageDenoise" or "Radeon" (WIP).
	// width, height: the width and height of the frames to be denoised. If the resolution changes dispose and create a new denoiser.
	// tilex, tiley: tile size in pixels, pass 0 to disable tiling (not all backends support this feature).
	// temporal: pass 1 to enable temporal mode (requires motion vectors - not all backends support this feature).
	// filterType: optimize the denoising for a specific type of data (i.e. Lightmap).
	DLL_EXPORT
	DenoiserHandle udnCreateDenoiser(const char* denoiserType, int32_t width, int32_t height, int32_t tilex, int32_t tiley, int32_t temporal, DenoiseFilterHint filterType);

	DLL_EXPORT
	void udnDisposeDenoiser(DenoiserHandle denoiser);

	// type: one of "color", "albedo", "normal" or "flow".
	// graphicsFormat: matches enumeration UnityEngine.Experimental.Rendering.GraphicsFormat, supported values are R16G16B16A16_SFloat, R32G32B32A32_SFloat.
	DLL_EXPORT
	int32_t udnDenoiseBuffer(DenoiserHandle denoiser, const char* type, void* data, int32_t graphicsFormat);

	DLL_EXPORT
	int32_t udnGetResult(DenoiserHandle denoiser, float* data);

	// Set the path for loading data (such as neural net models). 
	DLL_EXPORT
	int32_t udnSetPath(DenoiserHandle denoiser, const char* path);

#ifdef __cplusplus
}
#endif

#endif //_DENOISER_BINDINGS_H_
