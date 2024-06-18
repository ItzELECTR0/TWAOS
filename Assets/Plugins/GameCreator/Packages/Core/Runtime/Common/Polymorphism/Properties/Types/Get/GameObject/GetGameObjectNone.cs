using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("Game Objects/None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null game object reference")]

    [Serializable]
    public class GetGameObjectNone : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args) => null;
        public override GameObject Get(GameObject gameObject) => null;

        public static PropertyGetGameObject Create()
        {
            GetGameObjectNone instance = new GetGameObjectNone();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "None";
    }
}