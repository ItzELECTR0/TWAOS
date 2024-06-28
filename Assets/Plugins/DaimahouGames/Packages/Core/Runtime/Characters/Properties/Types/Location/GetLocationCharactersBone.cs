// using System;
// using GameCreator.Runtime.Characters;
// using GameCreator.Runtime.Common;
// using UnityEngine;
//
// namespace DaimahouGames.Runtime.Characters
// {
//     [Title("Character Bone")]
//     [Category("Characters/Character Bone")]
//     
//     [Image(typeof(IconBoneSolid), ColorTheme.Type.Yellow)]
//     [Description("The position and rotation of a Character bone")]
//
//     [Serializable]
//     public class GetLocation_CharacterBone : PropertyTypeGetLocation
//     {
//         [SerializeField] protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
//         [SerializeField] protected Bone m_Bone = new(HumanBodyBones.RightHand);
//         [SerializeField] private bool m_Rotate = true;
//
//         public override Location Get(Args args)
//         {
//             Character character = this.m_Character.Get<Character>(args);
//             if (character == null) return default;
//
//             Transform transform = this.m_Bone.GetTransform(character.Animim?.Animator);
//             
//             return new Location(
//                 new PositionTowards(transform, Vector3.one, Vector3.zero, 0f),
//                 this.m_Rotate ? new RotationTowards(transform) : new RotationNone()
//             );
//         }
//
//         public override string String => $"{this.m_Character}/{this.m_Bone}";
//     }
// }