using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [CreateAssetMenu(menuName = "Game Creator/Abilities/Indicator")]    
    [Icon(DaimahouPaths.GIZMOS + "GizmoTargetArea.png")]
    
    public class Indicator : ScriptableObject
    {
        [SerializeField] private PropertyGetInstantiate m_Prefab = new();
        
        public GameObject Get(Args args, Vector3 size)
        {
            var indicator = m_Prefab.Get(args).RequiredOn(args.Self);
            indicator.transform.localScale = size;
            return indicator;
        }

        public GameObject Get(Args args, Vector3 position, Vector3 size)
        {
            var indicator = m_Prefab.Get(args).RequiredOn(args.Self);
            indicator.transform.localScale = size;
            indicator.transform.position = position;
            return indicator;
        }
    }
}