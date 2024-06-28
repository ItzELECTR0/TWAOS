using System;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.VisualScripting.Conditions
{
    [Title("Are Enemy")]
    [Description("Returns true if the Characters are Enemies.")]

    [Category("Pawn/Factions/Are Enemies")]
    
    [Parameter("A, B", "The pawns which are being checked.")]

    [Keywords("Pawn", "Enemy", "Faction")]
    [Image(typeof(IconCharacterState), ColorTheme.Type.Red)]

    [Serializable]
    public abstract class ConditionPawnFaction : Condition
    {
        [SerializeField] private PropertyGetGameObject m_Pawn = GetGameObjectPlayer.Create();
        [SerializeField] private PropertyGetGameObject m_Other = GetGameObjectCharactersInstance.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{m_Pawn} and {m_Other} are enemies";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return m_Pawn.Get<Faction>(args).IsEnemy(m_Other.Get<Pawn>(args));
        }
    }
}