using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal struct TerrainDesc
    {
        public Terrain terrain;
        public Matrix4x4 localToWorldMatrix;
        public uint mask;
        public uint instanceID;
        public uint materialID;
        public bool enableTriangleCulling;
        public bool frontTriangleCounterClockwise;

        public TerrainDesc(Terrain terrain)
        {
            this.terrain = terrain;
            localToWorldMatrix = Matrix4x4.identity;
            mask = 0xFFFFFFFF;
            instanceID = 0;
            materialID = 0;
            enableTriangleCulling = true;
            frontTriangleCounterClockwise = false;
        }
    }

    internal static class TerrainAccelStructExtensions
    {
        public static List<int> AddTerrain(this IRayTracingAccelStruct accelStruct, TerrainDesc terrainInstance)
        {
            List<int> instanceHandles = new List<int>();

            AddHeightmap(accelStruct, terrainInstance, ref instanceHandles);
            AddTrees(accelStruct, terrainInstance, ref instanceHandles);

            return instanceHandles;
        }

        static void AddHeightmap(IRayTracingAccelStruct accelStruct, TerrainDesc terrainInstance, ref List<int> instanceHandles)
        {
            var terrainMesh = TerrainToMesh.Convert(terrainInstance.terrain);
            var instanceDesc = new MeshInstanceDesc(terrainMesh);
            instanceDesc.localToWorldMatrix = terrainInstance.localToWorldMatrix;
            instanceDesc.mask = terrainInstance.mask;
            instanceDesc.instanceID = terrainInstance.instanceID;
            instanceDesc.materialID = terrainInstance.materialID;
            instanceDesc.enableTriangleCulling = terrainInstance.enableTriangleCulling;
            instanceDesc.frontTriangleCounterClockwise = terrainInstance.frontTriangleCounterClockwise;

            instanceHandles.Add(accelStruct.AddInstance(instanceDesc));
        }

        static void AddTrees(IRayTracingAccelStruct accelStruct, TerrainDesc terrainInstance, ref List<int> instanceHandles)
        {
            TerrainData terrainData = terrainInstance.terrain.terrainData;
            float4x4 terrainLocalToWorld = terrainInstance.localToWorldMatrix;
            float3 positionScale = new float3((float)terrainData.heightmapResolution, 1.0f, (float)terrainData.heightmapResolution)* terrainData.heightmapScale;
            float3 positionOffset = new float3(terrainLocalToWorld[3].x, terrainLocalToWorld[3].y, terrainLocalToWorld[3].z);

            foreach (var treeInstance in terrainData.treeInstances)
            {
                var localToWorld = Matrix4x4.TRS(
                    positionOffset + new float3(treeInstance.position) * positionScale,
                    Quaternion.AngleAxis(treeInstance.rotation, Vector3.up),
                    new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale));

                var prefab = terrainData.treePrototypes[treeInstance.prototypeIndex].prefab;
                if (prefab.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                {
                    var mesh = meshFilter.sharedMesh;
                    if (!mesh)
                        continue;

                    for (int i = 0; i < mesh.subMeshCount; ++i)
                    {
                        var instanceDesc = new MeshInstanceDesc(mesh, i);
                        instanceDesc.localToWorldMatrix = localToWorld;
                        instanceDesc.mask = terrainInstance.mask;
                        instanceDesc.instanceID = terrainInstance.instanceID;
                        instanceDesc.materialID = terrainInstance.materialID;
                        instanceDesc.enableTriangleCulling = terrainInstance.enableTriangleCulling;
                        instanceDesc.frontTriangleCounterClockwise = terrainInstance.frontTriangleCounterClockwise;
                        instanceHandles.Add(accelStruct.AddInstance(instanceDesc));
                    }
                }
            }
        }

        internal static MeshInstanceDesc[] TerrainTreeToAccelStructInstances(Terrain terrain, TreeInstance treeInstance)
        {
            TerrainData terrainData = terrain.terrainData;
            float4x4 terrainLocalToWorld = terrain.transform.localToWorldMatrix;
            float3 positionScale = new float3((float)terrainData.heightmapResolution, 1.0f, (float)terrainData.heightmapResolution) * terrainData.heightmapScale;
            float3 positionOffset = new float3(terrainLocalToWorld[3].x, terrainLocalToWorld[3].y, terrainLocalToWorld[3].z);

            var localToWorld = Matrix4x4.TRS(
                positionOffset + new float3(treeInstance.position) * positionScale,
                Quaternion.AngleAxis(treeInstance.rotation, Vector3.up),
                new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale));

            var prefab = terrainData.treePrototypes[treeInstance.prototypeIndex].prefab;
            var mesh = prefab.gameObject.GetComponent<MeshFilter>().sharedMesh;
            if (!mesh)
                return new MeshInstanceDesc[0];

            var result = new MeshInstanceDesc[mesh.subMeshCount];
            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                var instanceDesc = new MeshInstanceDesc(mesh, i);
                instanceDesc.localToWorldMatrix = localToWorld;
                result[i] = instanceDesc;
            }

            return result;
        }

        internal static MeshInstanceDesc[] TerrainHeightmapToAccelStructInstances(Terrain terrain)
        {
            var terrainMesh = TerrainToMesh.Convert(terrain);
            var instanceDesc = new MeshInstanceDesc(terrainMesh);
            instanceDesc.localToWorldMatrix = terrain.transform.localToWorldMatrix;

            return new MeshInstanceDesc[] { instanceDesc };
        }

    }

}
