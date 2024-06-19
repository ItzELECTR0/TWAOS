namespace UnityEngine.Animations.Rigging
{
    /// <summary>
    /// The MultiPosition constraint job.
    /// </summary>
    [System.Serializable]
    public struct MultiPositionConstraintData : IAnimationJobData, IMultiPositionConstraintData
    {
        [SerializeField] Transform m_ConstrainedObject;

        [SyncSceneToStream, SerializeField, WeightRange(0f, 1f)] WeightedTransformArray m_SourceObjects;
        [SyncSceneToStream, SerializeField] Vector3 m_Offset;

        [NotKeyable, SerializeField] Vector3Bool m_ConstrainedAxes;
        [NotKeyable, SerializeField] bool m_MaintainOffset;

        /// <inheritdoc />
        public Transform constrainedObject { get => m_ConstrainedObject; set => m_ConstrainedObject = value; }

        /// <inheritdoc />
        public WeightedTransformArray sourceObjects
        {
            get => m_SourceObjects;
            set => m_SourceObjects = value;
        }

        /// <inheritdoc />
        public bool maintainOffset { get => m_MaintainOffset; set => m_MaintainOffset = value; }

        /// <summary>Post-Translation offset applied to the constrained Transform.</summary>
        public Vector3 offset { get => m_Offset; set => m_Offset = value; }

        /// <inheritdoc />
        public bool constrainedXAxis { get => m_ConstrainedAxes.x; set => m_ConstrainedAxes.x = value; }
        /// <inheritdoc />
        public bool constrainedYAxis { get => m_ConstrainedAxes.y; set => m_ConstrainedAxes.y = value; }
        /// <inheritdoc />
        public bool constrainedZAxis { get => m_ConstrainedAxes.z; set => m_ConstrainedAxes.z = value; }

        /// <inheritdoc />
        string IMultiPositionConstraintData.offsetVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_Offset));
        /// <inheritdoc />
        string IMultiPositionConstraintData.sourceObjectsProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_SourceObjects));

        /// <inheritdoc />
        bool IAnimationJobData.IsValid()
        {
            if (m_ConstrainedObject == null || m_SourceObjects.Count == 0)
                return false;

            foreach (var src in m_SourceObjects)
                if (src.transform == null)
                    return false;

            return true;
        }

        /// <inheritdoc />
        void IAnimationJobData.SetDefaultValues()
        {
            m_ConstrainedObject = null;
            m_ConstrainedAxes = new Vector3Bool(true);
            m_SourceObjects.Clear();
            m_MaintainOffset = false;
            m_Offset = Vector3.zero;
        }
    }

    /// <summary>
    /// MultiPosition constraint.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Multi-Position Constraint")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/MultiPositionConstraint.html")]
    public class MultiPositionConstraint : RigConstraint<
        MultiPositionConstraintJob,
        MultiPositionConstraintData,
        MultiPositionConstraintJobBinder<MultiPositionConstraintData>
        >
    {
        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();
            var weights = m_Data.sourceObjects;
            WeightedTransformArray.OnValidate(ref weights);
            m_Data.sourceObjects = weights;
        }
    }
}
