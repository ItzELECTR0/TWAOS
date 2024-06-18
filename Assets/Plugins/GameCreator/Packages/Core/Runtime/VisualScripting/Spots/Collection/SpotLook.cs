using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Characters.IK;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Look At")]
    [Image(typeof(IconEye), ColorTheme.Type.Green)]
    
    [Category("Characters/Look At")]
    [Description(
        "Makes the Character look at the center of the Hotspot when it's activated" +
        "and smoothly look away when it's deactivated"
    )]

    [Serializable]
    public class SpotLook : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] 
        protected int m_Priority;
        
        [SerializeField]
        protected PropertyGetDirection m_Offset = GetDirectionVector3Zero.Create();

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_WasActive;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Character look when near";

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnUpdate(Hotspot hotspot)
        {
            base.OnUpdate(hotspot);
            bool isActive = this.EnableInstance(hotspot);
            
            if (!this.m_WasActive && isActive)
            {
                RigLookTo lookTrack = this.GetCharacterLook(hotspot);
                lookTrack?.SetTarget(new LookToTransform(
                    this.m_Priority, 
                    hotspot.transform, 
                    this.m_Offset.Get(hotspot.Args)
                ));
            }

            if (this.m_WasActive && !isActive)
            {
                RigLookTo lookTrack = this.GetCharacterLook(hotspot);
                lookTrack?.RemoveTarget(new LookToTransform(
                    this.m_Priority, 
                    hotspot.transform,
                    this.m_Offset.Get(hotspot.Args)
                ));
            }

            this.m_WasActive = isActive;
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);
            this.m_WasActive = false;
            
            RigLookTo lookTrack = this.GetCharacterLook(hotspot);
            lookTrack?.RemoveTarget(new LookToTransform(
                this.m_Priority, 
                hotspot.transform,
                this.m_Offset.Get(hotspot.Args)
            ));
        }

        public override void OnDestroy(Hotspot hotspot)
        {
            base.OnDestroy(hotspot);
            this.m_WasActive = false;
            
            RigLookTo lookTrack = this.GetCharacterLook(hotspot);
            lookTrack?.RemoveTarget(new LookToTransform(
                this.m_Priority, 
                hotspot.transform,
                this.m_Offset.Get(hotspot.Args)
            ));
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual bool EnableInstance(Hotspot hotspot)
        {
            return hotspot.IsActive;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private RigLookTo GetCharacterLook(Hotspot hotspot)
        {
            if (hotspot.Target == null) return null;
            
            Character character = hotspot.Target.Get<Character>();
            return character != null ? character.IK.GetRig<RigLookTo>() : null;
        }
    }
}
