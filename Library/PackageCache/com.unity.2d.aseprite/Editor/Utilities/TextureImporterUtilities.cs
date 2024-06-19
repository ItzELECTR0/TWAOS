using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite.Common;

namespace UnityEditor.U2D.Aseprite
{
    internal static class TextureImporterUtilities
    {
        public static TextureImporterPlatformSettings GetPlatformTextureSettings(BuildTarget buildTarget, in List<TextureImporterPlatformSettings> platformSettings)
        {
            var buildTargetName = TexturePlatformSettingsHelper.GetBuildTargetGroupName(buildTarget);
            TextureImporterPlatformSettings settings = null;
            settings = platformSettings.SingleOrDefault(x => x.name == buildTargetName && x.overridden == true);
            settings = settings ?? platformSettings.SingleOrDefault(x => x.name == TexturePlatformSettingsHelper.defaultPlatformName);

            if (settings == null)
            {
                settings = new TextureImporterPlatformSettings();
                settings.name = buildTargetName;
                settings.overridden = false;
                UpdateWithDefaultSettings(ref settings);
            }
            return settings;
        }

        public static void UpdateWithDefaultSettings(ref TextureImporterPlatformSettings platformSettings)
        {
            platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
