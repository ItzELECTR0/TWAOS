using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null Material reference")]

    [Keywords("Null", "Empty", "Shader")]
    
    [Serializable]
    public class GetMaterialNone : PropertyTypeGetMaterial
    {
        public override Material Get(Args args) => null;
        public override Material Get(GameObject gameObject) => null;

        public static PropertyGetMaterial Create => new PropertyGetMaterial(
            new GetMaterialNone()
        );

        public override string String => "None";
    }
}