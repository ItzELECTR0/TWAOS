using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
	public class AnimVector3
	{
        private const float SMOOTH = 0.1f;

        // PROPERTIES: ----------------------------------------------------------------------------

		public Vector3 Current { get; set; }
		public Vector3 Target  { get; set; }
		public Vector3 Smooth  { get; set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public AnimVector3(Vector3 value, float smooth = SMOOTH)
		{
			this.Current = value;
			this.Target = value;
			this.Smooth = Vector3.one * smooth;
		}

        public AnimVector3(Vector3 value, Vector3 target, float smooth) : this(value, smooth)
        {
            this.Target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void UpdateWithDelta(float deltaTime)
        {
	        this.Current = new Vector3(
		        this.UpdateAxis(this.Current.x, this.Target.x, this.Smooth.x, deltaTime),
		        this.UpdateAxis(this.Current.y, this.Target.y, this.Smooth.y, deltaTime),
		        this.UpdateAxis(this.Current.z, this.Target.z, this.Smooth.z, deltaTime)
	        );
        }
        
        public void UpdateWithDelta(Vector3 target, float deltaTime)
        {
	        this.Target = target;
	        this.UpdateWithDelta(deltaTime);
        }
        
        public void UpdateWithDelta(Vector3 target, Vector3 smooth, float deltaTime)
        {
	        this.Smooth = smooth;
	        this.UpdateWithDelta(target, deltaTime);
        }
        
        public void Update()
        {
	        float deltaTime = Time.deltaTime;
	        
	        this.Current = new Vector3(
                this.UpdateAxis(this.Current.x, this.Target.x, this.Smooth.x, deltaTime),
                this.UpdateAxis(this.Current.y, this.Target.y, this.Smooth.y, deltaTime),
                this.UpdateAxis(this.Current.z, this.Target.z, this.Smooth.z, deltaTime)
            );
        }

        public void Update(Vector3 target)
        {
            this.Target = target;
            this.Update();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private float UpdateAxis(float current, float target, float smooth, float deltaTime)
        {
	        if (smooth <= 0.001f) return target;
	        
            float sign = Math.Sign(target - current);
            current += deltaTime * sign / smooth;

            if (sign <= 0f) current = Math.Max(current, target);
            if (sign >= 0f) current = Math.Min(current, target);

            return current;
        }
	}
}