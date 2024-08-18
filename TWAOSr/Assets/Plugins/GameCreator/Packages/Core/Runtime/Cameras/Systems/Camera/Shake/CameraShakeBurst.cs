namespace GameCreator.Runtime.Cameras
{
    internal class CameraShakeBurst : TCameraShake
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public void AddBurst(float delay, float duration, ShakeEffect shakeEffect)
        {
            ShakeSystem shakeSystem = ShakeSystem.Burst(delay, duration, shakeEffect);
            this.m_ShakeSystems.Add(shakeSystem);
        }

        public void RemoveBursts(float delay, float transition)
        {
            foreach (ShakeSystem shakeSystem in this.m_ShakeSystems)
            {
                shakeSystem.Stop(delay, transition);
            }
        }
    }
}