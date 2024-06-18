using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class MotionToDirection : TMotion
    {
        // INITIALIZERS: --------------------------------------------------------------------------

        public Character.MovementType Setup(Vector3 velocity, Space space)
        {
            this.Setup();

            Character.MovementType result = velocity.sqrMagnitude > Character.BIG_EPSILON
                ? Character.MovementType.MoveToDirection
                : Character.MovementType.None;
            
            // No call to CalculateSpeed because velocity already contains it
            velocity = this.CalculateAcceleration(velocity);

            this.Motion.MoveDirection = space switch
            {
                Space.World => velocity,
                Space.Self => this.Transform.TransformDirection(velocity),
                _ => this.Motion.MoveDirection
            };

            this.Motion.MovePosition = this.Transform.TransformPoint(this.Motion.MoveDirection);
            return result;
        }

        // INTERFACE METHODS: ---------------------------------------------------------------------

        public override Character.MovementType Update()
        {
            this.Motion.MovePosition = this.Motion
                .Transform
                .TransformPoint(this.Motion.MoveDirection);
            
            return this.Motion.MoveDirection.sqrMagnitude > Character.BIG_EPSILON
                ? Character.MovementType.MoveToDirection
                : Character.MovementType.None;
        }
    }
}