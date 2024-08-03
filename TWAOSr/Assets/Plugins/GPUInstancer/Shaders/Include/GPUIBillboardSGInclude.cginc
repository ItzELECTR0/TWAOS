#ifndef GPU_INSTANCER_BILLBOARD_SG_INCLUDED
#define GPU_INSTANCER_BILLBOARD_SG_INCLUDED

#define GPUI_PI            3.14159265359f
#define GPUI_TWO_PI        6.28318530718f

#ifdef unity_WorldToObject
#undef unity_WorldToObject
#endif
#ifdef unity_ObjectToWorld
#undef unity_ObjectToWorld
#endif

void GPUIBillboardVertexSG_float(in float3 vertex, in float frameCount, out float3 vertex_out, out float3 normal_out, out float3 tangent_out)
{
    float3 billboardCameraDir = normalize(mul(((float3x3) unity_WorldToObject), _WorldSpaceCameraPos.xyz - unity_ObjectToWorld._m03_m13_m23));

    // calculate camera vectors
    float3 up = float3(0, 1, 0);
    float3 forward = -normalize(UNITY_MATRIX_V._m20_m21_m22);

#ifdef _BILLBOARDFACECAMPOS_ON
    float3 right = normalize(cross(float3(0, 1, 0), unity_ObjectToWorld._m03_m13_m23 - _WorldSpaceCameraPos));
#else
    float3 right = normalize(UNITY_MATRIX_V._m00_m01_m02);
	    // adjust rotation matrix if camera is upside down
    right *= sign(normalize(UNITY_MATRIX_V._m10_m11_m12).y);
#endif	

	// create camera rotation matrix
    float4x4 rotationMatrix = float4x4(right, 0, up, 0, forward, 0, 0, 0, 0, 1);
    
	// rotate to camera lookAt
    vertex_out.x = vertex.x * length(unity_ObjectToWorld._m00_m10_m20);
    vertex_out.y = vertex.y * length(unity_ObjectToWorld._m01_m11_m21);
    vertex_out.z = vertex.z * length(unity_ObjectToWorld._m02_m12_m22);
    vertex_out = (mul(float4(vertex_out, 1), rotationMatrix)).xyz;
			
	// account for world position
    vertex_out.xyz += unity_ObjectToWorld._m03_m13_m23;
			
	// ignore initial object rotation for the billboard
    vertex_out = (mul(unity_WorldToObject, float4(vertex_out, 1))).xyz;
    vertex_out.y += (1 / frameCount * unity_ObjectToWorld[3].y + unity_ObjectToWorld[3].z) * length(unity_ObjectToWorld._m01_m11_m21);
    
    // adjust normal and tangents
    normal_out = -billboardCameraDir;
    tangent_out = normalize(cross(up, normal_out));
}

void GPUIBillboardAtlasUVSG_float(in float4 texcoord, in float frameCount, out float2 atlasUV)
{
    float3 billboardCameraDir = normalize(mul(((float3x3) unity_WorldToObject), _WorldSpaceCameraPos.xyz - unity_ObjectToWorld._m03_m13_m23));

    // get current camera angle in radians
    float angle = atan2(-billboardCameraDir.z, billboardCameraDir.x);
    
    // calculate current frame index and set uvs
    float frameIndex = round((angle - GPUI_PI / 2) / (GPUI_TWO_PI / frameCount));
    atlasUV = texcoord.xy * float2(1.0 / frameCount, 1) + float2((1.0 / frameCount) * frameIndex, 0);
}

void GPUIBillboardFragmentSG_float(in float4 albedoTexture, in float4 normalTexture, in float normalStrength, in float3 normalWorld, in float3 tangentWorld, out float3 albedo_out, out float3 normal_out, out float depth_out)
{
    albedo_out = albedoTexture.rgb;
    
    // remap normalTexture back to [-1, 1]
    float3 unpackedNormalTexture = lerp(float3(0, 1, 0), normalTexture.xyz, normalStrength);
    float3 bitangentWorld = normalize(cross(normalWorld, tangentWorld) * (-unity_WorldTransformParams.w));
    // modify normal map with the billboard world vectors
    normal_out = normalize(mul(float3x3(tangentWorld, bitangentWorld, normalWorld), unpackedNormalTexture));
    
    // calculate depth
    depth_out = max(normalTexture.w - 0.35, 0);
}

#endif // GPU_INSTANCER_BILLBOARD_SG_INCLUDED