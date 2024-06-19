#ifndef GLOBAL_RESOURCES_INCLUDED
#define GLOBAL_RESOURCES_INCLUDED

#if (RNG_METHOD == BLUE_NOISE)
Texture2D<float>                _ScramblingTileXSPP;
Texture2D<float>                _RankingTileXSPP;
Texture2D<float2>               _ScramblingTexture;
Texture2D<float2>               _OwenScrambledTexture;
#endif
#if (RNG_METHOD == SOBOL)
StructuredBuffer<uint>          _SobolBuffer;
StructuredBuffer<float>         _CPRBuffer;
#endif

#endif

