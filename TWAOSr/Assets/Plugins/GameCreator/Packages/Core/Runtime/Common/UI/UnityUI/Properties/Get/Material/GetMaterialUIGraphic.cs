using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common
{
    [Title("Graphic Material")]
    [Category("UI/Graphic Material")]
    
    [Image(typeof(IconUIImage), ColorTheme.Type.TextLight)]
    [Description("A reference to the main Material instance of a Graphic (Image or Text) component")]

    [Keywords("Material", "Shader", "Image", "Text")]
    
    [Serializable]
    public class GetMaterialUIGraphic : PropertyTypeGetMaterial
    {
        [SerializeField] private PropertyGetGameObject m_Graphic = GetGameObjectInstance.Create();

        public override Material Get(Args args)
        {
            Graphic graphic = this.m_Graphic.Get<Graphic>(args);
            return graphic != null ? graphic.material : null;
        }

        public static PropertyGetMaterial Create() => new PropertyGetMaterial(
            new GetMaterialRendererShared()
        );

        public override string String => $"{this.m_Graphic} Material";
    }
}