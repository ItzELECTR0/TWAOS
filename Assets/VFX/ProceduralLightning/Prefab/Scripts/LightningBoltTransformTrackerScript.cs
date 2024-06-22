//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning bolt transform tracker script, keeps lightning anchored properly between two transforms
    /// </summary>
    public class LightningBoltTransformTrackerScript : MonoBehaviour
    {
        /// <summary>The lightning script to track.</summary>
        [Tooltip("The lightning script to track.")]
        public LightningBoltPrefabScript LightningScript;

        /// <summary>The transform to track which will be where the bolts are emitted from.</summary>
        [Tooltip("The transform to track which will be where the bolts are emitted from.")]
        public Transform StartTarget;

        /// <summary>(Optional) The transform to track which will be where the bolts are emitted to. If no end target is specified, lightning will simply move to stay on top of the start target.</summary>
        [Tooltip("(Optional) The transform to track which will be where the bolts are emitted to. If no end target is specified, lightning will simply move to stay on top of the start target.")]
        public Transform EndTarget;

        /// <summary>Scaling limits.</summary>
        [SingleLine("Scaling limits.")]
        public RangeOfFloats ScaleLimit = new RangeOfFloats { Minimum = 0.1f, Maximum = 10.0f };

        private readonly Dictionary<Transform, LightningCustomTransformStateInfo> transformStartPositions = new Dictionary<Transform, LightningCustomTransformStateInfo>();

        private void Start()
        {
            if (LightningScript != null)
            {
                LightningScript.CustomTransformHandler.RemoveAllListeners();
                LightningScript.CustomTransformHandler.AddListener(CustomTransformHandler);
            }
        }

        private static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = (vec2 - vec1).normalized;
            return Vector2.Angle(Vector2.right, diference) * Mathf.Sign(vec2.y - vec1.y);
        }

        private static void UpdateTransform(LightningCustomTransformStateInfo state, LightningBoltPrefabScript script, RangeOfFloats scaleLimit)
        {
            if (state.Transform == null || state.StartTransform == null)
            {
                return;
            }
            else if (state.EndTransform == null)
            {
                // just put the lightning at the start and be done
                state.Transform.position = state.StartTransform.position - state.BoltStartPosition;
                return;
            }

            Quaternion rotation;
            if ((script.CameraMode == CameraMode.Auto && script.Camera.orthographic) ||
                (script.CameraMode == CameraMode.OrthographicXY))
            {
                // 2D rotation delta (xy)
                float startAngle = AngleBetweenVector2(state.BoltStartPosition, state.BoltEndPosition);
                float currentAngle = AngleBetweenVector2(state.StartTransform.position, state.EndTransform.position);
                rotation = Quaternion.AngleAxis((currentAngle - startAngle), Vector3.forward);
            }
            if (script.CameraMode == CameraMode.OrthographicXZ)
            {
                // 2D rotation delta (xz)
                float startAngle = AngleBetweenVector2(new Vector2(state.BoltStartPosition.x, state.BoltStartPosition.z), new Vector2(state.BoltEndPosition.x, state.BoltEndPosition.z));
                float currentAngle = AngleBetweenVector2(new Vector2(state.StartTransform.position.x, state.StartTransform.position.z), new Vector2(state.EndTransform.position.x, state.EndTransform.position.z));
                rotation = Quaternion.AngleAxis((currentAngle - startAngle), Vector3.up);
            }
            else
            {
                // 3D rotation delta
                Quaternion look1 = Quaternion.LookRotation((state.BoltEndPosition - state.BoltStartPosition).normalized);
                Quaternion look2 = Quaternion.LookRotation((state.EndTransform.position - state.StartTransform.position).normalized);
                rotation = look2 * Quaternion.Inverse(look1);
            }
            state.Transform.rotation = rotation;

            // scale based on how much the objects have moved relative to each other
            float startDistance = Vector3.Distance(state.BoltStartPosition, state.BoltEndPosition);
            float endDistance = Vector3.Distance(state.EndTransform.position, state.StartTransform.position);
            float scale = Mathf.Clamp((startDistance < Mathf.Epsilon ? 1.0f : endDistance / startDistance), scaleLimit.Minimum, scaleLimit.Maximum);
            state.Transform.localScale = new Vector3(scale, scale, scale);

            // anchor lightning to start position and account for rotation and scale
            Vector3 offset = rotation * (scale * state.BoltStartPosition);
            state.Transform.position = state.StartTransform.position - offset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void CustomTransformHandler(LightningCustomTransformStateInfo state)
        {
            if (!enabled)
            {
                return;
            }
            else if (LightningScript == null)
            {
                Debug.LogError("LightningScript property must be set to non-null.");
                return;
            }
            else if (state.State == LightningCustomTransformState.Executing)
            {
                UpdateTransform(state, LightningScript, ScaleLimit);
            }
            else if (state.State == LightningCustomTransformState.Started)
            {
                // mark the start and end positions to base rotation and scale changes on
                state.StartTransform = StartTarget;
                state.EndTransform = EndTarget;
                transformStartPositions[transform] = state;
            }
            else
            {
                // remove the transform, this bolt is done
                transformStartPositions.Remove(transform);
            }
        }
    }
}
