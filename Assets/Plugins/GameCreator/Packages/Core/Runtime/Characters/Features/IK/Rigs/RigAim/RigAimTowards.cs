using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Title("Aim Pitch")]
    [Category("Aim Pitch")]
    [Image(typeof(IconAimTarget), ColorTheme.Type.Green)]
    
    [Description("Aims with the bone upwards and downwards based on the rotation of an object")]
    
    [Serializable]
    public class RigAimTowards : TRigAnimatorIK
    {
        // CONSTANTS: -----------------------------------------------------------------------------

        public const string RIG_NAME = "RigAimTowards";

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private float m_SmoothTime = 0.1f;
        [SerializeField] private Bone m_Bone = new Bone(HumanBodyBones.Chest);
        [SerializeField] private PropertyGetGameObject m_From = GetGameObjectCameraMain.Create;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private AnimFloat m_Pitch;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Aim with {this.m_Bone} from {this.m_From}";
        
        public override string Name => RIG_NAME;
        
        public override bool RequiresHuman => false;
        public override bool DisableOnBusy => false;

        protected float SmoothTime => this.m_SmoothTime / Mathf.Rad2Deg;

        // IMPLEMENT METHODS: ---------------------------------------------------------------------

        protected override void DoStartup(Character character)
        {
            this.m_Pitch = new AnimFloat(0f, this.SmoothTime);
            base.DoStartup(character);
        }

        protected override void DoEnable(Character character)
        {
            character.EventAfterLateUpdate -= this.OnLateUpdate;
            character.EventAfterLateUpdate += this.OnLateUpdate;
            base.DoEnable(character);
        }

        protected override void DoDisable(Character character)
        {
            character.EventAfterLateUpdate -= this.OnLateUpdate;
            base.DoDisable(character);
        }

        protected override void DoUpdate(Character character)
        {
            base.DoUpdate(character);

            Transform from = this.m_From.Get<Transform>(this.Args);
            
            float target = from != null ? from.localRotation.eulerAngles.x : this.m_Pitch.Target;
            
            target -= target >= +180f ? 360f : 0f;
            target += target <= -180f ? 360f : 0f;
            
            this.m_Pitch.UpdateWithDelta(target, this.SmoothTime, character.Time.DeltaTime);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnLateUpdate()
        {
            Transform bone = this.m_Bone.GetTransform(this.Character.Animim.Animator);
            if (bone == null) return;
            
            Quaternion rotation = Quaternion.Euler(this.m_Pitch.Current, 0f, 0);
            bone.localRotation *= rotation;
        }
    }
}