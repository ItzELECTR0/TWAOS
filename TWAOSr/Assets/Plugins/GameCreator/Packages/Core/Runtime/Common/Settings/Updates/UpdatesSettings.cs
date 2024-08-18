namespace GameCreator.Runtime.Common
{
    public class UpdatesSettings : AssetRepository<UpdatesRepository>
    {
        public override IIcon Icon => new IconDownload(ColorTheme.Type.TextLight);
        public override string Name => "Updates";

        public override int Priority => int.MaxValue;
    }
}