using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.RenderPipelines.HighDefinition.Editor")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.HighDefinition.Runtime")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Universal.Editor")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Universal.Runtime")]


//WARNING:
//  Remember to only use this shared API to cherry pick the code part that you want to
//  share but not go directly in user codebase project.
//  Every new entry here should be discussed. It is always better to have good public API.
//  Don't add logic in this assembly. It is only to share private methods. Only redirection allowed.

//EXAMPLE: See com.unity.render-pipelines.core/Editor-PrivateShared/AssemblyInfo.cs

namespace Unity.RenderPipelines.Core.Runtime.Shared
{
    internal static class CameraCaptureBridge
    {
        public static System.Collections.Generic.IEnumerator<System.Action<UnityEngine.Rendering.RenderTargetIdentifier, UnityEngine.Rendering.CommandBuffer>>
            GetCachedCaptureActionsEnumerator(UnityEngine.Camera camera) => UnityEngine.Rendering.CameraCaptureBridge.GetCachedCaptureActionsEnumerator(camera);
    }
}
