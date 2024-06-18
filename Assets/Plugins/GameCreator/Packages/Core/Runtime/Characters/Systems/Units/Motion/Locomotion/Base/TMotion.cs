using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public abstract class TMotion
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected TUnitMotion Motion { get; private set; }

        protected Character Character { get; private set; }
        protected Transform Transform { get; private set; }

        public int Priority { get; internal set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Initialize(TUnitMotion motion, int priority)
        {
            this.Priority = priority;
            this.Motion = motion;

            this.Character = motion.Character;
            this.Transform = motion.Transform;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        public abstract Character.MovementType Update();

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual Character.MovementType Stop(bool success)
        {
            this.Priority = -1;
            
            this.Motion.MoveDirection = Vector3.zero;
            this.Motion.MovePosition = this.Transform.position;

            return Character.MovementType.None;
        }

        public virtual void OnDrawGizmos()
        { }

        protected virtual void Setup()
        { }

        // PROTECTED METHOD: ----------------------------------------------------------------------

        protected Vector3 CalculateSpeed(Vector3 direction)
        {
            direction = direction.normalized * this.Motion.LinearSpeed;
            return direction;
        }

        protected Vector3 CalculateAcceleration(Vector3 tarDirection)
        {
            if (!this.Motion.UseAcceleration) return tarDirection;
        
            Vector3 curDirection = this.Character.Motion.MoveDirection;
            
            if (tarDirection.sqrMagnitude < 0.01f) tarDirection = Vector3.zero;
            bool isIncreasing = curDirection.sqrMagnitude < tarDirection.sqrMagnitude;
            
            float acceleration = isIncreasing
                ? this.Motion.Acceleration
                : this.Motion.Deceleration;
        
            curDirection = Vector3.Lerp(
                curDirection, 
                tarDirection, 
                acceleration * this.Character.Time.DeltaTime
            );
        
            if (isIncreasing)
            {
                Vector3 projection = Vector3.Project(curDirection, tarDirection);
                curDirection = projection.sqrMagnitude < tarDirection.sqrMagnitude
                    ? curDirection
                    : tarDirection;                
            }
            else
            {
                float curMagnitude = curDirection.sqrMagnitude;
                float tarMagnitude = tarDirection.sqrMagnitude;
                curDirection = Mathf.Abs(curMagnitude) > 0.01f || Mathf.Abs(tarMagnitude) > 0.01f
                    ? curDirection
                    : Vector3.zero;
            }
        
            return curDirection;
        }
    }
}