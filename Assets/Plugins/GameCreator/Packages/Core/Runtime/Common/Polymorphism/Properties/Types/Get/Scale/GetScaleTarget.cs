using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target Scale")]
    [Category("Game Objects/Target Scale")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("Scale of the targeted game object in local or world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetScaleTarget : PropertyTypeGetScale
    {
        [SerializeField] private ScaleSpace m_Space = ScaleSpace.Local;

        public override Vector3 Get(Args args) => this.GetScale(args.Target);
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
            new GetScaleTarget()
        );

        public override string String => $"{this.m_Space} Target";
    }
}