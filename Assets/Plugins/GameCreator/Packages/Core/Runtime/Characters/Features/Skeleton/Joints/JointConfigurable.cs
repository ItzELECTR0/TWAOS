using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Configurable Joint")]
    [Category("Configurable Joint")]
    
    [Image(typeof(IconJoint), ColorTheme.Type.Yellow)]
    [Description("Use a Configurable Joint component to attach to")]
    
    [Serializable]
    public class JointConfigurable : IJoint
    {
        private const RotationDriveMode ROTATION = RotationDriveMode.Slerp;
        
        private const JointProjectionMode PROJECTION = JointProjectionMode.PositionAndRotation;
        private const float PROJECTION_DISTANCE = 0.5f;
        private const float PROJECTION_ANGLE = 10f;

        private const bool COLLISIONS = false;
        private const bool PREPROCESSING = false;

        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private Bone m_Parent = new Bone();

        [SerializeField] private ConfigurableJointMotion m_LinearMotion = ConfigurableJointMotion.Free;
        [SerializeField] private ConfigurableJointMotion m_AngularMotion = ConfigurableJointMotion.Free;
        
        [SerializeField] private Vector3 m_PrimaryAxis = Vector3.up;
        [SerializeField] private Vector3 m_SecondaryAxis = Vector3.right;

        [SerializeField] private SpringLimit m_SpringLimitX;
        [SerializeField] private SpringLimit m_SpringLimitYZ;

        [SerializeField] private TetherLimit m_LimitXLow;
        [SerializeField] private TetherLimit m_LimitXHigh;
        
        [SerializeField] private TetherLimit m_LimitY;
        [SerializeField] private TetherLimit m_LimitZ;

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public JointConfigurable()
        { }

        public JointConfigurable(
            Bone parent,
            ConfigurableJointMotion linearMotion, 
            ConfigurableJointMotion angularMotion,
            Vector3 primaryAxis, 
            Vector3 secondaryAxis,
            SpringLimit springLimitX, 
            SpringLimit springLimitYZ,
            TetherLimit limitXLow,
            TetherLimit limitXHigh,
            TetherLimit limitY,
            TetherLimit limitZ
        ) : this()
        {
            this.m_Parent = parent;

            this.m_LinearMotion = linearMotion;
            this.m_AngularMotion = angularMotion;

            this.m_PrimaryAxis = primaryAxis;
            this.m_SecondaryAxis = secondaryAxis;

            this.m_SpringLimitX = springLimitX;
            this.m_SpringLimitYZ = springLimitYZ;

            this.m_LimitXLow = limitXLow;
            this.m_LimitXHigh = limitXHigh;
            
            this.m_LimitY = limitY;
            this.m_LimitZ = limitZ;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Joint Setup(GameObject gameObject, Skeleton skeleton, Animator animator)
        {
            ConfigurableJoint joint = gameObject.Get<ConfigurableJoint>();
            if (joint == null) joint = gameObject.Add<ConfigurableJoint>();

            Transform parent = this.m_Parent.GetTransform(animator);
            if (parent != null)
            {
                joint.connectedBody = parent.gameObject.Get<Rigidbody>();
            }
            
            joint.autoConfigureConnectedAnchor = true;

            joint.xMotion = this.m_LinearMotion;
            joint.yMotion = this.m_LinearMotion;
            joint.zMotion = this.m_LinearMotion;
            
            joint.angularXMotion = this.m_AngularMotion;
            joint.angularYMotion = this.m_AngularMotion;
            joint.angularZMotion = this.m_AngularMotion;

            joint.axis = this.m_PrimaryAxis;
            joint.secondaryAxis = this.m_SecondaryAxis;

            joint.angularXLimitSpring = this.m_SpringLimitX.ToJoint();
            joint.angularYZLimitSpring = this.m_SpringLimitYZ.ToJoint();
            
            joint.lowAngularXLimit = this.m_LimitXLow.ToJoint();
            joint.highAngularXLimit = this.m_LimitXHigh.ToJoint();
            
            joint.angularYLimit = this.m_LimitY.ToJoint();
            joint.angularZLimit = this.m_LimitZ.ToJoint();

            joint.rotationDriveMode = ROTATION;
            
            joint.projectionMode = PROJECTION;
            joint.projectionDistance = PROJECTION_DISTANCE;
            joint.projectionAngle = PROJECTION_ANGLE;

            joint.enableCollision = COLLISIONS;
            joint.enablePreprocessing = PREPROCESSING;

            return joint;
        }
        
        public override string ToString()
        {
            return "Configurable Joint";
        }
    }
}