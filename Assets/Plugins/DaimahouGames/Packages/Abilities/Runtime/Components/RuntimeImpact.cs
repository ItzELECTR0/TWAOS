using System.Linq;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Creator/Abilities/Impact")]
    
    [Icon(AbilityPaths.GIZMOS + "GizmoImpact.png")]
    public class RuntimeImpact : MonoBehaviour
    {
        private const int BUFFER_SIZE = 32;

        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Impact m_Impact;
            
        private readonly Collider[] m_Buffer = new Collider[BUFFER_SIZE];
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        public void Initialize(Impact impact) => m_Impact = impact;

        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public async void Execute(ExtendedArgs args)
        {
            if (m_Impact.Effects.Length <= 0) return;

            var impactArgs = args.Clone;
            impactArgs.ChangeSelf(gameObject);
            
            await Awaiters.Seconds(m_Impact.Delay);
                
            CalculateTargets(impactArgs);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void CalculateTargets(ExtendedArgs args)
        {
            var count = Physics.OverlapSphereNonAlloc(
                transform.position, m_Impact.GetRadius(args), 
                m_Buffer, m_Impact.LayerMask
            );
            
            args.Set(new Target(transform));
            
            if(m_Impact.Filters.All(filter => !filter.Filter(args)))
            {
                foreach (var effect in m_Impact.Effects) effect.Apply(args);
            }
            
            for (var i = 0; i < count; i++)
            {
                var pawn = m_Buffer[i].GetComponent<Pawn>();
                if (pawn == null) continue;

                args.Set(new Target(pawn));
                
                if(m_Impact.Filters.Any(filter => filter.Filter(args))) continue; 
                
                if (pawn) pawn.Message.Send(new MessageAbilityHit(args.Get<RuntimeAbility>()?.Caster?.GameObject));
                
                foreach (var effect in m_Impact.Effects) effect.Apply(args);
            }
        }

        //============================================================================================================||
    }
}