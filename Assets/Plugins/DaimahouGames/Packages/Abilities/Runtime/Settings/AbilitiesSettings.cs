using GameCreator.Runtime.Common;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DaimahouGames.Runtime.Abilities
{
    public class AbilitiesSettings : AssetRepository<AbilitiesRepository>
    {
        public override IIcon Icon => new IconAbility(ColorTheme.Type.TextLight);
        public override string Name => "Abilities";

        #if UNITY_EDITOR

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= this.OnChangePlayMode;
            EditorApplication.playModeStateChanged += this.OnChangePlayMode;
            
            this.RefreshItemList();
        }

        private void OnChangePlayMode(PlayModeStateChange playModeStateChange)
        {
            this.RefreshItemList();
        }

        private void RefreshItemList()
        {
            string[] abilitiesGuids = AssetDatabase.FindAssets($"t:{nameof(Ability)}");
            Ability[] abilities = new Ability[abilitiesGuids.Length];

            for (int i = 0; i < abilitiesGuids.Length; i++)
            {
                string itemsGuid = abilitiesGuids[i];
                string itemPath = AssetDatabase.GUIDToAssetPath(itemsGuid);
                abilities[i] = AssetDatabase.LoadAssetAtPath<Ability>(itemPath);
            }

            this.Get().Abilities.Set(abilities);
        }

        #endif
    }
}
