using System;
using GameCreator.Runtime.Characters.IK;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Look Target")]
    [Category("Characters/IK/Character Look Target")]
    
    [Image(typeof(IconIK), ColorTheme.Type.Yellow)]
    [Description("Reference to the IK Look Target by a Character (if any)")]

    [Serializable]
    public class GetGameObjectCharactersIKLookTarget : PropertyTypeGetGameObject
    {
        [SerializeField] protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        public override GameObject Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null
                ? character.IK.GetRig<RigLookTo>()?.LookToTarget.Target
                : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            Character character = this.m_Character.Get<Character>(gameObject);
            return character != null
                ? character.IK.GetRig<RigLookTo>()?.LookToTarget.Target
                : null;
        }

        public override string String => $"{this.m_Character} Look Target";
    }
}