using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Tag")]
    [Image(typeof(IconTag), ColorTheme.Type.Red)]
    
    [Description("Filter based on tags")]
    
    [Serializable]
    public class AbilityFilterTag : AbilityFilter
    {    
        [SerializeField] private string tag;
        [SerializeField] private bool include;
        
        protected override string Summary => $"{(include ? "include" : "exclude")} [{tag}] tag";
        protected override bool Filter_Internal(ExtendedArgs args)
        {
            if (args.Target == null) return true;
            
            return include ? args.Target.CompareTag(tag) : !args.Target.CompareTag(tag);
        }
    }
}