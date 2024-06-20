using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;

namespace UnityEngine.Rendering.HighDefinition
{
    /// <summary>
    /// World light manager handles all lights in a scene and makes them accessible on the GPU. This includes
    /// information about their shape.
    ///
    /// 1. First create WorldLights object and populate it using CollectWorldLights().
    /// 2. Pass the WorldLights object to BuildWorldLightDatas() to create the GPU representation of the lights.
    /// 3. (optionally) Pass the WorldLights object to BuildWorldLightVolumes() to create the GPU representation of the light bounds.
    /// 4. Bind the buffers using Bind
    ///
    ///
    /// WorldLights life time loop
    /// - Construct / Update
    /// - Reset
    ///
    /// WorldLightsGpu and WorldLightsVolumes life time loop
    /// - Construct / Update
    /// - PushToGpu
    /// - Bind
    /// - Release
    ///
    /// TODO-WL: Replace data structures with nativearrays to be able to do burst jobs
    /// TODO-WL: CollectWorldLights() should be replaced with updates using the object dispatcher and kept persistent on the GPU
    /// TODO-WL: Remove the relative camera light positions, they need to be global.
    /// </summary>
    ///


    /// Flags:
    /// Active = 1
    /// Rest user defined
    [GenerateHLSL(PackingRules.Exact, false)]
    struct WorldLightVolume
    {
        public uint flags;
        public uint shape;
        public Vector3 position;
        public Vector3 range;
        public uint lightType;
        public uint lightIndex;
    }

    class  WorldLightsSettings
    {
        public bool enabled = false;
    }

    class WorldLights
    {
        public NativeList<HDLightRenderEntity> hdPointLightArray = new NativeList<HDLightRenderEntity>(Allocator.Persistent);
        public NativeList<HDLightRenderEntity> hdLineLightArray = new NativeList<HDLightRenderEntity>(Allocator.Persistent);
        public NativeList<HDLightRenderEntity> hdRectLightArray = new NativeList<HDLightRenderEntity>(Allocator.Persistent);
        public NativeList<HDLightRenderEntity> hdDiscLightArray = new NativeList<HDLightRenderEntity>(Allocator.Persistent);
        public NativeList<HDLightRenderEntity> hdLightEntityArray = new NativeList<HDLightRenderEntity>(Allocator.Persistent);

        // The list of reflection probes
        public List<HDProbe> reflectionProbeArray = new List<HDProbe>();

        // Counter of the total number of lights
        public int totalLighttCount => normalLightCount + envLightCount + decalCount;

        public int normalLightCount => pointLightCount + lineLightCount + rectLightCount + discLightCount;
        public int envLightCount => reflectionProbeArray.Count;
        public int decalCount = 0;

        public int pointLightCount => hdPointLightArray.Length;
        public int lineLightCount => hdLineLightArray.Length;
        public int rectLightCount => hdRectLightArray.Length;
        public int discLightCount => hdDiscLightArray.Length;

        internal void Reset()
        {
            hdPointLightArray.Clear();
            hdLineLightArray.Clear();
            hdRectLightArray.Clear();
            hdDiscLightArray.Clear();
            hdLightEntityArray.Clear();
            reflectionProbeArray.Clear();
        }

        internal void Release()
        {
            hdPointLightArray.Dispose();
            hdLineLightArray.Dispose();
            hdRectLightArray.Dispose();
            hdDiscLightArray.Dispose();
            hdLightEntityArray.Dispose();
        }
    }

    class WorldLightsGpu
    {
        // Light runtime data
        NativeArray<LightData> m_LightDataCPUArray = new NativeArray<LightData>(WorldLightManager.SizeAlignment, Allocator.Persistent);
        GraphicsBuffer m_LightDataGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(LightData)));

        // Env Light data
        NativeArray<EnvLightData> m_EnvLightDataCPUArray = new NativeArray<EnvLightData>(WorldLightManager.SizeAlignment, Allocator.Persistent);
        GraphicsBuffer m_EnvLightDataGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(EnvLightData)));

        // Env Light data support data
        WorldEnvLightReflectionData m_EnvLightReflectionDataRT = new WorldEnvLightReflectionData();

        int m_numLights = 0;
        int m_numEnvLights = 0;

        internal ref LightData GetRef(int i)
        {
            unsafe
            {
                LightData* data = (LightData*)m_LightDataCPUArray.GetUnsafePtr<LightData>() + i;
                return ref UnsafeUtility.AsRef<LightData>(data);
            }
        }

        internal ref EnvLightData GetEnvRef(int i)
        {
            unsafe
            {
                EnvLightData* data = (EnvLightData*)m_EnvLightDataCPUArray.GetUnsafePtr<EnvLightData>() + i;
                return ref UnsafeUtility.AsRef<EnvLightData>(data);
            }
        }

        internal void ResizeLightDataGraphicsBuffer(int numLights)
        {
            int numLightsGpu = Math.Max(numLights, 1);

            if (numLights > m_LightDataCPUArray.Length)
            {
                m_LightDataCPUArray.ResizeArray(numLights);
            }
            m_numLights = numLights;

            // If it is not null and it has already the right size, we are pretty much done
            if (m_LightDataGPUArray.count == numLightsGpu)
                return;

            // It is not the right size, free it to be reallocated
            CoreUtils.SafeRelease(m_LightDataGPUArray);

            // Allocate the next buffer buffer
            m_LightDataGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, numLightsGpu, System.Runtime.InteropServices.Marshal.SizeOf(typeof(LightData)));
        }

        internal const int k_MaxPlanarReflectionsOnScreen = 16;
        internal const int k_MaxCubeReflectionsOnScreen = 64;

        internal unsafe void SetPlanarReflectionDataRT(int index, ref Matrix4x4 vp, ref Vector4 scaleOffset)
        {
            Debug.Assert(index < k_MaxPlanarReflectionsOnScreen);

            for (int j = 0; j < 16; ++j)
                m_EnvLightReflectionDataRT._PlanarCaptureVPWL[index * 16 + j] = vp[j];

            for (int j = 0; j < 4; ++j)
                m_EnvLightReflectionDataRT._PlanarScaleOffsetWL[index * 4 + j] = scaleOffset[j];
        }

        internal unsafe void SetCubeReflectionDataRT(int index, ref Vector4 scaleOffset)
        {
            Debug.Assert(index < k_MaxCubeReflectionsOnScreen);

            for (int j = 0; j < 4; ++j)
                m_EnvLightReflectionDataRT._CubeScaleOffsetWL[index * 4 + j] = scaleOffset[j];
        }

        internal void ResizeEnvLightDataGraphicsBuffer(int numEnvLights)
        {
            int numEnvLightsGpu = Math.Max(numEnvLights, 1);

            if (numEnvLights > m_EnvLightDataCPUArray.Length)
            {
                int newSize = HDUtils.DivRoundUp(numEnvLights, WorldLightManager.SizeAlignment) * WorldLightManager.SizeAlignment;
                m_EnvLightDataCPUArray.ResizeArray(newSize);
            }
            m_numEnvLights = numEnvLights;

            if (m_EnvLightDataGPUArray.count == numEnvLightsGpu)
                return;

            CoreUtils.SafeRelease(m_EnvLightDataGPUArray);

            // Allocate the next buffer buffer
            m_EnvLightDataGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, numEnvLightsGpu, System.Runtime.InteropServices.Marshal.SizeOf(typeof(EnvLightData)));
        }

        internal void PushToGpu(CommandBuffer cmd)
        {
            if (m_numLights > 0)
                m_LightDataGPUArray.SetData(m_LightDataCPUArray, 0, 0, m_numLights);
            if (m_numEnvLights > 0)
                m_EnvLightDataGPUArray.SetData(m_EnvLightDataCPUArray, 0, 0, m_numEnvLights);
            ConstantBuffer.PushGlobal(cmd, m_EnvLightReflectionDataRT, HDShaderIDs._WorldEnvLightReflectionData);
        }

        public void Bind(CommandBuffer cmd, int lightDataShaderID, int envLightDataShaderID)
        {
            if (lightDataShaderID > 0)
                cmd.SetGlobalBuffer(lightDataShaderID, m_LightDataGPUArray);
            if (envLightDataShaderID > 0)
                cmd.SetGlobalBuffer(envLightDataShaderID, m_EnvLightDataGPUArray);
        }

        internal void Release()
        {
            CoreUtils.SafeRelease(m_LightDataGPUArray);
            m_LightDataGPUArray = null;

            CoreUtils.SafeRelease(m_EnvLightDataGPUArray);
            m_EnvLightDataGPUArray = null;

            m_LightDataCPUArray.Dispose();
            m_EnvLightDataCPUArray.Dispose();
        }
    }

    class WorldLightsVolumes
    {
        // Light Culling data
        NativeArray<WorldLightVolume> m_LightVolumesCPUArray = new NativeArray<WorldLightVolume>(WorldLightManager.SizeAlignment, Allocator.Persistent);
        GraphicsBuffer m_LightVolumeGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(WorldLightVolume)));

        NativeArray<uint> m_LightFlagsCPUArray = new NativeArray<uint>(WorldLightManager.SizeAlignment, Allocator.Persistent);
        GraphicsBuffer m_LightFlagsGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint)));

        int m_numLights = 0;

        internal ref WorldLightVolume GetRef(int i)
        {
            unsafe
            {
                WorldLightVolume* data = (WorldLightVolume*)m_LightVolumesCPUArray.GetUnsafePtr<WorldLightVolume>() + i;
                return ref UnsafeUtility.AsRef<WorldLightVolume>(data);
            }
        }

        internal ref uint GetFlagsRef(int i)
        {
            unsafe
            {
                uint* data = (uint*)m_LightFlagsCPUArray.GetUnsafePtr<uint>() + i;
                return ref UnsafeUtility.AsRef<uint>(data);
            }
        }

        internal void ResizeVolumeBuffer(int numLights)
        {
            int numLightsGpu = Math.Max(numLights, 1);

            // Always reset the bounds
            bounds.SetMinMax(WorldLightManager.minBounds, WorldLightManager.maxBounds);

            if (numLights > m_LightVolumesCPUArray.Length)
            {
                int newSize = HDUtils.DivRoundUp(numLights, WorldLightManager.SizeAlignment) * WorldLightManager.SizeAlignment;
                m_LightVolumesCPUArray.ResizeArray(newSize);
                m_LightFlagsCPUArray.ResizeArray(newSize);
            }
            m_numLights = numLights;


            // If it is not null and it has already the right size, we are pretty much done
            if (m_LightVolumeGPUArray.count == numLightsGpu)
                return;

            CoreUtils.SafeRelease(m_LightVolumeGPUArray);
            CoreUtils.SafeRelease(m_LightFlagsGPUArray);

            m_LightVolumeGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, numLightsGpu, System.Runtime.InteropServices.Marshal.SizeOf(typeof(WorldLightVolume)));
            m_LightFlagsGPUArray = new GraphicsBuffer(GraphicsBuffer.Target.Structured, numLightsGpu, System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint)));
        }

        internal void PushToGpu()
        {
            if (m_numLights > 0)
                m_LightVolumeGPUArray.SetData(m_LightVolumesCPUArray, 0, 0, m_numLights);
            if (m_numLights > 0)
                m_LightFlagsGPUArray.SetData(m_LightFlagsCPUArray, 0, 0, m_numLights);
        }

        public void Bind(CommandBuffer cmd, int lightVolumeShaderID, int lightFlagsShaderID)
        {
            if (lightVolumeShaderID > 0)
                cmd.SetGlobalBuffer(lightVolumeShaderID, m_LightVolumeGPUArray);
            if (lightFlagsShaderID > 0)
                cmd.SetGlobalBuffer(lightFlagsShaderID, m_LightFlagsGPUArray);
        }

        internal void Release()
        {
            m_LightVolumesCPUArray.Dispose();
            m_LightFlagsCPUArray.Dispose();

            CoreUtils.SafeRelease(m_LightVolumeGPUArray);
            m_LightVolumeGPUArray = null;
            CoreUtils.SafeRelease(m_LightFlagsGPUArray);
            m_LightFlagsGPUArray = null;
        }

        public GraphicsBuffer GetBuffer()
        {
            return m_LightVolumeGPUArray;
        }

        public int GetCount()
        {
            return m_numLights;
        }

        public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
    }

    class WorldLightManager
    {
        public static void CollectWorldLights(in HDCamera hdCamera, in WorldLightsSettings settings, in Func<HDCamera, HDAdditionalLightData, Light, uint> flagFunc, WorldLights worldLights)
        {
            // Refresh the entire structure every frame for now
            worldLights.Reset();

            if (!settings.enabled)
                return;

            using var profilerScope = k_ProfilerMarkerCollect.Auto();

            // fetch all the lights in the scene
            HDLightRenderDatabase lightEntities = HDLightRenderDatabase.instance;
            for (int lightIdx = 0; lightIdx < lightEntities.lightCount; ++lightIdx)
            {
                HDLightRenderEntity lightRenderEntity = lightEntities.lightEntities[lightIdx];
                HDAdditionalLightData hdLight = lightEntities.hdAdditionalLightData[lightIdx];

                if (hdLight != null && hdLight.enabled && hdLight != HDUtils.s_DefaultHDAdditionalLightData)
                {
                    Light light = hdLight.gameObject.GetComponent<Light>();
                    // If the light is null or disabled, skip it
                    if (light == null || !light.enabled)
                        continue;

                    // TODO-WL: Here we filter out all light that doesn't have a flag set
                    //          this to ensure that we don't process more lights than before
                    if ((flagFunc(hdCamera, hdLight, light) & 0xfffffffe) == 0)
                        continue;

                    switch (hdLight.legacyLight.type)
                    {
                        case LightType.Directional:
                            // TODO-WL: Directional lights
                            break;
                        case LightType.Point:
                        case LightType.Spot:
                        case LightType.Pyramid:
                        case LightType.Box:
                            worldLights.hdPointLightArray.Add(lightRenderEntity);
                            break;
                        case LightType.Rectangle:
                            worldLights.hdRectLightArray.Add(lightRenderEntity);
                            break;
                        case LightType.Tube:
                            worldLights.hdLineLightArray.Add(lightRenderEntity);
                            break;
                        case LightType.Disc:
                            worldLights.hdDiscLightArray.Add(lightRenderEntity);
                            break;
                    }
                }
            }

            // Add the lights to the structure
            worldLights.hdLightEntityArray.AddRange(worldLights.hdPointLightArray.AsArray());
            worldLights.hdLightEntityArray.AddRange(worldLights.hdLineLightArray.AsArray());
            worldLights.hdLightEntityArray.AddRange(worldLights.hdRectLightArray.AsArray());
            worldLights.hdLightEntityArray.AddRange(worldLights.hdDiscLightArray.AsArray());

            // Process the lights
            HDAdditionalReflectionData[] reflectionProbeArray = UnityEngine.GameObject.FindObjectsByType<HDAdditionalReflectionData>(FindObjectsSortMode.None);
            for (int reflIdx = 0; reflIdx < reflectionProbeArray.Length; ++reflIdx)
            {
                HDAdditionalReflectionData reflectionProbe = reflectionProbeArray[reflIdx];

                // Add it to the list if enabled
                // Skip the probe if the probe has never rendered (in real time cases) or if texture is null
                if (reflectionProbe != null
                    && reflectionProbe.enabled
                    && reflectionProbe.ReflectionProbeIsEnabled()
                    && reflectionProbe.gameObject.activeSelf
                    && reflectionProbe.HasValidRenderedData())
                {
                    worldLights.reflectionProbeArray.Add(reflectionProbe);
                }
            }

            // Decals
            worldLights.decalCount = DecalSystem.GetDecalCount(hdCamera);
        }

        public static void BuildWorldLightDatas(CommandBuffer cmd, in HDCamera hdCamera, in HDRenderPipeline renderPipeline, in WorldLights worldLights, WorldLightsGpu worldLightsGpu)
        {
            using var profilerScope = k_ProfilerMarkerBuild.Auto();

            // Clear gpu data
            worldLightsGpu.ResizeLightDataGraphicsBuffer(worldLights.normalLightCount);
            worldLightsGpu.ResizeEnvLightDataGraphicsBuffer(worldLights.envLightCount);

            #region Normal lights
            if (worldLights.normalLightCount > 0)
            {
                // Grab the shadow settings
                var hdShadowSettings = hdCamera.volumeStack.GetComponent<HDShadowSettings>();

                // Build the data for every light
                HDLightRenderDatabase lightEntities = HDLightRenderDatabase.instance;
                var processedLightEntity = new HDProcessedVisibleLight()
                {
                    shadowMapFlags = HDProcessedVisibleLightsBuilder.ShadowMapFlags.None
                };

                var globalConfig = HDGpuLightsBuilder.CreateGpuLightDataJobGlobalConfig.Create(hdCamera, hdShadowSettings);
                var shadowInitParams = renderPipeline.currentPlatformRenderPipelineSettings.hdShadowInitParams;

                for (int lightIdx = 0; lightIdx < worldLights.hdLightEntityArray.Length; ++lightIdx)
                {
                    // Grab the additinal light data to process
                    int dataIndex = lightEntities.GetEntityDataIndex(worldLights.hdLightEntityArray[lightIdx]);
                    HDAdditionalLightData additionalLightData = lightEntities.hdAdditionalLightData[dataIndex];

                    ref LightData lightData = ref worldLightsGpu.GetRef(lightIdx);

                    lightData = s_defaultLightData;

                    // When the user deletes a light source in the editor, there is a single frame where the light is null before the collection of light in the scene is triggered
                    // the workaround for this is simply to add an invalid light for that frame
                    if (additionalLightData == null)
                    {
                        continue;
                    }

                    // Evaluate all the light type data that we need
                    LightCategory lightCategory = LightCategory.Count;
                    GPULightType gpuLightType = GPULightType.Point;
                    LightVolumeType lightVolumeType = LightVolumeType.Count;
                    LightType lightType = additionalLightData.legacyLight.type;
                    HDRenderPipeline.EvaluateGPULightType(lightType, ref lightCategory, ref gpuLightType, ref lightVolumeType);

                    // Fetch the light component for this light
                    Light lightComponent;
                    additionalLightData.gameObject.TryGetComponent(out lightComponent);

                    ref HDLightRenderData lightRenderData = ref lightEntities.GetLightDataAsRef(dataIndex);

                    // Build the processed light data  that we need
                    processedLightEntity.dataIndex = dataIndex;
                    processedLightEntity.gpuLightType = gpuLightType;
                    processedLightEntity.lightType = lightType;
                    processedLightEntity.distanceToCamera = (additionalLightData.transform.position - hdCamera.camera.transform.position).magnitude;
                    processedLightEntity.lightDistanceFade = HDUtils.ComputeLinearDistanceFade(processedLightEntity.distanceToCamera, lightRenderData.fadeDistance);
                    processedLightEntity.lightVolumetricDistanceFade = HDUtils.ComputeLinearDistanceFade(processedLightEntity.distanceToCamera, lightRenderData.volumetricFadeDistance);
                    processedLightEntity.isBakedShadowMask = HDRenderPipeline.IsBakedShadowMaskLight(lightComponent);

                    // Build a visible light
                    VisibleLight visibleLight = new VisibleLight();
                    visibleLight.finalColor = additionalLightData.EvaluateLightColor();
                    visibleLight.range = lightComponent.range;
                    // This should be done explicitly, localToWorld matrix doesn't work here
                    Matrix4x4 localToWorldMatrix = new Matrix4x4();
                    localToWorldMatrix.SetColumn(3, lightComponent.gameObject.transform.position);
                    localToWorldMatrix.SetColumn(2, lightComponent.transform.forward);
                    localToWorldMatrix.SetColumn(1, lightComponent.transform.up);
                    localToWorldMatrix.SetColumn(0, lightComponent.transform.right);
                    visibleLight.localToWorldMatrix = localToWorldMatrix;
                    visibleLight.spotAngle = lightComponent.spotAngle;

                    int shadowIndex = additionalLightData.shadowIndex;
                    Vector3 lightDimensions = new Vector3(0.0f, 0.0f, 0.0f);

                    // Use the shared code to build the light data
                    HDGpuLightsBuilder.CreateGpuLightDataJob.ConvertLightToGPUFormat(
                        lightCategory, gpuLightType, globalConfig,
                        lightComponent.lightShadowCasterMode, lightComponent.bakingOutput,
                        visibleLight, processedLightEntity, lightRenderData, out var _, ref lightData);
                    renderPipeline.gpuLightList.ProcessLightDataShadowIndex(cmd, shadowInitParams, lightType, lightComponent, additionalLightData, shadowIndex, ref lightData);

                    // We make the light position camera-relative as late as possible in order
                    // to allow the preceding code to work with the absolute world space coordinates.
                    Vector3 camPosWS = hdCamera.mainViewConstants.worldSpaceCameraPos;
                    HDRenderPipeline.UpdateLightCameraRelativetData(ref lightData, camPosWS);
                }
             }

            #endregion

            #region Env lights
            if (worldLights.envLightCount > 0)
            {
                ProcessedProbeData processedProbe = new ProcessedProbeData();

                int fetchIndex;
                Vector4 scaleOffset;
                Matrix4x4 vp;

                // Build the data for every light
                for (int lightIdx = 0; lightIdx < worldLights.reflectionProbeArray.Count; ++lightIdx)
                {
                    ref EnvLightData envLightData = ref worldLightsGpu.GetEnvRef(lightIdx);

                    envLightData = s_defaultEnvLightData;

                    HDProbe probeData = worldLights.reflectionProbeArray[lightIdx];

                    HDRenderPipeline.PreprocessProbeData(ref processedProbe, probeData, hdCamera);
                    renderPipeline.GetEnvLightData(cmd, hdCamera, processedProbe, ref envLightData, out fetchIndex, out scaleOffset, out vp);

                    switch (processedProbe.hdProbe)
                    {
                        case PlanarReflectionProbe planarProbe:
                            worldLightsGpu.SetPlanarReflectionDataRT(fetchIndex, ref vp, ref scaleOffset);
                            break;
                        case HDAdditionalReflectionData reflectionData:
                            worldLightsGpu.SetCubeReflectionDataRT(fetchIndex, ref scaleOffset);
                            break;
                    };

                    // We make the light position camera-relative as late as possible in order
                    // to allow the preceding code to work with the absolute world space coordinates.
                    Vector3 camPosWS = hdCamera.mainViewConstants.worldSpaceCameraPos;
                    HDRenderPipeline.UpdateEnvLighCameraRelativetData(ref envLightData, camPosWS);
                }
            }
            #endregion

            worldLightsGpu.PushToGpu(cmd);
        }

        public static void BuildWorldLightVolumes(in HDCamera hdCamera, in HDRenderPipeline renderPipeline, in WorldLights worldLights, in Func<HDCamera, HDAdditionalLightData, Light, uint> flagFunc, WorldLightsVolumes worldLightsVolumes)
        {
            int totalNumLights = worldLights.totalLighttCount;

            worldLightsVolumes.ResizeVolumeBuffer(totalNumLights);

            if (totalNumLights < 1)
                return;

            using var profilerScope = k_ProfilerMarkerVolume.Auto();

            int realIndex = 0;
            HDLightRenderDatabase lightEntities = HDLightRenderDatabase.instance;
            for (int lightIdx = 0; lightIdx < worldLights.hdLightEntityArray.Length; ++lightIdx)
            {
                int dataIndex = lightEntities.GetEntityDataIndex(worldLights.hdLightEntityArray[lightIdx]);
                HDAdditionalLightData currentLight = lightEntities.hdAdditionalLightData[dataIndex];

                // When the user deletes a light source in the editor, there is a single frame where the light is null before the collection of light in the scene is triggered
                // the workaround for this is simply to not add it if it is null for that invalid frame
                if (currentLight != null)
                {
                    // The light is guaranteed to be there since it was checked when gathering the light list
                    Light light = currentLight.gameObject.GetComponent<Light>();

                    var lightType = currentLight.legacyLight.type;
                    // Reserve space in the cookie atlas
                    renderPipeline.ReserveCookieAtlasTexture(currentLight, light, lightType);

                    // Compute the camera relative position
                    Vector3 lightPositionRWS = currentLight.gameObject.transform.position;
                    if (ShaderConfig.s_CameraRelativeRendering != 0)
                    {
                        lightPositionRWS -= hdCamera.camera.transform.position;
                    }

                    // Grab the light range
                    float lightRange = light.range;

                    // User defined flags
                    ref uint lightFlags = ref worldLightsVolumes.GetFlagsRef(realIndex);
                    lightFlags = flagFunc(hdCamera, currentLight, light) | (currentLight.gameObject.activeInHierarchy ? 1u : 0u);

                    ref WorldLightVolume lightVolume = ref worldLightsVolumes.GetRef(realIndex);

                    // Common volume data
                    lightVolume.flags = lightFlags;
                    lightVolume.lightIndex = (uint)lightIdx;

                    bool isAreaLight = lightType.IsArea();
                    bool isBoxLight = lightType == LightType.Box;

                    if (!isAreaLight && !isBoxLight)
                    {
                        lightVolume.range = new Vector3(lightRange, lightRange, lightRange);
                        lightVolume.position = lightPositionRWS;
                        lightVolume.shape = 0;
                        lightVolume.lightType = 0;

                        worldLightsVolumes.bounds.Encapsulate(new Bounds(lightPositionRWS, lightVolume.range));
                    }
                    // Area lights and box spot lights require AABB intersection data
                    else
                    {
                        // let's compute the oobb of the light influence volume first
                        Vector3 oobbDimensions = new Vector3(currentLight.shapeWidth + 2 * lightRange, currentLight.shapeHeight + 2 * lightRange, lightRange); // One-sided
                        Vector3 extents = 0.5f * oobbDimensions;
                        Vector3 oobbCenter = lightPositionRWS;

                        // Tube lights don't have forward / backward facing and have full extents on all directions as a consequence, since their OOBB is centered
                        if (lightType == LightType.Tube)
                        {
                            oobbDimensions.z *= 2;
                            extents.z *= 2;
                        }
                        else
                        {
                            oobbCenter += extents.z * currentLight.gameObject.transform.forward;
                        }

                        // Let's now compute an AABB that matches the previously defined OOBB
                        Bounds bounds = new Bounds();
                        OOBBToAABBBounds(oobbCenter, extents, currentLight.gameObject.transform.up, currentLight.gameObject.transform.right, currentLight.gameObject.transform.forward, ref bounds);

                        worldLightsVolumes.bounds.Encapsulate(bounds);

                        // Fill the volume data
                        lightVolume.range = bounds.extents;
                        lightVolume.position = bounds.center;
                        lightVolume.shape = 1;
                        lightVolume.lightType = 1;

                        if (isAreaLight)
                        {
                            lightVolume.lightType = 1;
                        }
                        else
                        {
                            lightVolume.lightType = 0;
                        }
                    }
                    realIndex++;
                }
            }

            // Set Env Light volume data to the CPU buffer
            for (int lightIdx = 0; lightIdx < worldLights.reflectionProbeArray.Count; ++lightIdx)
            {
                HDProbe currentEnvLight = worldLights.reflectionProbeArray[lightIdx];

                // Compute the camera relative position
                Vector3 probePositionRWS = currentEnvLight.influenceToWorld.GetColumn(3);
                if (ShaderConfig.s_CameraRelativeRendering != 0)
                {
                    probePositionRWS -= hdCamera.camera.transform.position;
                }

                ref WorldLightVolume lightVolume = ref worldLightsVolumes.GetRef(realIndex);

                if (currentEnvLight.influenceVolume.shape == InfluenceShape.Sphere)
                {
                    lightVolume.shape = 0;
                    lightVolume.range = new Vector3(currentEnvLight.influenceVolume.sphereRadius, currentEnvLight.influenceVolume.sphereRadius, currentEnvLight.influenceVolume.sphereRadius);
                    lightVolume.position = probePositionRWS;
                }
                else
                {
                    lightVolume.shape = 1;
                    lightVolume.range = new Vector3(currentEnvLight.influenceVolume.boxSize.x / 2.0f, currentEnvLight.influenceVolume.boxSize.y / 2.0f, currentEnvLight.influenceVolume.boxSize.z / 2.0f);
                    lightVolume.position = probePositionRWS;
                }

                // User defined flags
                // TODO-WL: Hard coded flags for Env lights
                ref uint lightFlags = ref worldLightsVolumes.GetFlagsRef(realIndex);
                lightFlags = lightVolume.flags = (uint)WorldLightFlags.Raytracing | (currentEnvLight.gameObject.activeInHierarchy ? 1u : 0u);
                lightVolume.lightIndex = (uint)lightIdx;
                lightVolume.lightType = 2;

                worldLightsVolumes.bounds.Encapsulate(new Bounds(probePositionRWS, lightVolume.range));
                realIndex++;
            }

            // Add Decal data to m_lightVolumesCPUArray
            for (int decalIdx = 0; decalIdx < worldLights.decalCount; ++decalIdx)
            {
                ref WorldLightVolume lightVolume = ref worldLightsVolumes.GetRef(realIndex);
                // Decal projectors are box shaped
                lightVolume.shape = 1;

                // Compute the camera relative position
                Vector3 decalPositionRWS = DecalSystem.instance.GetClusteredDecalPosition(decalIdx);
                if (ShaderConfig.s_CameraRelativeRendering != 0)
                {
                    decalPositionRWS -= hdCamera.camera.transform.position;
                }
                lightVolume.position = decalPositionRWS;
                lightVolume.range = DecalSystem.instance.GetClusteredDecalRange(decalIdx);
                lightVolume.lightIndex = (uint)decalIdx;
                lightVolume.lightType = 3;
                // TODO-WL: Hard coded flags for Decals
                ref uint lightFlags = ref worldLightsVolumes.GetFlagsRef(realIndex);
                lightFlags = lightVolume.flags = (uint)WorldLightFlags.Raytracing |(uint)WorldLightFlags.Pathtracing | (uint)WorldLightFlags.Active;

                realIndex++;
            }

            worldLightsVolumes.PushToGpu();
        }

        static readonly ProfilerMarker k_ProfilerMarkerCollect = new ("WorldLightManager.Collect");
        static readonly ProfilerMarker k_ProfilerMarkerBuild = new ("WorldLightManager.Build");
        static readonly ProfilerMarker k_ProfilerMarkerVolume = new ("WorldLightManager.Volumes");
        internal const int SizeAlignment = 32;
        internal static Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        internal static Vector3 maxBounds = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);
        internal static LightData s_defaultLightData = new LightData();
        internal static EnvLightData s_defaultEnvLightData = new EnvLightData();

        private static void OOBBToAABBBounds(Vector3 centerWS, Vector3 extents, Vector3 up, Vector3 right, Vector3 forward, ref Bounds bounds)
        {
            // Reset the bounds of the AABB
            bounds.min = minBounds;
            bounds.max = maxBounds;

            // Push the 8 corners of the oobb into the AABB
            bounds.Encapsulate(centerWS + right * extents.x + up * extents.y + forward * extents.z);
            bounds.Encapsulate(centerWS + right * extents.x + up * extents.y - forward * extents.z);
            bounds.Encapsulate(centerWS + right * extents.x - up * extents.y + forward * extents.z);
            bounds.Encapsulate(centerWS + right * extents.x - up * extents.y - forward * extents.z);
            bounds.Encapsulate(centerWS - right * extents.x + up * extents.y + forward * extents.z);
            bounds.Encapsulate(centerWS - right * extents.x + up * extents.y - forward * extents.z);
            bounds.Encapsulate(centerWS - right * extents.x - up * extents.y + forward * extents.z);
            bounds.Encapsulate(centerWS - right * extents.x - up * extents.y - forward * extents.z);
        }
    }
} // namespace UnityEngine.Rendering.HighDefinition
