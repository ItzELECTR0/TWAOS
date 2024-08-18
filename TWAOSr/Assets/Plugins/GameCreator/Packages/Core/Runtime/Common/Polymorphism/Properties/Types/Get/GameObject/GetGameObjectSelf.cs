using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self")]
    [Category("Self")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("Reference to the origin game object that made this call")]

    [Serializable]
    public class GetGameObjectSelf : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args) => args.Self;
        public override GameObject Get(GameObject gameObject) => gameObject;

        public static PropertyGetGameObject Create()
        {
            GetGameObjectSelf instance = new GetGameObjectSelf();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Self";
    }
}