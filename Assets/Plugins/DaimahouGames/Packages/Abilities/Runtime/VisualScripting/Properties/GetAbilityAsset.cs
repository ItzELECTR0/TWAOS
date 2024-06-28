using System;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace Plugins.DaimahouGames.Packages.Abilities.Runtime.VisualScripting.Properties
{
    [Title("Asset")]
    [Category("Asset")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Yellow)]
    [Description("Reference to an ability")]

    [Serializable] [HideLabelsInEditor]
    public class GetAbilityAsset : PropertyTypeGetAbility
    {
        [SerializeField] private Ability m_Ability;
        
        public override Ability Get(Args args)
        {
            return this.m_Ability;
        }

        public override Ability Get(GameObject gameObject)
        {
            return this.m_Ability;
        }

        public static PropertyGetAbility Create => new(
            new GetAbilityAsset()
        );

        public override string String => this.m_Ability != null
            ? this.m_Ability.name
            : "(none)";
    }
}