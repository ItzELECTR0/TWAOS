using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    internal static class SkinMeshUtils
    {
        private const string NAME = "Armature-{0}";

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static GameObject PutOn(GameObject prefab, Animator animator)
        {
            if (prefab == null || animator == null) return null;

            Transform root = animator.transform;
            Armature armature = new Armature(animator.GetComponentInParent<Character>(), root);

            GameObject instance = Object.Instantiate(prefab, root.position, root.rotation);
            instance.name = string.Format(NAME, prefab.name);
            
            SkinnedMeshRenderer[] renderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
            Transform target = SetupSkin(instance.transform, root);
            
            foreach (SkinnedMeshRenderer render in renderers)
            {
                SkinnedMeshRenderer renderer = AddSkinnedMeshRenderer(render, target);
                renderer.bones = GetTransforms(render.bones, armature);
            }

            return target.gameObject;
        }

        public static void TakeOff(GameObject instance)
        {
            if (instance == null) return;
            Object.Destroy(instance);
        }
        
        public static void TakeOff(GameObject prefab, Animator animator)
        {
            if (prefab == null || animator == null) return;

            string wearName = string.Format(NAME, prefab.name);
            Transform wear = animator.transform.Find(wearName);
            
            if (wear == null) return;
            Object.Destroy(wear.gameObject);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Transform SetupSkin(Transform source, Transform parent)
        {
            Animator animator = source.GetComponent<Animator>();
            if (animator != null) Object.Destroy(animator);
            
            source.SetParent(parent);

            for (int i = source.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(source.GetChild(i).gameObject);
            }

            return source;
        }

        private static SkinnedMeshRenderer AddSkinnedMeshRenderer(SkinnedMeshRenderer source, Transform parent)
        {
            GameObject instance = new GameObject(source.name);
            instance.transform.SetParent(parent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            SkinnedMeshRenderer instanceMesh = instance.AddComponent<SkinnedMeshRenderer>();
            instanceMesh.sharedMesh = source.sharedMesh;
            instanceMesh.sharedMaterials = source.sharedMaterials;
            return instanceMesh;
        }

        private static Transform[] GetTransforms(Transform[] sources, Armature armature)
        {
            Transform[] transforms = new Transform[sources.Length];
            for (int i = 0; i < sources.Length; ++i)
            {
                transforms[i] = armature.Get(sources[i].name);
            }

            return transforms;
        }
    }
}