using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Start Following")]
    [Description("Instructs a Character to follow another game object")]

    [Category("Characters/Navigation/Start Following")]

    [Parameter("Target", "The target game object to follow")]
    
    [Parameter(
        "Min Distance", 
        "Distance from the Target the Character aims to move when approaching the Target"
    )]
    
    [Parameter(
        "Max Distance", 
        "Maximum distance to the Target the Character leaves before attempting to move closer"
    )]
    
    [Keywords("Lead", "Pursue", "Chase", "Walk", "Run", "Position", "Location", "Destination")]
    [Image(typeof(IconCharacterRun), ColorTheme.Type.Blue)]

    [Serializable]
    public class InstructionCharacterNavigationFollowStart : TInstructionCharacterNavigation
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_FollowTarget = GetGameObjectPlayer.Create();

        [SerializeField] private PropertyGetDecimal m_MinDistance = GetDecimalDecimal.Create(2f);
        [SerializeField] private PropertyGetDecimal m_MaxDistance = GetDecimalDecimal.Create(4f);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Character} Follow {this.m_FollowTarget}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject target = this.m_FollowTarget.Get(args);
            if (target == null) return DefaultResult;
            
            character.Motion.StartFollowingTarget(
                target.transform,
                (float) this.m_MinDistance.Get(args),
                (float) this.m_MaxDistance.Get(args)
            );

            return DefaultResult;
        }
    }
}