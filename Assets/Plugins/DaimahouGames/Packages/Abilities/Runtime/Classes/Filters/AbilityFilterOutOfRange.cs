using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Out of Range")]
    [Image(typeof(IconTargetArea), ColorTheme.Type.Red)]
    
    [Description("Filter targets that moved out of range between the start of the animation and the activation " +
                 "for the effects - useful for melee attacks")]
    
    [Serializable]
    public class AbilityFilterOutOfRange : AbilityFilter
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField, Range(0, 1f)] private float m_Leeway = .25f;
        [SerializeField, Range(0, 360f)] private float m_MaxFacingAngle = 180f;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => string.Format("Out of range target - in a {0} cone of vision.", 
            m_MaxFacingAngle
        );
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        protected override bool Filter_Internal(ExtendedArgs args)
        {
            var targetPosition = args.Get<Target>().Position;
            var casterTransform = args.Self.transform;
            
            var distance = VectorHelper.Distance2D(casterTransform.position, targetPosition);
            var angle = VectorHelper.Angle2D(casterTransform, targetPosition);
            var range = args.Get<RuntimeAbility>().GetRange(args);

            return angle > m_MaxFacingAngle / 2 || distance > (1 + m_Leeway) * range;
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        
        //============================================================================================================||
    }
}