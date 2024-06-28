using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Title("Look at Location")]
    [Image(typeof(IconLocation), ColorTheme.Type.Blue)]
    
    [Category("Look at Location")]
    [Description("Rotates the Character towards a specific Location")]
    
    [Serializable]
    public class UnitFacingLocation : TUnitFacing
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private PropertyGetLocation m_Target = GetLocationNone.Create;
        
        private Args m_Args;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public Args Args => m_Args ??= new Args(Character);
        public override string ToString() => "Look at Location";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        public override Axonometry Axonometry { get; set; }

        protected override Vector3 GetDefaultDirection()
        {
            var driverDirection = Vector3.Scale(
                m_Target.Get(Args).GetPosition(Character.gameObject) - Transform.position,
                Vector3Plane.NormalUp
            );

            return DecideDirection(driverDirection);
        }
        
        //============================================================================================================||
    }
}