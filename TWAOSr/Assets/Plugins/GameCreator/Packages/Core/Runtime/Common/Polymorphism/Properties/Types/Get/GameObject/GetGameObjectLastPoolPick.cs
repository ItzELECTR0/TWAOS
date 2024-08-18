using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Last Pick from Pool")]
    [Category("Game Objects/Last Pick from Pool")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Description("The last Game Object instance picked from its Pool")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectLastPoolPick : PropertyTypeGetGameObject
    {
        [SerializeField] protected PropertyGetGameObject m_Prefab = GetGameObjectInstance.Create();

        public override GameObject Get(Args args)
        {
            GameObject prefab = this.m_Prefab.Get(args);
            return PoolManager.Instance.GetLastPicked(prefab);
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectLastPoolPick instance = new GetGameObjectLastPoolPick();
            return new PropertyGetGameObject(instance);
        }
        
        public override string String => $"Pool[{this.m_Prefab}] Last Pick";
    }
}