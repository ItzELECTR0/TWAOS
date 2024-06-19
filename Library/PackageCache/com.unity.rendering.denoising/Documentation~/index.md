# Unity Denoising

Unity Denoising provides high quality denoising using the Optix, Radeon Image Filters, and Open Image Denoise libraries. Currently Unity's [Progressive Lightmapper](https://docs.unity3d.com/2022.2/Documentation/Manual/progressive-lightmapper.html) and [HDRP Path Tracer](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Ray-Tracing-Path-Tracing.html) use this denoiser.

# Available Backends
The following denoising backends are currently available:
- **Optix** : based on NVIDIA's [Optix](https://developer.nvidia.com/rtx/ray-tracing/optix). 
- **Open Image Denoise** : based on [Intel Open Image Denoise](https://github.com/OpenImageDenoise/oidn).
- **Radeon Image Filters** : based on AMD's [Radeon Image Filters](https://github.com/GPUOpen-LibrariesAndSDKs/RadeonImageFilter).

# Public API

Unity Denoising exposes a common C# public API that you can use to access the denoising functionality of the Optix, Radeon, and Open Image denoising backends.
There are three different ways to use this API.

- **Immediate** : Works with images stored as native arrays in memory. Denoising results are immediately available.
- **Synchronous API with command buffers** : Works with images stored as GPU textures. Suitable for applications that sync denoising with other graphics-related operations recorded in a command buffer. Blocks function calls until the operation completes and results are available.
- **Asynchronous API with command buffers** : Works with images stored as GPU textures. Suitable for applications that sync denoising with other graphics-related operations recorded in a command buffer. Does not block function calls; as a result can obscure denoising request latency by processing in the background.

## Immediate API
The following script snippet demonstrates how to denoise an image stored in a Native Array.

```C#
		// Create a new denoiser object 
		var denoiser = new Denoiser();

        // Initialize the denoising state
        Denoiser.State result = denoiser.Init(DenoiserType.OpenImageDenoise, width, height);
		Assert.AreEqual(Denoiser.State.Success, result);

        // Create a new denoise request for a colorImage on a native array
        result = denoiser.DenoiseRequest("color", colorImage);
		Assert.AreEqual(Denoiser.State.Success, result);

        // Get the results
        var dst = new NativeArray<Vector4>(colorImage.Length, Allocator.Temp);
		result = denoiser.GetResults(dst);
		Assert.AreEqual(Denoiser.State.Success, result);
```
## Synchronous API using command buffers 
The following script snippet demonstrates how to denoise an image stored in a Render Texture by using the synchronous (blocking) API.

```C#
		// Create a new denoiser object 
		var denoiser = new CommandBufferDenoiser();

        // Initialize the denoising state
        Denoiser.State result = denoiser.Init(DenoiserType.OpenImageDenoise, width, height);
		Assert.AreEqual(Denoiser.State.Success, result);

        // Create a new denoise request for a color image stored in a Render Texture
        denoiser.DenoiseRequest(cmd, "color", colorImage);

        // Wait until the denoising request is done executing
        result = denoiser.WaitForCompletion(renderContext, cmd);
        Assert.AreEqual(Denoiser.State.Success, result);

        // Get the results
        var dst = new RenderTexture(colorImage.descriptor);
		result = denoiser.GetResults(cmd, dst);
		Assert.AreEqual(Denoiser.State.Success, result);
```

## Asynchronous API using command buffers
The following script snippet demonstrates how to denoise an image stored in a Render Texture by using the asynchronous (non-blocking) API.

```C#
		// Create a new denoiser object 
		var denoiser = new CommandBufferDenoiser();

        // Initialize the denoising state
        Denoiser.State result = denoiser.Init(DenoiserType.OpenImageDenoise, width, height);
		Assert.AreEqual(Denoiser.State.Success, result);

        // Create a new denoise request for a color image stored in a Render Texture
        denoiser.DenoiseRequest(cmd, "color", colorImage);
        
        // Use albedo AOV to improve results
        denoiser.DenoiseRequest(cmd, "albedo", albedoImage);

        // Check if the denoiser request is still executing
        if (denoiser.QueryCompletion() != Denoiser.State.Executing)
        {
            // Get the results
            var dst = new RenderTexture(colorImage.descriptor);
            result = denoiser.GetResults(cmd, dst);
            Assert.AreEqual(Denoiser.State.Success, result);
        }

```

# Improve denoising quality using Arbitrary Output Variables (AOVs)
Aside from the input image that will be denoised, the denoiser can use additional input buffers (commonly referred to as Arbitrary Output Variables or AOVs) to improve the output quality. The expected input format for all buffers is floating point RGBA. Currently the following input types are supported:
- **color** : The (HDR) color image to be denoised.
- **albedo** : The albedo values of the image being denoised, stored in the RGB channels.
- **normal** : The normals of the image being denoised, stored in the RGB channels, in the [-1, 1] range.
- **flow** : Optix only. screen-space motion vectors or optical flow measured in pixels for the input image compared to the previous frame, stored in the RG channels (currently used only by the Optix backend)
