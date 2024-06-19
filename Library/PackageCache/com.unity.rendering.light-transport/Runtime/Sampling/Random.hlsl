#ifndef _RANDOM_HLSL_
#define _RANDOM_HLSL_

#define BLUE_NOISE  0
#define WANG_HASH   1
#define XOR_SHIFT   2
#define PCG_4D      3
#define R2          4 // http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/
#define SOBOL       5

// Should be define by the file that includes Random.hlsl
#ifndef RNG_METHOD
#define RNG_METHOD 4
#endif

#if (RNG_METHOD == BLUE_NOISE)
#include "BluenoiseSampling.hlsl"
#endif
#if (RNG_METHOD == SOBOL)
#include "SobolSampling.hlsl"
#endif

// The number of dimensions used per bounce
// Should be define by the file that includes Random.hlsl
//#define RAND_SAMPLES_PER_BOUNCE
#define RAND_OFFSET 0            // global dimension offset (could be used to alter the noise pattern)

#if (RNG_METHOD == BLUE_NOISE) || (RNG_METHOD == PCG_4D) || (RNG_METHOD == SOBOL)
    #define RngStateType uint4
#elif (RNG_METHOD == R2)
#define R2_ROTATION_SIZE 65536 // Chosen empirically. Too low and the set of possible samples gets too small. Too high and you get numerical precision issues.

struct RngStateType
{
    float noise;
    uint rotatedSampleIdx;
};
#else
    #define RngStateType uint
#endif

// 32-bit Xorshift random number generator
uint xorshift(inout uint rngState)
{
    rngState ^= rngState << 13;
    rngState ^= rngState >> 17;
    rngState ^= rngState << 5;
    return rngState;
}

// Jenkins's "one at a time" hash function
uint jenkinsHash(uint x) {
    x += x << 10;
    x ^= x >> 6;
    x += x << 3;
    x ^= x >> 11;
    x += x << 15;
    return x;
}

// PCG random numbers generator
// Source: "Hash Functions for GPU Rendering" by Jarzynski & Olano
uint4 pcg4d(uint4 v)
{
    v = v * 1664525u + 1013904223u;

    v.x += v.y * v.w;
    v.y += v.z * v.x;
    v.z += v.x * v.y;
    v.w += v.y * v.z;

    v = v ^ (v >> 16u);

    v.x += v.y * v.w;
    v.y += v.z * v.x;
    v.z += v.x * v.y;
    v.w += v.y * v.z;

    return v;
}

// Converts unsigned integer into float int range <0; 1) by using 23 most significant bits for mantissa
float uintToFloat(uint x) {
    return asfloat(0x3f800000 | (x >> 9)) - 1.0f;
}

uint WangHash(inout uint seed)
{
    seed = (seed ^ 61) ^ (seed >> 16);
    seed *= 9;
    seed = seed ^ (seed >> 4);
    seed *= 0x27d4eb2d;
    seed = seed ^ (seed >> 15);
    return seed;
}

RngStateType InitRNG(uint2 launchIndex, uint frameIndex, uint sampleCount)
{
    const uint startSampleIndex = frameIndex * sampleCount;
#if (RNG_METHOD == BLUE_NOISE)
    return RngStateType(launchIndex, startSampleIndex, 0);
#elif (RNG_METHOD == WANG_HASH)
    // Initial random number generator seed for this pixel. The rngState will change every time we draw a random number.
    return uint(uint(launchIndex.x) * uint(1973) + uint(launchIndex.y) * uint(9277) + uint(startSampleIndex) * uint(26699)) | uint(1);
#elif (RNG_METHOD == XOR_SHIFT)
    RngStateType seed = dot(launchIndex, uint2(1, 1280)) ^ jenkinsHash(startSampleIndex);
    return jenkinsHash(seed);
#elif (RNG_METHOD == PCG_4D)
    //< Seed for PCG uses a sequential sample number in 4th channel, which increments on every RNG call and starts from 0
    return RngStateType(launchIndex, startSampleIndex, 0);
#elif (RNG_METHOD == R2)
    RngStateType state;
    const uint scrambledIndex = jenkinsHash(launchIndex.x * 9277 + launchIndex.y);
    state.noise = uintToFloat(scrambledIndex);
    state.rotatedSampleIdx = (startSampleIndex + scrambledIndex) % R2_ROTATION_SIZE;
    return state;
#elif (RNG_METHOD == SOBOL)
    return RngStateType(launchIndex, startSampleIndex, 0);
#endif
}

void NextSampleRNG(inout RngStateType state)
{
    #if (RNG_METHOD == BLUE_NOISE)
    state.z += 1;
    #elif (RNG_METHOD == PCG_4D)
    state.z += 1;
    #elif (RNG_METHOD == R2)
    state.rotatedSampleIdx = (state.rotatedSampleIdx + 1) % R2_ROTATION_SIZE;
    #elif (RNG_METHOD == SOBOL)
    state.z += 1;
    #endif
}

float RandomFloat01RNG(inout RngStateType state, uint dimension)
{
#if (RNG_METHOD == BLUE_NOISE)
    // If we go past the number of stored samples per dim, just shift all to the next pair of dimensions
    dimension += (state.z / 256) * 2;
    return GetBNDSequenceSample(state.xy, state.z, dimension );
#elif (RNG_METHOD == WANG_HASH)
    return float(WangHash(state)) / float(0xFFFFFFFF);
#elif (RNG_METHOD == XOR_SHIFT)
    return uintToFloat(xorshift(state));
#elif (RNG_METHOD == PCG_4D)
    state.w++; //< Increment sample index
    return uintToFloat(pcg4d(state).x);
#elif (RNG_METHOD == R2)
    // Using an 8 dimensional variant of the R2 sequence.
    // 8 was chosen empirically.
    // See http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/.
    const float phi8 = 1.085070245491771;
    const float as[]= {
        1.0f / phi8,
        1.0f / (phi8 * phi8),
        1.0f / (phi8 * phi8 * phi8),
        1.0f / (phi8 * phi8 * phi8 * phi8),
        1.0f / (phi8 * phi8 * phi8 * phi8 * phi8),
        1.0f / (phi8 * phi8 * phi8 * phi8 * phi8 * phi8),
        1.0f / (phi8 * phi8 * phi8 * phi8 * phi8 * phi8 * phi8),
        1.0f / (phi8 * phi8 * phi8 * phi8 * phi8 * phi8 * phi8 * phi8)
    };
    return frac(state.noise + as[dimension % 8] * float(state.rotatedSampleIdx));
#elif (RNG_METHOD == SOBOL)
    dimension = dimension % SOBOL_MATRICES_COUNT;
    float rand = GetSobolSequenceSample(state.xy, state.z, dimension);
    return rand;
#endif
}

struct RngState
{
    RngStateType state;
    int bounceIndex;

    void Init(uint2 launchIndex, uint frameIndex, uint sampleCount)
    {
        state = InitRNG(launchIndex, frameIndex, sampleCount);
        bounceIndex = 0;
    }

    float GetFloatSample(uint dimension)
    {
        uint actualDimension = RAND_OFFSET + RAND_SAMPLES_PER_BOUNCE * bounceIndex + dimension;
        return RandomFloat01RNG(state, actualDimension);
    }

    void NextSample()
    {
        NextSampleRNG(state);
    }

    void NextBounce()
    {
        bounceIndex++;
    }
};

#endif
