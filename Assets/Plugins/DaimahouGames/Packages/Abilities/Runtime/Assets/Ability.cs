using System.Collections.Generic;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [CreateAssetMenu(menuName = "Game Creator/Abilities/Ability")]    
    [Icon(AbilityPaths.GIZMOS + "GizmoAbility.png")]

    public class Ability : ScriptableObject
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private UniqueID m_UID = new();
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private PropertyGetDecimal m_Range = new(1.5f);
        
        [SerializeReference] private AbilityActivator m_Activator;
        [SerializeReference] private AbilityTargeting m_Targeting;
        [SerializeReference] private AbilityFilter[] m_Filters;
        [SerializeReference] private AbilityRequirement[] m_Requirements;
        [SerializeReference] private AbilityEffect[] m_Effects;

        [SerializeField] private bool m_BetaFeature__ControllableWhileCasting;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public IdString ID => m_UID.Get;
        public AbilityActivator Activator => m_Activator;
        public AbilityTargeting Targeting => m_Targeting;
        public IEnumerable<AbilityRequirement> Requirements => m_Requirements;
        public IEnumerable<AbilityFilter> Filters => m_Filters;
        public IEnumerable<AbilityEffect> Effects => m_Effects;
        public bool ControllableWhileCasting => m_BetaFeature__ControllableWhileCasting;
        public Sprite Icon => m_Icon;

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public int GetRange(ExtendedArgs args) => (int) m_Range.Get(args);
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        //============================================================================================================||
    }
}