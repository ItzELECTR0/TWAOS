using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Image(typeof(IconReaction), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class ReactionItem : TPolymorphicItem<ReactionItem>
    {
        private const float INFINITE = 9999f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private EnablerFloat m_MinPower = new EnablerFloat(false, 1f);
        [SerializeField] private ReactionDirection m_Direction = ReactionDirection.FromAny;
        
        [SerializeField] private RunConditionsList m_Conditions = new RunConditionsList();

        [SerializeField] private AvatarMask m_AvatarMask;
        [SerializeField] private EnablerFloat m_CancelTime = new EnablerFloat(false, 0.5f);
        [SerializeField] private ReactionRotation m_Rotation = ReactionRotation.None; 
        [SerializeField] [Range(0f, 1f)] private float m_Gravity = 1f;
        
        [SerializeField] private ReactionAnimations m_Animations = new ReactionAnimations();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public AnimationClip AnimationClip => this.m_Animations.AnimationClip;
        public AvatarMask AvatarMask => this.m_AvatarMask;
        
        public float CancelTime => this.m_CancelTime.IsEnabled
            ? this.m_CancelTime.Value
            : INFINITE;

        public ReactionRotation Rotation => this.m_Rotation;
        
        public float Gravity => this.m_Gravity;

        public override string Title
        {
            get
            {
                string direction = TextUtils.Humanize(this.m_Direction).ToLower();
                direction = this.m_Direction == ReactionDirection.FromAny
                    ? $"{char.ToUpper(direction[0])}{direction[1..]} direction"
                    : $"{char.ToUpper(direction[0])}{direction[1..]}";

                string power = this.m_MinPower.IsEnabled
                    ? $" with Power ≥ {this.m_MinPower.Value}"
                    : "";

                string conditions = this.m_Conditions.ToString();
                conditions = !string.IsNullOrEmpty(conditions)
                    ? $" and {char.ToLower(conditions[0])}{conditions[1..]}"
                    : conditions;

                return $"{direction}{power}{conditions}";
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool CheckPower(float power)
        {
            if (this.m_MinPower.IsEnabled) return this.m_MinPower.Value <= power;
            return true;
        }

        public bool CheckDirection(Vector3 direction)
        {
            Vector3 flatDirection = Vector3.Scale(direction, Vector3Plane.NormalUp);
            
            return this.m_Direction switch
            {
                ReactionDirection.FromAny => true,
                ReactionDirection.FromTop => direction.y <= -0.5f,
                ReactionDirection.FromBottom => direction.y >= 0.5f,
                ReactionDirection.FromLeft => flatDirection.x >= 0.5f,
                ReactionDirection.FromRight => flatDirection.x <= -0.5f,
                ReactionDirection.FromFront => flatDirection.z <= -0.5f,
                ReactionDirection.FromBack => flatDirection.z >= 0.5f,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public bool CheckConditions(Args args) => this.m_Conditions.Check(args);
    }
}