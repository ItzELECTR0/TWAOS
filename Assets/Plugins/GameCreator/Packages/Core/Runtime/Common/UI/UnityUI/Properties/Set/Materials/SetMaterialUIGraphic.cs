using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Graphic")]
    [Category("UI/Graphic")]
    
    [Description("Sets the Graphic's Material value")]
    [Image(typeof(IconUIImage), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetMaterialUIGraphic : PropertyTypeSetMaterial
    {
        [SerializeField] private PropertyGetGameObject m_Graphic = GetGameObjectInstance.Create();

        public override void Set(Material value, Args args)
        {
            Graphic graphic = this.m_Graphic.Get<Graphic>(args);
            if (graphic == null) return;
            
            graphic.material = value;
        }

        public override Material Get(Args args)
        {
            Graphic graphic = this.m_Graphic.Get<Graphic>(args);
            return graphic != null ? graphic.material : null;
        }

        public static PropertySetMaterial Create => new PropertySetMaterial(
            new SetMaterialUIGraphic()
        );
        
        public override string String => this.m_Graphic.ToString();
    }
}