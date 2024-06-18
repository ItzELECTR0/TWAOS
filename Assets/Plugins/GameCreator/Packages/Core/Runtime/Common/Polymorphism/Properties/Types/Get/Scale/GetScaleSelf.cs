using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self Scale")]
    [Category("Game Objects/Self Scale")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("Scale of the caller in local or world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetScaleSelf : PropertyTypeGetScale
    {
        [SerializeField] private ScaleSpace m_Space = ScaleSpace.Local;

        public override Vector3 Get(Args args) => this.GetScale(args.Self);
        public override Vector3 Get(GameObject gameObject) => this.GetScale(gameObject);

        private Vector3 GetScale(GameObject gameObject)
        {
            if (gameObject == null) return Vector3.one;
            return this.m_Space switch
            {
                ScaleSpace.Local => gameObject.transform.localScale,
                ScaleSpace.Global => gameObject.transform.lossyScale,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertyGetScale Create => new PropertyGetScale(
            new GetScaleSelf()
        );

        public override string String => $"{this.m_Space} Self";
    }
}