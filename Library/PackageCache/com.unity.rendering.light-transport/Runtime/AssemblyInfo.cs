using System.Runtime.CompilerServices;

// Make visible to tests
[assembly: InternalsVisibleTo("Unity.PathTracing.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.LightTransport.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.Testing.Rendering.LightTransport.Runtime")]
[assembly: InternalsVisibleTo("Unity.Testing.Performance.LightTransport.Runtime")]
[assembly: InternalsVisibleTo("Assembly-CSharp-editor-testable")]

// Make visible internally to packages using the Unified Raytracing API, TODO: remove when the API is made public
[assembly: InternalsVisibleTo("Unity.PathTracing.Runtime")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Runtime")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.Core.Editor")]
[assembly: InternalsVisibleTo("Unity.RenderPipelines.HighDefinition.Runtime")]
