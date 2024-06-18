using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target")]
    [Category("Target")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("Reference to the targeted game object")]

    [Serializable]
    public class GetGameObjectTarget : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args) => args.Target;
        public override GameObject Get(GameObject gameObject) => gameObject;

        public static PropertyGetGameObject Create()
        {
            GetGameObjectTarget instance = new GetGameObjectTarget();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Target";
    }
}