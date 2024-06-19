#ifndef UNITY_SOBOL_SAMPLING_INCLUDED
#define UNITY_SOBOL_SAMPLING_INCLUDED

#define SOBOL_MATRIX_SIZE 52
#define SOBOL_MATRICES_COUNT 1024
#define CranleyPattersonRotationSize 512
#define CranleyPattersonRotationMaxDimension 12

#include "GlobalResources.hlsl"


float ApplyCranleyPattersonRotation(float rnd, uint2 pixelCoord, uint dimension)
{
    pixelCoord = uint2(pixelCoord.x % CranleyPattersonRotationSize, pixelCoord.y % CranleyPattersonRotationSize);
    dimension = dimension % CranleyPattersonRotationMaxDimension;

    float cpShift = _CPRBuffer[(pixelCoord.x + pixelCoord.y * CranleyPattersonRotationSize) * CranleyPattersonRotationMaxDimension + dimension];
    return frac(rnd + cpShift);
}

float SobolSample(uint index, int dimension)
{
    uint result = 0;
    for (uint i = dimension * SOBOL_MATRIX_SIZE; index; index >>= 1, ++i)
    {
        if (index & 1)
            result ^= _SobolBuffer[i];
    }
    return result * 2.3283064365386963e-10; // (1.f / (1ULL << 32));
}

float GetSobolSequenceSample(uint2 pixelCoord, uint sampleIndex, uint sampleDimension)
{
    float rnd = SobolSample(sampleIndex, sampleDimension);
    return ApplyCranleyPattersonRotation(rnd, pixelCoord, sampleDimension);
}

#endif // UNITY_SOBOL_SAMPLING_INCLUDED
