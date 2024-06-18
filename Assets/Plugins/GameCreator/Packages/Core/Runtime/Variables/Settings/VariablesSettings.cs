using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    public class VariablesSettings : AssetRepository<VariablesRepository>
    {
        public override IIcon Icon => new IconNameVariable(ColorTheme.Type.TextLight);
        public override string Name => "Variables";
    }
}