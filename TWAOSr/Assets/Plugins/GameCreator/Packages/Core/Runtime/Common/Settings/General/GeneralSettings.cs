namespace GameCreator.Runtime.Common
{
    public class GeneralSettings : AssetRepository<GeneralRepository>
    {
        public override IIcon Icon => new IconCog(ColorTheme.Type.TextLight);
        public override string Name => "General";

        public override int Priority => 0;
    }
}