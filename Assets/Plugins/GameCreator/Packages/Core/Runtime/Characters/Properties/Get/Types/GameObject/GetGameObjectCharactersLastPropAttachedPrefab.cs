using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Last Prop Prefab Attached")]
    [Category("Characters/Props/Last Prop Prefab Attached")]
    
    [Image(typeof(IconTennis), ColorTheme.Type.Blue, typeof(OverlayPlus))]
    [Description("Reference to the latest Prop prefab used to instantiate the attachment to a Character")]

    [Serializable]
    public class GetGameObjectCharactersLastPropAttachedPrefab : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return Props.LastPropAttachedPrefab;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return Props.LastPropAttachedPrefab;
        }

        public override string String => "Last Prop Attached";
        
        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersLastPropAttachedPrefab()
        );
    }
}