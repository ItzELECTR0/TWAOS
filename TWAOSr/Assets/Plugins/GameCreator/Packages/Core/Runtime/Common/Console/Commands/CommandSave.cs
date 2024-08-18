using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Console
{
    public sealed class CommandSave : Command
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Name => "store";

        public override string Description => "Manages the storage options";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public CommandSave() : base(new[]
        {
            new ActionOutput(
                "save",
                "Saves the game at slot value",
                value =>
                {
                    int slot = Convert.ToInt32(value);
                    _ = SaveLoadManager.Instance.Save(slot);
                    return Output.Success($"Saving: {slot}");
                }
            ),
            new ActionOutput(
                "load",
                "Loads a game from from the slot value",
                value =>
                {
                    int slot = Convert.ToInt32(value);
                    _ = SaveLoadManager.Instance.Load(slot);
                    return Output.Success($"Loading: {slot}");
                }
            ),
            new ActionOutput(
                "exists",
                "Returns true the slot value has saved game",
                value =>
                {
                    int slot = Convert.ToInt32(value);
                    bool hasSave = SaveLoadManager.Instance.HasSaveAt(slot);
                    return Output.Success($"Has Save at {slot} = {hasSave}");
                }
            ),
            new ActionOutput(
                "restart",
                "Loads a scene by index and resets any progress",
                value =>
                {
                    bool isIndex = int.TryParse(value, out int sceneIndex);
                    if (!isIndex) return Output.Error($"Unknown scene index for {value}");
                    
                    _ = SaveLoadManager.Instance.Restart(sceneIndex);
                    return Output.Success($"Restarting on scene index: {sceneIndex}");
                }
            ),
        }) { }
    }
}