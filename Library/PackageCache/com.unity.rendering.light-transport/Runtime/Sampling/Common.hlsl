#ifndef _SAMPLING_COMMON_H_
#define _SAMPLING_COMMON_H_

#ifndef PI
#define PI 3.141592653589f
#endif

void OrthoBasisFromVector(float3 n, out float3 b1, out float3 b2)
{
    if (n.z < -0.9999999) // Handle the singularity
    {
        b1 = float3(0.0, -1.0, 0.0);
        b2 = float3(-1.0, 0.0, 0.0);
    }
    else
    {
        float a = 1.0 / (1.0 + n.z);
        float b = -n.x * n.y * a;
        b1 = float3(1.0 - n.x * n.x * a, b, -n.x);
        b2 = float3(b, 1.0 - n.y * n.y * a, -n.y);    
    }
}

float3x3 OrthoBasisFromVector(float3 n)
{
    float3 t, b;
    OrthoBasisFromVector(n, t, b);

    return transpose(float3x3(t, b, n));
}

void SampleDiffuseBrdf(float2 u, float3 shadingNormal, out float3 wi)
{
    wi = float3(0, 0, 0);

    float a = sqrt(u.x);
    float b = 2.0 * PI * u.y;
    float3 localWi = float3(a * cos(b), a * sin(b), sqrt(1.0f - u.x));

    float3x3 TBN = OrthoBasisFromVector(shadingNormal);
    wi = mul(TBN, localWi);
}

bool SampleDiffuseBrdf(float2 u, float3 geometryNormal, float3 shadingNormal, float3 wo, out float3 wi)
{
    if (dot(geometryNormal, wo) <= 0.0f)
    {
        wi = float3(0, 0, 0);
        return false;
    }
    SampleDiffuseBrdf(u, shadingNormal, wi);
    return true;
}

float3 SphereSample(float2 rnd)
{
    float ct = 1.0f - 2.0f * rnd.y;
    float st = sqrt(1.0f - ct * ct);

    float phi = 2.0f*PI * rnd.x;
    float cp = cos(phi);
    float sp = sin(phi);

    return normalize(float3(cp * st, sp * st, ct));
}

#endif
