using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Last Prop Instance Detached")]
    [Category("Characters/Props/Last Prop Instance Detached")]
    
    [Image(typeof(IconTennis), ColorTheme.Type.Yellow, typeof(OverlayMinus))]
    [Description("Reference to the latest Prop instance detached from a Character")]

    [Serializable]
    public class GetGameObjectCharactersLastPropDetached : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return Props.LastPropDetachedInstance;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return Props.LastPropDetachedInstance;
        }

        public override string String => "Last Prop Attached";
        
        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersLastPropDetached()
        );
    }
}