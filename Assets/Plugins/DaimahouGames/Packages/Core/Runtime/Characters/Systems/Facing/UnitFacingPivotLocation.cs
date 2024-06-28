using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Title("Pawn - Pivot")]
    [Image(typeof(IconPawn), ColorTheme.Type.Blue)]
    
    [Category("Look at Target - Pivot")]
    [Description("Rotates the Character towards its target if available, pivot otherwise." +
                 " Prevents editor errors from switching facing unit at runtime while inspector is open")]
    
    [Serializable]
    public class UnitFacingPivotLocation : TUnitFacing
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        private Location? m_Location;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public void UpdateLocation(Location? location)
        {
            m_Location = location;
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        public override Axonometry Axonometry { get; set; }

        protected override Vector3 GetDefaultDirection()
        {
            var driverDirection = m_Location == null
                ? GetPivotDirection()
                : GetLocation(m_Location.Value);
            
            return DecideDirection(driverDirection);
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private Vector3 GetPivotDirection()
        {
            return Vector3.Scale(
                Character.Driver.WorldMoveDirection,
                Vector3Plane.NormalUp
            );
        }

        private Vector3 GetLocation(Location location)
        {
            return Vector3.Scale(
                location.GetPosition(Character.gameObject) - Transform.position,
                Vector3Plane.NormalUp
            );
        }

        //============================================================================================================||
    }
}