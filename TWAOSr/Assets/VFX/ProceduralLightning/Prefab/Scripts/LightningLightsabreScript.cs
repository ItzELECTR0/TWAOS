using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning bolt script that acts like a laser sword
    /// </summary>
    public class LightningLightsabreScript : LightningBoltPrefabScript
    {
        /// <summary>Height of the blade</summary>
        [Header("Lightsabre Properties")]
        [Tooltip("Height of the blade")]
        public float BladeHeight = 19.0f;

        /// <summary>How long it takes to turn the lightsabre on and off</summary>
        [Tooltip("How long it takes to turn the lightsabre on and off")]
        public float ActivationTime = 0.5f;

        /// <summary>Sound to play when the lightsabre turns on</summary>
        [Tooltip("Sound to play when the lightsabre turns on")]
        public AudioSource StartSound;

        /// <summary>Sound to play when the lightsabre turns off</summary>
        [Tooltip("Sound to play when the lightsabre turns off")]
        public AudioSource StopSound;

        /// <summary>Sound to play when the lightsabre stays on</summary>
        [Tooltip("Sound to play when the lightsabre stays on")]
        public AudioSource ConstantSound;

        private int state; // 0 = off, 1 = on, 2 = turning off, 3 = turning on
        private Vector3 bladeStart;
        private Vector3 bladeDir;
        private float bladeTime;
        private float bladeIntensity;

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (state == 2 || state == 3)
            {
                bladeTime += LightningBoltScript.DeltaTime;
                float percent = Mathf.Lerp(0.01f, 1.0f, bladeTime / ActivationTime);
                Vector3 end = bladeStart + (bladeDir * percent * BladeHeight);
                Destination.transform.position = end;
                GlowIntensity = bladeIntensity * (state == 3 ? percent : (1.0f - percent));

                if (bladeTime >= ActivationTime)
                {
                    GlowIntensity = bladeIntensity;
                    bladeTime = 0.0f;
                    if (state == 2)
                    {
                        ManualMode = true;
                        state = 0;
                    }
                    else
                    {
                        state = 1;
                    }
                }
            }
            base.Update();
        }

        /// <summary>
        /// True to turn on the lightsabre, false to turn it off
        /// </summary>
        /// <param name="value">Whether the lightsabre is on or off</param>
        /// <returns>True if success, false if invalid operation (i.e. lightsabre is already on or off)</returns>
        public bool TurnOn(bool value)
        {
            if (state == 2 || state == 3 || (state == 1 && value) || (state == 0 && !value))
            {
                return false;
            }
            bladeStart = Destination.transform.position;
            ManualMode = false;
            bladeIntensity = GlowIntensity;

            if (value)
            {
                bladeDir = (Camera.orthographic ? transform.up : transform.forward);
                state = 3;
                StartSound.Play();
                StopSound.Stop();
                ConstantSound.Play();
            }
            else
            {
                bladeDir = -(Camera.orthographic ? transform.up : transform.forward);
                state = 2;
                StartSound.Stop();
                StopSound.Play();
                ConstantSound.Stop();
            }

            return true;
        }

        /// <summary>
        /// Convenience method to turn lightsabre on / off from Unity GUI
        /// </summary>
        /// <param name="value">Value</param>
        public void TurnOnGUI(bool value)
        {
            TurnOn(value);
        }
    }
}
