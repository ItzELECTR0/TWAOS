using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class ShaderGraphVariationsController : MonoBehaviour
    {
        // The reference to the Prototype (the prefab itself can be assigned here since the GPUI Prototype component lives on the Prefab).
        public GPUInstancerPrefab prefab;

        // The reference to the active Prefab Manager in the scene.
        public GPUInstancerPrefabManager prefabManager;

        // The count of instances that will be generated.
        public int instances = 1000;

        // The name of the buffer. Must be the same with the StructuredBuffer in the shader that the Material will use. See: "GPUIFloat4VariationInclude.cginc".
        private string bufferName = "gpuiFloat4Variation";

        // The List to hold the instances that will be generated.
        private List<GPUInstancerPrefab> goList;

        void Start()
        {
            System.Random random = new System.Random();

            goList = new List<GPUInstancerPrefab>();

            // Define the buffer to the Prefab Manager.
            if (prefabManager != null && prefabManager.isActiveAndEnabled)
            {
                GPUInstancerAPI.DefinePrototypeVariationBuffer<Vector4>(prefabManager, prefab.prefabPrototype, bufferName);
            }

            // Generate instances inside a radius.
            for (int i = 0; i < instances; i++)
            {
                GPUInstancerPrefab prefabInstance = Instantiate(prefab);
                prefabInstance.transform.localPosition = new Vector3(Random.Range(-24f, 24f), 0, Random.Range(-24f, 24f));
                prefabInstance.transform.SetParent(transform);
                goList.Add(prefabInstance);

                // Register the variation buffer for this instance.
                prefabInstance.AddVariation(bufferName, new Vector4(random.Next(0, 9) / 8f, random.Next(0, 5) / 4f, 1, 1));
            }

            // Register the generated instances to the manager and initialize the manager.
            if (prefabManager != null && prefabManager.isActiveAndEnabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, goList);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            }
        }
    }

}