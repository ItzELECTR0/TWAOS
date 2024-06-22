//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

// comment out to disable lightning path lines
#define SHOW_LIGHTNING_PATH

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Base lghtning bolt path script
    /// </summary>
    public abstract class LightningBoltPathScriptBase : LightningBoltPrefabScriptBase
    {
        /// <summary>The game objects to follow for the lightning path</summary>
        [Header("Lightning Path Properties")]
        [Tooltip("The game objects to follow for the lightning path")]
        public List<GameObject> LightningPath;
        private readonly List<GameObject> currentPathObjects = new List<GameObject>();

#if UNITY_EDITOR

        [System.NonSerialized]
        private bool gizmosCleanedUp;

        [System.NonSerialized]
        private readonly List<GameObject> lastGizmos = new List<GameObject>();

        private void DoGizmoCleanup()
        {
            if (gizmosCleanedUp)
            {
                return;
            }

            gizmosCleanedUp = true;

            foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(LightningGizmoScript)))
            {
                LightningGizmoScript s;
                s = obj as LightningGizmoScript;
                if (s == null)
                {
                    GameObject gObj = obj as GameObject;
                    if (gObj != null)
                    {
                        s = gObj.GetComponent<LightningGizmoScript>();
                    }
                }
                if (s != null)
                {
                    GameObject.DestroyImmediate(s, true);
                }
            }
        }

        /// <summary>
        /// OnDrawGizmos
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DoGizmoCleanup();

            if (HideGizmos)
            {
                return;
            }

#if SHOW_LIGHTNING_PATH

            bool noLightningPath = (LightningPath == null || LightningPath.Count == 0);

#else

            bool noLightningPath = true;            

#endif

            // remove any objects that were taken out of the list and cleanup the gizmo script
            for (int i = lastGizmos.Count - 1; i >= 0; i--)
            {
                if (noLightningPath || !LightningPath.Contains(lastGizmos[i]))
                {
                    if (lastGizmos[i] != null)
                    {
                        LightningGizmoScript s = lastGizmos[i].GetComponent<LightningGizmoScript>();
                        if (s != null)
                        {
                            GameObject.DestroyImmediate(s, true);
                        }
                    }
                    lastGizmos.RemoveAt(i);
                }
            }

            // no objects, we are done
            if (noLightningPath)
            {
                return;
            }

            // add gizmo scripts and draw lines as needed
            Vector3 gizmoPosition;
            HashSet<GameObject> gizmos = new HashSet<GameObject>();
            Vector3? previousPoint = null;
            LightningGizmoScript gizmoScript;
            lastGizmos.Clear();

            for (int index = 0; index < LightningPath.Count; index++)
            {
                GameObject o = LightningPath[index];
                if (o == null || !o.activeInHierarchy)
                {
                    continue;
                }
                else if ((gizmoScript = o.GetComponent<LightningGizmoScript>()) == null)
                {
                    // we need to add the gizmo script so that this object can be selectable by tapping on the lightning bolt in the scene view
                    gizmoScript = o.AddComponent<LightningGizmoScript>();
                }
                gizmoScript.hideFlags = HideFlags.HideInInspector;

                // setup label based on whether we've seen this one before
                if (gizmos.Add(o))
                {
                    gizmoScript.Label = index.ToString();
                }
                else
                {
                    gizmoScript.Label += ", " + index.ToString();
                }
                gizmoScript.LightningBoltScript = this;

                gizmoPosition = o.transform.position;
                if (previousPoint != null && previousPoint.Value != gizmoPosition)
                {
                    // draw a line and arrow in the proper direction
                    Gizmos.DrawLine(previousPoint.Value, gizmoPosition);
                    Vector3 direction = (gizmoPosition - previousPoint.Value);
                    Vector3 center = (previousPoint.Value + gizmoPosition) * 0.5f;
                    float arrowSize = Mathf.Min(1.0f, direction.magnitude) * 2.0f;
                    UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(direction), arrowSize, EventType.Repaint);
                }

                previousPoint = gizmoPosition;
                lastGizmos.Add(o);
            }
        }

#endif

        /// <summary>
        /// Get the game objects in the path currently - null or inactive objects are not returned
        /// </summary>
        /// <returns>List of game objects in the path</returns>
        protected List<GameObject> GetCurrentPathObjects()
        {
            currentPathObjects.Clear();
            if (LightningPath != null)
            {
                foreach (GameObject obj in LightningPath)
                {
                    if (obj != null && obj.activeInHierarchy)
                    {
                        currentPathObjects.Add(obj);
                    }
                }
            }
            return currentPathObjects;
        }

        /// <summary>
        /// Create lightning bolt path parameters
        /// </summary>
        /// <returns>Lightning bolt path parameters</returns>
        protected override LightningBoltParameters OnCreateParameters()
        {
            LightningBoltParameters p = base.OnCreateParameters();
            p.Generator = LightningGeneratorPath.GeneratorInstance;
            return p;
        }
    }

    /// <summary>
    /// Lightning bolt path script implementation
    /// </summary>
    public class LightningBoltPathScript : LightningBoltPathScriptBase
    {
        /// <summary>How fast the lightning moves through the points or objects. 1 is normal speed, 0.01 is slower, so the lightning will move slowly between the points or objects.</summary>
        [Tooltip("How fast the lightning moves through the points or objects. 1 is normal speed, " +
            "0.01 is slower, so the lightning will move slowly between the points or objects.")]
        [Range(0.01f, 1.0f)]
        public float Speed = 1.0f;

        /// <summary>When each new point is moved to, this can provide a random value to make the movement to the next point appear more staggered or random. Leave as 1 and 1 to have constant speed. Use a higher maximum to create more randomness.</summary>
        [Tooltip("Repeat when the path completes?")]
        [SingleLineClamp("When each new point is moved to, this can provide a random value to make the movement to " +
            "the next point appear more staggered or random. Leave as 1 and 1 to have constant speed. Use a higher " +
            "maximum to create more randomness.", 1.0, 500.0)]
        public RangeOfFloats SpeedIntervalRange = new RangeOfFloats { Minimum = 1.0f, Maximum = 1.0f };

        /// <summary>Repeat when the path completes?</summary>
        [Tooltip("Repeat when the path completes?")]
        public bool Repeat = true;

        private float nextInterval = 1.0f;
        private int nextIndex;
        private Vector3? lastPoint;

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            Vector3? currentPoint = null;
            List<GameObject> lightningPath = GetCurrentPathObjects();

            if (lightningPath.Count < 2)
            {
                return;
            }
            else if (nextIndex >= lightningPath.Count)
            {
                if (!Repeat)
                {
                    return;
                }
                else if (lightningPath[lightningPath.Count - 1] == lightningPath[0])
                {
                    nextIndex = 1;
                }
                else
                {
                    nextIndex = 0;
                    lastPoint = null;
                }
            }
            try
            {
                if (lastPoint == null)
                {
                    lastPoint = lightningPath[nextIndex++].transform.position;
                }
                currentPoint = lightningPath[nextIndex].transform.position;
                if (lastPoint != null && currentPoint != null)
                {
                    parameters.Start = lastPoint.Value;
                    parameters.End = currentPoint.Value;
                    base.CreateLightningBolt(parameters);

                    if ((nextInterval -= Speed) <= 0.0f)
                    {
                        float speedValue = UnityEngine.Random.Range(SpeedIntervalRange.Minimum, SpeedIntervalRange.Maximum);
                        nextInterval = speedValue + nextInterval;
                        lastPoint = currentPoint;
                        nextIndex++;
                    }
                }
            }
            catch (System.NullReferenceException)
            {
                // null reference exception can happen here, in which case we bail as the list is broken until the null object gets taken out
            }
        }

        /// <summary>
        /// Reset everything
        /// </summary>
        public void Reset()
        {
            lastPoint = null;
            nextIndex = 0;
            nextInterval = 1.0f;
        }
    }
}
