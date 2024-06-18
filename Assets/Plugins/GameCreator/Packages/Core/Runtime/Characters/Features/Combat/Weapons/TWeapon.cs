using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Icon(RuntimePaths.GIZMOS + "GizmoWeapon.png")]
    [Serializable]
    public abstract class TWeapon : ScriptableObject, IWeapon
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private UniqueID m_Id = new UniqueID();
        
        [SerializeField] private PropertyGetString m_Title = GetStringString.Create;
        [SerializeField] private PropertyGetString m_Description = GetStringTextArea.Create();

        [SerializeField] private PropertyGetSprite m_Icon = GetSpriteNone.Create;
        [SerializeField] private PropertyGetColor m_Color = GetColorColorsWhite.Create;
        
        [SerializeField] private Reaction m_HitReaction;
        [SerializeField] private Reaction m_ParriedReaction;

        [SerializeField] private RunInstructionsList m_OnEquip = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnUnequip = new RunInstructionsList();
        [SerializeField] private RunInstructionsList m_OnDodge = new RunInstructionsList();

        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString Id => this.m_Id.Get;

        public abstract Texture EditorIcon { get; }

        public abstract IShield Shield { get; }
        
        public IReaction HitReaction => this.m_HitReaction;
        public IReaction ParriedReaction => this.m_ParriedReaction;

        // GETTERS: -------------------------------------------------------------------------------

        public string GetName(Args args) => this.m_Title.Get(args);
        public string GetDescription(Args args) => this.m_Description.Get(args);
        
        public Sprite GetSprite(Args args) => this.m_Icon.Get(args);
        public Color GetColor(Args args) => this.m_Color.Get(args);
        
        // RUNNERS: -------------------------------------------------------------------------------
        
        public virtual async Task RunOnEquip(Character character, Args args)
        {
            await this.m_OnEquip.Run(args);
        }

        public virtual async Task RunOnUnequip(Character character, Args args)
        {
            await this.m_OnUnequip.Run(args);
        }
        
        public virtual async Task RunOnDodge(Character character, Args args)
        {
            await this.m_OnDodge.Run(args);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public abstract TMunitionValue CreateMunition();

        public override string ToString() => TextUtils.Humanize(this.name);
    }
}