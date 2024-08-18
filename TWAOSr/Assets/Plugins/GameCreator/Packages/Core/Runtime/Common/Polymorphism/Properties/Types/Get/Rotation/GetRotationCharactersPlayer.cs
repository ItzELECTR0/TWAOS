using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Player Rotation")]
    [Category("Characters/Player Rotation")]
    
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]
    [Description("Rotation of the Player character in local or world space")]

    [Serializable]
    public class GetRotationCharactersPlayer : PropertyTypeGetRotation
    {
        [SerializeField] private RotationSpace m_Space = RotationSpace.Global;
        
        public override Quaternion Get(Args args)
        {
            Transform transform = ShortcutPlayer.Transform;
            if (transform == null) return default;

            return this.m_Space switch
            {
                RotationSpace.Local => transform.localRotation,
                RotationSpace.Global => transform.rotation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override Quaternion Get(GameObject gameObject) => ShortcutPlayer.Transform.rotation;

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationCharactersPlayer()
        );

        public override string String => $"{this.m_Space} Player";
    }
}