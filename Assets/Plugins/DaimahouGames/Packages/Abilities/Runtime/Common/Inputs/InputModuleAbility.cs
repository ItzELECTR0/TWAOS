using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaimahouGames.Runtime.Abilities
{
    [Serializable]
    public class InputModuleAbility : InputModule, IInputProviderAbility
    {
        //============================================================================================================||
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private LayerMask m_GroundLayer;
        [SerializeField] private bool m_CastAtMaximumRange;
        
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        public override string Title => "Abilities Input";

        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public async Task GetTargetLocation(ExtendedArgs args, Indicator indicator, float radius)
        {
            var ability = args.Get<RuntimeAbility>();
            ability.OnStatus.Send("Begin input location");

            var casterPosition = ability.Caster.Position;
            
            var downwardRay = new Ray(ability.Caster.Position, Vector3.down);
            if (Physics.Raycast(downwardRay, out var groundHit, 1000, m_GroundLayer))
            {
                casterPosition.y = groundHit.point.y;
            }
            
            var marker = indicator != null 
                ? indicator.Get(args, casterPosition, GetScale(radius)) 
                : null;
            
            while (!ability.IsCanceled)
            {
                if (ability.GetStatus() == RuntimeAbility.Status.End || Mouse.current.leftButton.wasPressedThisFrame)
                {
                    ability.OnInputComplete.Send(args);
                    ability.OnStatus.Send("Input complete");
                    break;
                }
                
                await Task.Yield();

                if (ability.GetStatus() == RuntimeAbility.Status.Canceled)
                {
                    ability.OnStatus.Send("Input canceled");
                    ability.Cancel();
                    break;
                }

                if (!Raycast(out var raycastHit))
                {
                    continue;
                }

                var location = m_CastAtMaximumRange
                    ? GetValidLocation((float) ability.GetRange(args), ability.Caster.Position, raycastHit.point)
                    : raycastHit.point;
                
                args.Set(new Target(location));

                if(marker != null) marker.transform.position = location;

                if (!args.GetBool(ON_INPUT_PRESSED)) continue;

                ability.OnInputComplete.Send(args);
                ability.OnStatus.Send("Input complete");
                break;
            }
            
            if(marker != null) marker.SetActive(false);
        }
        public async Task GetTargetDirection(ExtendedArgs args, float distance)
        {
            var ability = args.Get<RuntimeAbility>();
            ability.OnStatus.Send("Begin input direction");

            var caster = args.Get<RuntimeAbility>().Caster;
            
            var camera = ShortcutMainCamera.Get<Camera>();
            
            while (!ability.IsCanceled)
            {
                if (ability.GetStatus() == RuntimeAbility.Status.End || Mouse.current.rightButton.wasPressedThisFrame)
                {
                    ability.OnInputComplete.Send(args);
                    ability.OnStatus.Send("Input complete");
                    break;
                }
                
                await Task.Yield();
                
                var plane = new Plane(Vector3.up, caster.Position);
                var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (plane.Raycast(ray, out var enter))
                {
                    var hitPoint = ray.GetPoint(enter);
                    var target = distance > 0
                        ? caster.Position + (hitPoint - caster.Position).normalized * distance
                        : hitPoint;
                
                    args.Set(new Target(target));
                }

                if (!args.GetBool(ON_INPUT_PRESSED)) continue;
                
                ability.OnInputComplete.Send(args);
                ability.OnStatus.Send("Input complete");
                break;
            }
        }
        
        public Pawn GetTargetUnit()
        {
            throw new NotImplementedException();
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|

        protected override void Started(int index)
        {
            m_Pawn.Get<Caster>().RequiredOn(m_Pawn).StartCast(index);
        }

        protected override void Canceled(int index)
        {
            m_Pawn.Get<Caster>().RequiredOn(m_Pawn).EndCast(index);
        }

        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|

        private Vector3 GetValidLocation(float range, Vector3 casterPosition, Vector3 location)
        {
            var distance = VectorHelper.Distance2D(casterPosition, location);
            if (distance <= range) return location;

            var direction = location - casterPosition;
            var validLocation = casterPosition + direction.normalized * range;
            validLocation.y = location.y;
            
            return validLocation;
        }
        
        private bool Raycast(out RaycastHit raycastHit)
        {
            var mouseRay = ShortcutMainCamera.Get<Camera>().ScreenPointToRay
            (
                Mouse.current.position.ReadValue()
            );

            return Physics.Raycast(mouseRay, out raycastHit, 1000, m_GroundLayer);
        }       
        
        private Vector3 GetScale(float radius)
        {
            return new Vector3
            (
                radius * 2, 
                1, 
                radius * 2
            );
        }

        //============================================================================================================||
    }
}