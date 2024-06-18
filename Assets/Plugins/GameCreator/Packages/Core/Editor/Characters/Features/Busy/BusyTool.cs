using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    public class BusyTool
    {
        private const ColorTheme.Type THEME_BUSY = ColorTheme.Type.Red;
        private const ColorTheme.Type THEME_AVAILABLE = ColorTheme.Type.Green;
        
        private readonly IIcon ICON_BASE_PLAY = new IconBusyBase(THEME_AVAILABLE);
        private readonly IIcon ICON_BASE_EDIT = new IconBusyBase(ColorTheme.Type.TextLight);
        
        private readonly IIcon ICON_DEAD = new IconBusyDead(THEME_BUSY);
        private readonly IIcon ICON_NONE = new IconBusyNone(Color.white);
        
        private readonly IIcon ICON_ARM_L = new IconBusyArmL(Color.white);
        private readonly IIcon ICON_ARM_R = new IconBusyArmR(Color.white);
        private readonly IIcon ICON_LEG_L = new IconBusyLegL(Color.white);
        private readonly IIcon ICON_LEG_R = new IconBusyLegR(Color.white);
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly Character m_Character;

        private readonly Image m_ImageBackground;
        
        private readonly Image m_ImageDead;
        private readonly Image m_ImageArmL;
        private readonly Image m_ImageArmR;
        private readonly Image m_ImageLegL;
        private readonly Image m_ImageLegR;

        // INITIALIZERS: --------------------------------------------------------------------------

        public BusyTool(Character character, Image background, Image dead, 
            Image armL, Image armR, Image legL, Image legR)
        {
            this.m_Character = character;
            this.m_ImageBackground = background;

            this.m_ImageDead = dead;
            this.m_ImageArmL = armL;
            this.m_ImageArmR = armR;
            this.m_ImageLegL = legL;
            this.m_ImageLegR = legR;

            switch (EditorApplication.isPlaying)
            {
                case true:  this.OnChangeMode(PlayModeStateChange.EnteredPlayMode); break;
                case false: this.OnChangeMode(PlayModeStateChange.EnteredEditMode); break;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeMode(PlayModeStateChange state)
        {
            if (this.m_Character == null) return;
            
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    this.m_Character.Busy.EventChange -= this.OnChangeBusyState;
                    this.m_Character.Busy.EventChange += this.OnChangeBusyState;
                    
                    this.m_Character.EventDie -= this.OnChangeDeadState;
                    this.m_Character.EventDie += this.OnChangeDeadState;
                    
                    this.m_Character.EventRevive -= this.OnChangeDeadState;
                    this.m_Character.EventRevive += this.OnChangeDeadState;

                    this.Refresh(true);
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    this.m_Character.Busy.EventChange -= this.OnChangeBusyState;
                    this.m_Character.EventDie -= this.OnChangeDeadState;
                    this.m_Character.EventRevive -= this.OnChangeDeadState;
                    
                    this.Refresh(false);
                    break;
            }
        }

        private void OnChangeBusyState(Busy.Limb limbs)
        {
            this.Refresh(Application.isPlaying);
        }
        
        private void OnChangeDeadState()
        {
            this.Refresh(Application.isPlaying);
        }

        private void Refresh(bool playmode)
        {
            switch (playmode)
            {
                case true: this.RefreshPlaymode(); break;
                case false: this.RefreshEditmode(); break;
            }
        }

        private void RefreshEditmode()
        {
            this.m_ImageBackground.image = ICON_BASE_EDIT.Texture;

            this.m_ImageDead.image = ICON_NONE.Texture;
            this.m_ImageArmL.image = ICON_NONE.Texture;
            this.m_ImageArmR.image = ICON_NONE.Texture;
            this.m_ImageLegL.image = ICON_NONE.Texture;
            this.m_ImageLegR.image = ICON_NONE.Texture;
        }

        private void RefreshPlaymode()
        {
            bool isDead = this.m_Character.IsDead;
            
            this.m_ImageBackground.image = isDead ? ICON_NONE.Texture : ICON_BASE_PLAY.Texture;
            this.m_ImageDead.image = isDead ? ICON_DEAD.Texture : ICON_NONE.Texture;
            
            this.m_ImageArmL.image = isDead ? ICON_NONE.Texture : ICON_ARM_L.Texture;
            this.m_ImageArmR.image = isDead ? ICON_NONE.Texture : ICON_ARM_R.Texture;
            this.m_ImageLegL.image = isDead ? ICON_NONE.Texture : ICON_LEG_L.Texture;
            this.m_ImageLegR.image = isDead ? ICON_NONE.Texture : ICON_LEG_R.Texture;
            
            Busy state = this.m_Character.Busy;

            Color busy = ColorTheme.Get(THEME_BUSY);
            Color available = ColorTheme.Get(THEME_AVAILABLE);

            this.m_ImageArmL.tintColor = state.IsArmLeftBusy  ? busy : available;
            this.m_ImageArmR.tintColor = state.IsArmRightBusy ? busy : available;
            this.m_ImageLegL.tintColor = state.IsLegLeftBusy  ? busy : available;
            this.m_ImageLegR.tintColor = state.IsLegRightBusy ? busy : available;
        }
    }
}