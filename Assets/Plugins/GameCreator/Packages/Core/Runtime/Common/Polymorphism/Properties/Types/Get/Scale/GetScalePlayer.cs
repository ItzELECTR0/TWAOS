using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Player Scale")]
    [Category("Characters/Player Scale")]
    
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]
    [Description("Scale of the Player character in local or world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetScalePlayer : PropertyTypeGetScale
    {
        [SerializeField] private ScaleSpace m_Space = ScaleSpace.Local;

        public override Vector3 Get(Args args) => this.GetScale();
        public override Vector3 Get(GameObject gameObject) => this.GetScale();

        private Vector3 GetScale()
        {
            if (ShortcutPlayer.Instance == null) return Vector3.one;
            return this.m_Space switch
            {
                ScaleSpace.Local => ShortcutPlayer.Transform.localScale,
                ScaleSpace.Global => ShortcutPlayer.Transform.lossyScale,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertyGetScale Create => new PropertyGetScale(
            new GetScalePlayer()
        );

        public override string String => $"{this.m_Space} Player";
    }
}