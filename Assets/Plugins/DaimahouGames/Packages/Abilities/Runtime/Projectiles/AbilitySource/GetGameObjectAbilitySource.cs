using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{    
    [Title("Ability Source")]
    [Category("Abilities/Source")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Blue)]
    [Description("The source of the ability")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectAbilitySource : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            if (args is ExtendedArgs extendedArgs && extendedArgs.Has<AbiltySource>())
            {
                return extendedArgs.Get<AbiltySource>().GameObject;
            }
            return null;
        }

        public override GameObject Get(GameObject gameObject) => gameObject;

        public static PropertyGetGameObject Create()
        {
            return new PropertyGetGameObject(new GetGameObjectAbilitySource());
        }

        public override string String => "Ability Source";
    }
}