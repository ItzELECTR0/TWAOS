using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public class MotionFollow : TMotion
    {
        protected bool m_ActiveFollow;
        protected bool m_IsFollowing;

        protected Transform m_Target;
        protected Vector3 m_LastKnownPosition;

        protected float m_MinRadius;
        protected float m_MaxRadius;

        // INITIALIZERS: --------------------------------------------------------------------------

        public Character.MovementType Setup(Transform target, float minDistance, float maxDistance)
        {
            this.Setup();

            this.m_ActiveFollow = true;
            this.m_Target = target;

            this.m_LastKnownPosition = this.m_Target != null
                ? this.m_Target.position
                : this.Transform.position;

            this.m_IsFollowing = true;

            this.m_MinRadius = minDistance;
            this.m_MaxRadius = maxDistance;

            return Character.MovementType.None;
        }

        public override Character.MovementType Stop(bool success)
        {
            Character.MovementType movementType = base.Stop(success);
            this.m_ActiveFollow = false;
            
            return movementType;
        }

        // INTERFACE METHODS: ---------------------------------------------------------------------

        public override Character.MovementType Update()
        {
            if (this.m_Target) this.m_LastKnownPosition = this.m_Target.position;

            float distance = Vector3.Distance(this.Transform.position, this.m_LastKnownPosition);
            bool shouldStop = (
                !this.m_Target ||
                !this.m_ActiveFollow ||
                (this.m_IsFollowing && distance <= this.m_MinRadius) ||
                (!this.m_IsFollowing && distance <= this.m_MaxRadius)
            );

            Vector3 direction = this.m_Target.position - this.Transform.position;

            if (shouldStop)
            {
                direction = Vector3.zero;
                direction = this.CalculateSpeed(direction);
                direction = this.CalculateAcceleration(direction);

                this.Motion.MoveDirection = direction;
                this.Motion.MovePosition = this.Transform.TransformDirection(Vector3.forward);

                this.m_IsFollowing = false;

                return direction.sqrMagnitude > float.Epsilon
                    ? Character.MovementType.MoveToDirection
                    : Character.MovementType.None;
            }

            this.m_IsFollowing = true;
            this.Motion.MovePosition = this.m_LastKnownPosition;

            direction = this.CalculateSpeed(direction);
            direction = this.CalculateAcceleration(direction);

            this.Motion.MoveDirection = direction;
            return Character.MovementType.MoveToPosition;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (this.m_ActiveFollow && this.m_Target)
            {
                Gizmos.color = this.m_IsFollowing
                    ? new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f)
                    : new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 1.0f);

                GizmosExtension.Circle(this.m_Target.position, this.m_MaxRadius);

                Gizmos.color = this.m_IsFollowing
                    ? new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 1.0f)
                    : new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f);

                GizmosExtension.Circle(this.m_Target.position, this.m_MinRadius);
            }
        }
    }
}