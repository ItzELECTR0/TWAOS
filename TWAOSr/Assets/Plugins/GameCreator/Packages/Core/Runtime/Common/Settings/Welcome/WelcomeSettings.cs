namespace GameCreator.Runtime.Common
{
    public class WelcomeSettings : AssetRepository<WelcomeRepository>
    {
        public override IIcon Icon => new IconHome(ColorTheme.Type.TextLight);
        public override string Name => "Welcome";

        public override int Priority => -1;

        public override bool IsFullScreen => true;
    }
}