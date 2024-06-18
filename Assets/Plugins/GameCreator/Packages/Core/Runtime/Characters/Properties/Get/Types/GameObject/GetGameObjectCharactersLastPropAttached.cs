using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Last Prop Instance Attached")]
    [Category("Characters/Props/Last Prop Instance Attached")]
    
    [Image(typeof(IconTennis), ColorTheme.Type.Yellow, typeof(OverlayPlus))]
    [Description("Reference to the latest Prop instance attached to a Character")]

    [Serializable]
    public class GetGameObjectCharactersLastPropAttached : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return Props.LastPropAttachedInstance;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return Props.LastPropAttachedInstance;
        }

        public override string String => "Last Prop Attached";
        
        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersLastPropAttached()
        );
    }
}