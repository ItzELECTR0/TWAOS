using System;

namespace GameCreator.Runtime.Common
{
    public static class AssemblyUtils
    {
        [field: NonSerialized]
        public static bool IsReloading { get; private set; }

        static AssemblyUtils()
        {
            #if UNITY_EDITOR
            
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

            #endif
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static void OnBeforeAssemblyReload()
        {
            IsReloading = true;
        }
        
        private static void OnAfterAssemblyReload()
        {
            IsReloading = false;
        }
    }
}