using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
	public struct TimeMode
	{
        public enum UpdateMode
		{
            GameTime,
            UnscaledTime
		}

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private UpdateMode m_UpdateTime;

        // PROPERTIES: ----------------------------------------------------------------------------

        public UpdateMode UpdateTime => this.m_UpdateTime;

        public float Time => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.time
            : UnityEngine.Time.unscaledTime;

        public float DeltaTime => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.deltaTime
            : UnityEngine.Time.unscaledDeltaTime;

        public float FixedTime => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.fixedTime
            : UnityEngine.Time.fixedUnscaledTime;

        public float FixedDeltaTime => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.fixedDeltaTime
            : UnityEngine.Time.fixedUnscaledDeltaTime;

        public float TimeScale => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.timeScale
            : 1f;

        public int Frame => UnityEngine.Time.frameCount;
        public int RenderedFrame => UnityEngine.Time.renderedFrameCount;
        
        // DOUBLE TIMES: --------------------------------------------------------------------------
        
        public double TimeAsDouble => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.timeAsDouble
            : UnityEngine.Time.unscaledTimeAsDouble;

        public double FixedTimeAsDouble => this.m_UpdateTime == UpdateMode.GameTime
            ? UnityEngine.Time.fixedTimeAsDouble
            : UnityEngine.Time.fixedUnscaledTimeAsDouble;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TimeMode(UpdateMode mode)
		{
            this.m_UpdateTime = mode;
		}
	}
}