using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 0)]

    [Description("Change the faction of target pawn")]

    [Category("Pawns/Factions/Change Faction")]

    [Parameter("Pawn", "The pawn target")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Pink)]

    [Keywords("Disable")]
    
    [Serializable]
    public abstract class InstructionPawnChangeFaction : Instruction
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] 
        private PropertyGetGameObject m_Pawn = GetGameObjectPlayer.Create();

        [SerializeField] private int m_Faction = 0;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => $"Change {m_Pawn}'s faction to {m_Faction}";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        protected override Task Run(Args args)
        {
            m_Pawn.Get<Faction>(args).ChangeFaction(m_Faction);
            return Task.CompletedTask;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}