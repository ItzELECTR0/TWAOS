using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandApplication : Command
    {
        public override string Name => "app";

        public override string Description => "Interacts with the game application";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandApplication() : base(new[]
        {
            new ActionOutput(
                "quit",
                "Quits the game",
                _ =>
                {
                    Application.Quit();
                    return Output.Success("Exiting application...");
                }
            ),
            new ActionOutput(
                "fps",
                "Displays the frames per second",
                _ =>
                {
                    float fps = 1f / Time.unscaledDeltaTime;
                    return Output.Success($"FPS: {fps}");
                }
            ),
            new ActionOutput(
                "version",
                "Displays the game and Unity versions",
                _ =>
                {
                    string gameVersion = Application.version;
                    string unityVersion = Application.unityVersion;
                    return Output.Success($"Version: {gameVersion} in Unity: {unityVersion}");
                }
            ),
        }) { }
    }
}