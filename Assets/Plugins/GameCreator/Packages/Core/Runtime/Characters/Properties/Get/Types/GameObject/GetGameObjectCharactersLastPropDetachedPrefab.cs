using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Last Prop Prefab Detached")]
    [Category("Characters/Props/Last Prop Prefab Detached")]
    
    [Image(typeof(IconTennis), ColorTheme.Type.Blue, typeof(OverlayMinus))]
    [Description("Reference to the latest Prop instance detached from a Character")]

    [Serializable]
    public class GetGameObjectCharactersLastPropDetachedPrefab : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return Props.LastPropDetachedPrefab;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return Props.LastPropDetachedPrefab;
        }

        public override string String => "Last Prop Attached";
        
        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersLastPropDetachedPrefab()
        );
    }
}