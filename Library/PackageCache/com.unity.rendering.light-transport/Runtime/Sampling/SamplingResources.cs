#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Rendering.Sampling
{
    internal class SamplingResources
    {
        public Texture2D scramblingTile;
        public Texture2D rankingTile;
        public Texture2D scramblingTex;
        public Texture2D owenScrambled256Tex;

#if UNITY_EDITOR
        public void Load()
        {
            const string path = "Packages/com.unity.rendering.light-transport/Runtime/";

            scramblingTile      = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "Sampling/Textures/CoherentNoise/ScramblingTile256SPP.png");
            rankingTile         = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "Sampling/Textures/CoherentNoise/RankingTile256SPP.png");
            scramblingTex       = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "Sampling/Textures/CoherentNoise/ScrambleNoise.png");
            owenScrambled256Tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "Sampling/Textures/CoherentNoise/OwenScrambledNoise256.png");
        }
#endif

        static public uint[] sobolMatrices = SobolData.SobolMatrices;
        public static readonly int cranleyPattersonRotationSize = 512;
        public static readonly int cranleyPattersonRotationMaxDimension = 12;
        public static readonly int cranleyPattersonRotationBufferSize = cranleyPattersonRotationSize * cranleyPattersonRotationSize * cranleyPattersonRotationMaxDimension;

        public static float[] GetCranleyPattersonRotations()
        {
            var values = new float[cranleyPattersonRotationBufferSize];
            UnityEngine.Random.InitState(5476424);
            for (int i = 0; i < cranleyPattersonRotationBufferSize; i++)
            {
                values[i] = UnityEngine.Random.Range(0.0f, 1.0f);
            }
            return values;
        }

        internal static void BindSamplingTextures(CommandBuffer cmd, SamplingResources resources)
        {
            cmd.SetGlobalTexture(Shader.PropertyToID("_ScramblingTileXSPP"), resources.scramblingTile);
            cmd.SetGlobalTexture(Shader.PropertyToID("_RankingTileXSPP"), resources.rankingTile);
            cmd.SetGlobalTexture(Shader.PropertyToID("_ScramblingTexture"), resources.scramblingTex);
            cmd.SetGlobalTexture(Shader.PropertyToID("_OwenScrambledTexture"), resources.owenScrambled256Tex);
        }
    }
}


