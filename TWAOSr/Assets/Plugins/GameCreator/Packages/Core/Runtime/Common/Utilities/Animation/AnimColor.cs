using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
	public class AnimColor
	{
        private const float SMOOTH = 0.1f;

        // PROPERTIES: ----------------------------------------------------------------------------

		public Color Current { get; set; }
		public Color Target  { get; set; }
		
		public Vector3 SmoothRGB { get; set; }
		public float SmoothAlpha { get; set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public AnimColor(Color value, float smooth = SMOOTH)
		{
			this.Current = value;
			this.Target = value;
			
			this.SmoothRGB = Vector3.one * smooth;
			this.SmoothAlpha = smooth;
		}

        public AnimColor(Color value, Color target, float smooth) : this(value, smooth)
        {
            this.Target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void UpdateWithDelta(float deltaTime)
        {
	        this.Current = new Color(
		        this.UpdateAxis(this.Current.r, this.Target.r, this.SmoothRGB.x, deltaTime),
		        this.UpdateAxis(this.Current.g, this.Target.g, this.SmoothRGB.y, deltaTime),
		        this.UpdateAxis(this.Current.b, this.Target.b, this.SmoothRGB.z, deltaTime),
		        this.UpdateAxis(this.Current.a, this.Target.a, this.SmoothAlpha, deltaTime)
	        );
        }
        
        public void UpdateWithDelta(Color target, float deltaTime)
        {
	        this.Target = target;
	        this.UpdateWithDelta(deltaTime);
        }
        
        public void UpdateWithDelta(Color target, float smooth, float deltaTime)
        {
	        this.SmoothRGB = Vector3.one * smooth;
	        this.SmoothAlpha = smooth;
	        this.UpdateWithDelta(target, deltaTime);
        }
        
        public void Update()
        {
	        float deltaTime = Time.deltaTime;
	        
	        this.Current = new Color(
		        this.UpdateAxis(this.Current.r, this.Target.r, this.SmoothRGB.x, deltaTime),
		        this.UpdateAxis(this.Current.g, this.Target.g, this.SmoothRGB.y, deltaTime),
		        this.UpdateAxis(this.Current.b, this.Target.b, this.SmoothRGB.z, deltaTime),
		        this.UpdateAxis(this.Current.a, this.Target.a, this.SmoothAlpha, deltaTime)
            );
        }

        public void Update(Color target)
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