//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Thunder and lightning script for random lightning bolts
    /// </summary>
    public class ThunderAndLightningScript : MonoBehaviour
    {
        private class LightningBoltHandler
        {
            public float VolumeMultiplier { get; set; }

            private ThunderAndLightningScript script;
            private readonly System.Random random = new System.Random();

            public LightningBoltHandler(ThunderAndLightningScript script)
            {
                this.script = script;
                CalculateNextLightningTime();
            }

            private void UpdateLighting()
            {
                if (script.lightningInProgress)
                {
                    return;
                }

                if (script.ModifySkyboxExposure)
                {
                    script.skyboxExposureStorm = 0.35f;

                    if (script.skyboxMaterial != null && script.skyboxMaterial.HasProperty("_Exposure"))
                    {
                        script.skyboxMaterial.SetFloat("_Exposure", script.skyboxExposureStorm);
                    }
                }

                CheckForLightning();
            }

            private void CalculateNextLightningTime()
            {
                script.nextLightningTime = DigitalRuby.ThunderAndLightning.LightningBoltScript.TimeSinceStart + script.LightningIntervalTimeRange.Random(random);
                script.lightningInProgress = false;

                if (script.ModifySkyboxExposure && script.skyboxMaterial.HasProperty("_Exposure"))
                {
                    script.skyboxMaterial.SetFloat("_Exposure", script.skyboxExposureStorm);
                }
            }

            public IEnumerator ProcessLightning(Vector3? _start, Vector3? _end, bool intense, bool visible)
            {
                float sleepTime;
                AudioClip[] sounds;
                float intensity;
                script.lightningInProgress = true;

                if (intense)
                {
                    float percent = UnityEngine.Random.Range(0.0f, 1.0f);
                    intensity = Mathf.Lerp(2.0f, 8.0f, percent);
                    sleepTime = 5.0f / intensity;
                    sounds = script.ThunderSoundsIntense;
                }
                else
                {
                    float percent = UnityEngine.Random.Range(0.0f, 1.0f);
                    intensity = Mathf.Lerp(0.0f, 2.0f, percent);
                    sleepTime = 30.0f / intensity;
                    sounds = script.ThunderSoundsNormal;
                }
                if (script.skyboxMaterial != null && script.ModifySkyboxExposure)
                {
                    script.skyboxMaterial.SetFloat("_Exposure", Mathf.Max(intensity * 0.5f, script.skyboxExposureStorm));
                }

                // perform the strike
                Strike(_start, _end, intense, intensity, script.Camera, visible ? script.Camera : null);

                // calculate the next lightning strike
                CalculateNextLightningTime();

                // thunder will play depending on intensity of lightning
                bool playThunder = (intensity >= 1.0f);
                //Debug.Log("Lightning intensity: " + intensity.ToString("0.00") + ", thunder delay: " +
                //          (playThunder ? sleepTime.ToString("0.00") : "No Thunder"));

                if (playThunder && sounds != null && sounds.Length > 0)
                {
                    // wait for a bit then play a thunder sound
                    yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(sleepTime);

                    AudioClip clip = null;
                    do
                    {
                        // pick a random thunder sound that wasn't the same as the last sound, unless there is only one sound, then we have no choice
                        clip = sounds[UnityEngine.Random.Range(0, sounds.Length - 1)];
                    }
                    while (sounds.Length > 1 && clip == script.lastThunderSound);

                    // set the last sound and play it
                    script.lastThunderSound = clip;
                    script.audioSourceThunder.PlayOneShot(clip, intensity * 0.5f * VolumeMultiplier);
                }
            }

            private void Strike(Vector3? _start, Vector3? _end, bool intense, float intensity, Camera camera, Camera visibleInCamera)
            {
                // find a point around the camera that is not too close
                const float minDistance = 500.0f;
                float minValue = (intense ? -1000.0f : -5000.0f);
                float maxValue = (intense ? 1000 : 5000.0f);
                float closestValue = (intense ? 500.0f : 2500.0f);
                float x, y, z;
                Vector3 start, end;
                bool areaBounded = (_start is null && _end is null && script.AreaOveride != null);

                if (areaBounded)
                {
                    var bounds = script.AreaOveride.bounds;
                    x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                    y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
                    z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
                    start = new Vector3(x, y, z);
                }
                else
                {
                    x = (UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(minValue, -closestValue) : UnityEngine.Random.Range(closestValue, maxValue));
                    y = script.LightningYStart;
                    z = (UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(minValue, -closestValue) : UnityEngine.Random.Range(closestValue, maxValue));
                    start = script.Camera.transform.position;
                    start.x += x;
                    start.y = y;
                    start.z += z;

                    if (visibleInCamera != null)
                    {
                        // try and make sure the strike is visible in the camera
                        Quaternion q = visibleInCamera.transform.rotation;
                        visibleInCamera.transform.rotation = Quaternion.Euler(0.0f, q.eulerAngles.y, 0.0f);
                        float screenX = UnityEngine.Random.Range(visibleInCamera.pixelWidth * 0.1f, visibleInCamera.pixelWidth * 0.9f);
                        float ScreenZ = UnityEngine.Random.Range(visibleInCamera.nearClipPlane + closestValue + closestValue, maxValue);
                        Vector3 point = visibleInCamera.ScreenToWorldPoint(new Vector3(screenX, 0.0f, ScreenZ));
                        start = point;
                        start.y = y;
                        visibleInCamera.transform.rotation = q;
                    }
                }

                end = start;

                x = UnityEngine.Random.Range(-100, 100.0f);

                // 1 in 4 chance not to strike the ground
                y = (UnityEngine.Random.Range(0, 4) == 0 ? UnityEngine.Random.Range(-1, 600.0f) : -1.0f);

                z += UnityEngine.Random.Range(-100.0f, 100.0f);

                end.x += x;
                end.y = y;
                end.z += z;

                // make sure the bolt points away from the camera
                end.x += (closestValue * camera.transform.forward.x);
                end.z += (closestValue * camera.transform.forward.z);

                while ((start - end).magnitude < minDistance)
                {
                    end.x += (closestValue * camera.transform.forward.x);
                    end.z += (closestValue * camera.transform.forward.z);
                }

                start = (_start ?? start);
                end = (_end ?? end);

                // see if the bolt hit anything on it's way to the ground - if so, change the end point
                RaycastHit hit;
                if (Physics.Raycast(start, (start - end).normalized, out hit, float.MaxValue))
                {
                    end = hit.point;
                }

                int generations = script.LightningBoltScript.Generations;
                RangeOfFloats trunkWidth = script.LightningBoltScript.TrunkWidthRange;
                if (UnityEngine.Random.value < script.CloudLightningChance)
                {
                    // cloud only lightning
                    script.LightningBoltScript.TrunkWidthRange = new RangeOfFloats();
                    script.LightningBoltScript.Generations = 1;
                }
                script.LightningBoltScript.LightParameters.LightIntensity = intensity * 0.5f;
                script.LightningBoltScript.Trigger(start, end);
                script.LightningBoltScript.TrunkWidthRange = trunkWidth;
                script.LightningBoltScript.Generations = generations;
            }

            private void CheckForLightning()
            {
                // time for another strike?
                if (Time.timeSinceLevelLoad >= script.nextLightningTime)
                {
                    bool intense = UnityEngine.Random.value < script.LightningIntenseProbability;
                    script.StartCoroutine(ProcessLightning(null, null, intense, script.LightningAlwaysVisible));
                }
            }

            public void Update()
            {
                UpdateLighting();
            }
        }

        /// <summary>Lightning bolt script - optional, leave null if you don't want lightning bolts</summary>
        [Tooltip("Lightning bolt script - optional, leave null if you don't want lightning bolts")]
        public LightningBoltPrefabScript LightningBoltScript;

        /// <summary>Camera where the lightning should be centered over. Defaults to main camera.</summary>
        [Tooltip("Camera where the lightning should be centered over. Defaults to main camera.")]
        public Camera Camera;

        /// <summary>Random interval between strikes.</summary>
        [SingleLine("Random interval between strikes.")]
        public RangeOfFloats LightningIntervalTimeRange = new RangeOfFloats { Minimum = 10.0f, Maximum = 25.0f };

        /// <summary>Probability (0-1) of an intense lightning bolt that hits really close. Intense lightning has increased brightness and louder thunder compared to normal lightning, and the thunder sounds plays a lot sooner.</summary>
        [Tooltip("Probability (0-1) of an intense lightning bolt that hits really close. Intense lightning has increased brightness and louder thunder compared to normal lightning, and the thunder sounds plays a lot sooner.")]
        [Range(0.0f, 1.0f)]
        public float LightningIntenseProbability = 0.2f;

        /// <summary>Sounds to play for normal thunder. One will be chosen at random for each lightning strike. Depending on intensity, some normal lightning may not play a thunder sound.</summary>
        [Tooltip("Sounds to play for normal thunder. One will be chosen at random for each lightning strike. Depending on intensity, some normal lightning may not play a thunder sound.")]
        public AudioClip[] ThunderSoundsNormal;

        /// <summary>Sounds to play for intense thunder. One will be chosen at random for each lightning strike.</summary>
        [Tooltip("Sounds to play for intense thunder. One will be chosen at random for each lightning strike.")]
        public AudioClip[] ThunderSoundsIntense;

        /// <summary>Whether lightning strikes should always try to be in the camera view</summary>
        [Tooltip("Whether lightning strikes should always try to be in the camera view")]
        public bool LightningAlwaysVisible = true;

        /// <summary>The chance lightning will simply be in the clouds with no visible bolt</summary>
        [Tooltip("The chance lightning will simply be in the clouds with no visible bolt")]
        [Range(0.0f, 1.0f)]
        public float CloudLightningChance = 0.5f;

        /// <summary>Whether to modify the skybox exposure when lightning is created</summary>
        [Tooltip("Whether to modify the skybox exposure when lightning is created")]
        public bool ModifySkyboxExposure = false;

        /// <summary>Base point light range for lightning bolts. Increases as intensity increases.</summary>
        [Tooltip("Base point light range for lightning bolts. Increases as intensity increases.")]
        [Range(1, 10000)]
        public float BaseLightRange = 2000.0f;

        /// <summary>Starting y value for the lightning strikes</summary>
        [Tooltip("Starting y value for the lightning strikes")]
        [Range(0, 100000)]
        public float LightningYStart = 500.0f;

        /// <summary>Volume multiplier</summary>
        [Tooltip("Volume multiplier")]
        [Range(0.0f, 1.0f)]
        public float VolumeMultiplier = 1.0f;

        /// <summary>Override the lightning strike area, takes all dimensions into account</summary>
        [Tooltip("Override the lightning strike area, takes all dimensions into account")]
        public BoxCollider AreaOveride;

        private float skyboxExposureOriginal;
        private float skyboxExposureStorm;
        private float nextLightningTime;
        private bool lightningInProgress;
        private AudioSource audioSourceThunder;
        private LightningBoltHandler lightningBoltHandler;
        private Material skyboxMaterial;
        private AudioClip lastThunderSound;

        private void Start()
        {
            EnableLightning = true;

            if (Camera == null)
            {
                Camera = Camera.main;
            }

#if DEBUG

            if (Camera.farClipPlane < 10000.0f && !Camera.orthographic)
            {
                Debug.LogWarning("Far clip plane should be 10000+ for best lightning effects");
            }

#endif

            if (RenderSettings.skybox != null)
            {
                skyboxMaterial = RenderSettings.skybox = new Material(RenderSettings.skybox);
            }

            skyboxExposureOriginal = skyboxExposureStorm = (skyboxMaterial == null || !skyboxMaterial.HasProperty("_Exposure") ? 1.0f : skyboxMaterial.GetFloat("_Exposure"));
            audioSourceThunder = gameObject.AddComponent<AudioSource>();
            lightningBoltHandler = new LightningBoltHandler(this);
            lightningBoltHandler.VolumeMultiplier = VolumeMultiplier;
        }

        private void Update()
        {
            if (lightningBoltHandler != null && EnableLightning)
            {
                lightningBoltHandler.VolumeMultiplier = VolumeMultiplier;
                lightningBoltHandler.Update();
            }
        }

        /// <summary>
        /// Summon normal lighntning at a random position
        /// </summary>
        public void CallNormalLightning()
        {
            CallNormalLightning(null, null);
        }

        /// <summary>
        /// Summon normal lightning
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        public void CallNormalLightning(Vector3? start, Vector3? end)
        {
            StartCoroutine(lightningBoltHandler.ProcessLightning(start, end, false, true));
        }

        /// <summary>
        /// Summon intense lightning at random location
        /// </summary>
        public void CallIntenseLightning()
        {
            CallIntenseLightning(null, null);
        }

        /// <summary>
        /// Summon intense lightning
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        public void CallIntenseLightning(Vector3? start, Vector3? end)
        {
            StartCoroutine(lightningBoltHandler.ProcessLightning(start, end, true, true));
        }

        /// <summary>
        /// Skybox exposure original value
        /// </summary>
        public float SkyboxExposureOriginal
        {
            get { return skyboxExposureOriginal; }
        }

        /// <summary>
        /// Whether to enable lightning
        /// </summary>
        public bool EnableLightning { get; set; }
    }
}