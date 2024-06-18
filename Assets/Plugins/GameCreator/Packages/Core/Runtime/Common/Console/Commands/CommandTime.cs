using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandTime : Command
    {
        private const int TIME_LAYER = 99;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Name => "time";

        public override string Description => "Changes the time of the game";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandTime() : base(new[]
        {
            new ActionOutput(
                "pause",
                "Sets the time scale to zero",
                _ =>
                {
                    TimeManager.Instance.SetTimeScale(0f, TIME_LAYER);
                    return Output.Success("Time Scale = 0");
                }
            ),
            new ActionOutput(
                "scale",
                "Changes the time scale",
                value =>
                {
                    float timeScale = Convert.ToSingle(value);
                    TimeManager.Instance.SetTimeScale(timeScale, TIME_LAYER);
                    return Output.Success($"Time Scale = {timeScale}");
                }
            ),
            new ActionOutput(
                "normal",
                "Sets the time scale to one",
                _ =>
                {
                    TimeManager.Instance.SetTimeScale(1f, TIME_LAYER);
                    return Output.Success("Time Scale = 1");
                }
            ),
        }) { }
    }
}