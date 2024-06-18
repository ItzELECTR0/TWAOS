using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Has Prop Attached")]
    [Description("Returns true if the Character has a Prop attached to the specified bone")]

    [Category("Characters/Visuals/Has Prop Attached")]
    [Parameter("Bone", "The bone that has the prop attached to")]

    [Keywords("Characters", "Holds", "Grab", "Draw", "Pull", "Take", "Object")]
    [Image(typeof(IconTennis), ColorTheme.Type.Yellow)]

    [Serializable]
    public class ConditionHasPropAttached : TConditionCharacter
    {
        [SerializeField] private Bone m_Bone = new Bone(HumanBodyBones.RightHand);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"has {this.m_Character} Prop at {this.m_Bone}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && character.Props.HasAtBone(this.m_Bone);
        }
    }
}
