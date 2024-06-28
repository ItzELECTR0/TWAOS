using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Activation: Charged")]
    [Category("In development/[Place holder] Activation: Charged")]

    [Serializable]
    public abstract class AbilityActivatorCharged : AbilityActivator
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private ReactiveGesture m_Animation;
        
        [SerializeField] private bool m_HasDuration;
        [SerializeField] private float m_Duration;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        //public override string Title => "[PLACE HOLDER] " + base.Title;

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}