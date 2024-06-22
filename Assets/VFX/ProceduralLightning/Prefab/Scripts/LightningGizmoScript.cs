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
    /// Draws lightning bolt script gizmos
    /// </summary>
    public class LightningGizmoScript : MonoBehaviour
    {

#if UNITY_EDITOR

        /// <summary>
        /// Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Lightning bolt script
        /// </summary>
        public LightningBoltScript LightningBoltScript { get; set; }

        private static readonly Vector3 labelOffset = Vector3.up * 1.5f;
        private static GUIStyle style;

        private void OnDrawGizmos()
        {
            if (Label == null || (LightningBoltScript != null && LightningBoltScript.HideGizmos))
            {
                return;
            }
            else if (style == null)
            {
                style = new GUIStyle();
                style.fontSize = 14;
                style.fontStyle = FontStyle.Normal;
                style.normal.textColor = Color.white;
            }
            Vector3 v = gameObject.transform.position;
            if ((Label == "0" || Label.StartsWith("0,")))
            {
                Gizmos.DrawIcon(v, "LightningPathStart.png");
            }
            else
            {
                Gizmos.DrawIcon(v, "LightningPathNext.png");
            }
            UnityEditor.Handles.Label(v + labelOffset, Label, style);
        }

#endif

    }
}