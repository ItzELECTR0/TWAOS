using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Material")]
    [Image(typeof(IconMaterial), ColorTheme.Type.Blue)]
    
    [Category("Materials/Change Material")]
    [Description("Changes the Material depending on whether the Hotspot is active or not")]
    
    [Keywords("Material", "Color", "Shader")]

    [Serializable]
    public class SpotMaterial : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertySetMaterial m_Target = SetMaterialNone.Create;
        
        [SerializeField] private PropertyGetMaterial m_OnActive = new PropertyGetMaterial();
        [SerializeField] private PropertyGetMaterial m_OnInactive = new PropertyGetMaterial();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_WasActive;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Material {this.m_OnActive} / {this.m_OnInactive}";

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnEnable(Hotspot hotspot)
        {
            base.OnEnable(hotspot);
            this.m_WasActive = false;
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);
            if (ApplicationManager.IsExiting) return;
            
            if (this.m_WasActive)
            {
                Args args = new Args(hotspot.gameObject, hotspot.Target);
                Material material = this.m_OnInactive.Get(args);
                this.m_Target.Set(material, args);
            }
        }

        public override void OnUpdate(Hotspot hotspot)
        {
            base.OnUpdate(hotspot);

            switch (this.m_WasActive)
            {
                case false when hotspot.IsActive:
                {
                    Args args = new Args(hotspot.gameObject, hotspot.Target);
                    Material material = this.m_OnActive.Get(args);
                    this.m_Target.Set(material, args);
                    break;
                }
                case true when !hotspot.IsActive:
                {
                    Args args = new Args(hotspot.gameObject, hotspot.Target);
                    Material material = this.m_OnInactive.Get(args);
                    this.m_Target.Set(material, args);
                    break;
                }
            }


            this.m_WasActive = hotspot.IsActive;
        }
    }
}