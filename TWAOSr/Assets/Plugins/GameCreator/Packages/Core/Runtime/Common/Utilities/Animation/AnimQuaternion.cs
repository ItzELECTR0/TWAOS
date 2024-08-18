using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
	public class AnimQuaternion
	{
        private const float SMOOTH = 0.1f;

        // PROPERTIES: ----------------------------------------------------------------------------

		public Quaternion Current { get; set; }
		public Quaternion Target  { get; set; }
		
		public float Smooth  { get; set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public AnimQuaternion(Quaternion value, float smooth = SMOOTH)
		{
			this.Current = value;
			this.Target = value;
			this.Smooth = smooth;
		}

        public AnimQuaternion(Quaternion value, Quaternion target, float smooth) : this(value, smooth)
        {
            this.Target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void UpdateWithDelta(float deltaTime)
        {
	        this.Current = UpdateRotation(this.Current, this.Target, deltaTime);
        }
        
        public void UpdateWithDelta(Quaternion target, float deltaTime)
        {
	        this.Target = target;
	        this.UpdateWithDelta(deltaTime);
        }
        
        public void UpdateWithDelta(Quaternion target, float smooth, float deltaTime)
        {
	        this.Smooth = smooth;
	        this.UpdateWithDelta(target, deltaTime);
        }
        
        public void Update()
        {
	        float deltaTime = Time.deltaTime;
	        this.Current = UpdateRotation(this.Current, this.Target, deltaTime);
        }

        public void Update(Quaternion target)
        {
            this.Target = target;
            this.Update();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Quaternion UpdateRotation(Quaternion from, Quaternion to, float deltaTime)
        {
	        return this.Smooth > 0.001f
		        ? Quaternion.RotateTowards(from, to, deltaTime / this.Smooth) 
		        : to;
        }
	}
}