using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public class ToolCombat : VisualElement
    {
        private const string USS_PATH = EditorPaths.CHARACTERS + "StyleSheets/Combat";

        private const string NAME_ROOT = "GC-Characters-Combat-Root";
        private const string NAME_HEAD = "GC-Characters-Combat-Head";
        private const string NAME_BODY = "GC-Characters-Combat-Body";
        private const string NAME_FOOT = "GC-Characters-Combat-Foot";

        private const string NAME_DEFENSE_LABEL = "GC-Characters-Combat-Defense-Label";
        private const string NAME_DEFENSE_BAR = "GC-Characters-Combat-Defense-Bar";
        private const string NAME_DEFENSE_PROGRESS = "GC-Characters-Combat-Defense-Progress";

        private const long UPDATE_INTERVAL = 30;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly VisualElement m_Root;
        [NonSerialized] private readonly VisualElement m_Head;
        [NonSerialized] private readonly VisualElement m_Body;
        [NonSerialized] private readonly VisualElement m_Foot;

        [NonSerialized] private readonly LabelProgress m_Invincibility;
        [NonSerialized] private readonly LabelProgress m_Poise;
        [NonSerialized] private readonly LabelProgress m_Defense;

        [NonSerialized] private readonly Character m_Character;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ToolCombat(Character character)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (character == null) return;
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in sheets) this.styleSheets.Add(styleSheet);

            this.m_Character = character;
            
            this.m_Character.Combat.EventEquip -= this.RefreshFoot;
            this.m_Character.Combat.EventUnequip -= this.RefreshFoot;
            
            this.m_Character.Combat.EventEquip += this.RefreshFoot;
            this.m_Character.Combat.EventUnequip += this.RefreshFoot;

            this.m_Root = new VisualElement { name = NAME_ROOT };
            this.m_Head = new VisualElement { name = NAME_HEAD };
            this.m_Body = new VisualElement { name = NAME_BODY };
            this.m_Foot = new VisualElement { name = NAME_FOOT };
            
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);
            this.m_Root.Add(this.m_Foot);
            
            this.Add(this.m_Root);

            this.m_Invincibility = new LabelProgress(0f, ColorTheme.Get(ColorTheme.Type.Yellow), "Invincible");
            this.m_Head.Add(this.m_Invincibility);

            character.Combat.Invincibility.EventChange += isInvincible =>
            {
                this.m_Invincibility.Value = isInvincible ? 1f : 0f;
            };
            
            this.m_Poise = new LabelProgress(0f, ColorTheme.Get(ColorTheme.Type.Green), "Poise");
            this.m_Head.Add(this.m_Poise);

            character.Combat.Poise.EventChange += () =>
            {
                float ratio = character.Combat.Poise.Ratio;
                this.m_Poise.Value = ratio;
            };

            this.m_Defense = new LabelProgress(0f, ColorTheme.Get(ColorTheme.Type.Blue), "Defense");
            this.m_Body.Add(this.m_Defense);
            
            this.schedule.Execute(this.RefreshBody).Every(UPDATE_INTERVAL);
            
            this.RefreshFoot(null, null);
        }

        private void RefreshBody()
        {
            string shieldName = this.m_Character.Combat.Block.Shield?.Name ?? "(No Shield)";
            float curDefense = this.m_Character.Combat.CurrentDefense;
            float maxDefense = this.m_Character.Combat.MaximumDefense;
            
            this.m_Defense.Label = $"{shieldName}: {curDefense:0.0}/{maxDefense:0}";

            float progress = maxDefense > 0f ? curDefense / maxDefense : 1f;
            this.m_Defense.Value = progress;
        }

        private void RefreshFoot(IWeapon weapon, GameObject instance)
        {
            this.m_Foot.Clear();

            Weapon[] weapons = this.m_Character.Combat.Weapons;
            this.m_Root.style.display = weapons.Length > 0
                ? DisplayStyle.Flex
                : DisplayStyle.None; 
            
            foreach (Weapon reference in weapons)
            {
                TMunitionValue munition = this.m_Character.Combat.RequestMunition(reference.Asset);
                ToolCombatItem item = new ToolCombatItem(reference, munition);
                this.m_Foot.Add(item);
            }
        }
    }
}