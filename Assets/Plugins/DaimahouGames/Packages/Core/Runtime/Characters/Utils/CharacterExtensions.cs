using System.Threading.Tasks;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    public static class CharacterExtensions
    {
        public static Pawn GetPawn(this Character character)
        {
            var actor = character.GetComponent<Pawn>();
            if (actor == null) actor = character.gameObject.AddComponent<Pawn>();
            return actor;
        }
        
        public static Task PlayGesture(this Character character, ReactiveGesture gesture, Args args)
        {
            var gestureOutput = character.Get<ReactiveGestureOutput>();
            if (gestureOutput == null)
            {
                gestureOutput = character.gameObject.AddComponent<ReactiveGestureOutput>();
            }
            return gestureOutput.PlayGesture(gesture, args);
        }
        
        public static Task PlayGestureState(this Character character, ReactiveState state, Args args, int layer, ConfigState configuration, BlendMode blendMode)
        {
            var stateOutput = character.Get<ReactiveStateOutput>();
            if (stateOutput == null)
            {
                stateOutput = character.gameObject.AddComponent<ReactiveStateOutput>();
            }
            return stateOutput.SetState(state, args, layer, configuration, blendMode);
        }
        
        public static void StopState(this Character character, Args args)
        {
            var stateOutput = character.Get<ReactiveStateOutput>();
            if (stateOutput) stateOutput.StopState(args);
        }

        public static Task CancelGesture(this Character character)
        {
            var gestureOutput = character.Get<ReactiveGestureOutput>();
            return gestureOutput ? gestureOutput.Cancel() : Task.CompletedTask;
        }

        public static void FaceLocation(this Character character, Location location)
        {
            if (character.Kernel.Facing is UnitFacingPivotLocation facing)
            {
                facing.UpdateLocation(location);
            }
            else
            {
                var faceUnit = new UnitFacingPivotLocation();
                faceUnit.UpdateLocation(location);
                
                character.Kernel.ChangeFacing(character, faceUnit);
            }
        }

        public static void StopFacingLocation(this Character character, TUnitFacing prevFacing)
        {
            if (character.Kernel.Facing is UnitFacingPivotLocation facing)
            {
                facing.UpdateLocation(null);
            }
            character.Kernel.ChangeFacing(character, prevFacing);
        }
    }
}