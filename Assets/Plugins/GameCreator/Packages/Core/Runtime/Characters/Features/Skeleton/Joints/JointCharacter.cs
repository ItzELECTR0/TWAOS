using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Joint")]
    [Category("Character Joint")]
    
    [Image(typeof(IconJoint), ColorTheme.Type.Green)]
    [Description("Use a Character Joint component to attach to")]
    
    [Serializable]
    public class JointCharacter : IJoint
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private Bone m_Parent = new Bone();
        
        [SerializeField] private Vector3 m_TwistAxis = Vector3.forward;
        [SerializeField] private Vector3 m_SwingAxis = Vector3.up;

        [SerializeField] private float m_LowTwistLimit = 0f;
        [SerializeField] private float m_HighTwistLimit = 0f;
        
        [SerializeField] private float m_LowSwingLimit = 0f;
        [SerializeField] private float m_HighSwingLimit = 0f;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public JointCharacter()
        { }

        public JointCharacter(HumanBodyBones parent, Vector3 twist, Vector3 swing,
            Vector2 twistLimit, Vector2 swingLimit) : this()
        {
            this.m_Parent = new Bone(parent);
            
            this.m_TwistAxis = twist;
            this.m_SwingAxis = swing;

            this.m_LowTwistLimit = twistLimit.x;
            this.m_HighTwistLimit = twistLimit.y;

            this.m_LowSwingLimit = swingLimit.x;
            this.m_HighSwingLimit = swingLimit.y;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public Joint Setup(GameObject gameObject, Skeleton skeleton, Animator animator)
        {
            Transform connection = this.m_Parent.GetTransform(animator);
            if (connection == null) return null;
            
            CharacterJoint joint = gameObject.Get<CharacterJoint>();
            if (joint == null) joint = gameObject.Add<CharacterJoint>();

            joint.enableProjection = true;
            joint.connectedBody = connection.gameObject.Get<Rigidbody>();

            Vector3 twistAxis = gameObject.transform.InverseTransformDirection(this.m_TwistAxis);
            Vector3 swingAxis = gameObject.transform.InverseTransformDirection(this.m_SwingAxis);
            
            joint.axis = this.CalculateDirectionAxis(twistAxis);
            joint.swingAxis = this.CalculateDirectionAxis(swingAxis);

            joint.lowTwistLimit = new SoftJointLimit { limit = this.m_LowTwistLimit };
            joint.highTwistLimit = new SoftJointLimit { limit = this.m_HighTwistLimit };

            joint.swing1Limit = new SoftJointLimit { limit = this.m_LowSwingLimit };
            joint.swing2Limit = new SoftJointLimit { limit = this.m_HighSwingLimit };

            return joint;
        }
        
        public override string ToString()
        {
            return "Character Joint";
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private Vector3 CalculateDirectionAxis(Vector3 point)
        {
            this.CalculateDirection(point, out int direction, out float distance);
            
            Vector3 axis = Vector3.zero;
            if (distance > 0) axis[direction] = 1.0f;
            else axis[direction] = -1.0f;
            
            return axis;
        }
        
        private void CalculateDirection(Vector3 point, out int direction, out float distance)
        {
            direction = 0;
            if (Mathf.Abs(point[1]) > Mathf.Abs(point[0])) direction = 1;
            if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction])) direction = 2;

            distance = point[direction];
        }
    }
}