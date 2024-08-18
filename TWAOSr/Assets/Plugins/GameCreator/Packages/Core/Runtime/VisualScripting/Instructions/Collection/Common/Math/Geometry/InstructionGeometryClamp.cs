using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Clamp")]
    [Description("Clamps all components of a Vector3 between two values")]

    [Category("Math/Geometry/Clamp")]

    [Parameter("Set", "Dynamic variable where the resulting value is set")]
    [Parameter("Value", "The Vector3 value clamped between Minimum and Maximum")]
    [Parameter("Minimum", "The minimum value")]
    [Parameter("Maximum", "The maximum value")]

    [Keywords("Limit", "Vector3", "Vector2", "Constraint", "Variable")]
    [Image(typeof(IconContrast), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryClamp : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetPosition m_Value = new PropertyGetPosition();
        
        [SerializeField] private Vector3 m_Minimum = Vector3.zero;
        [SerializeField] private Vector3 m_Maximum = Vector3.one;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title =>
            $"Clamp {this.m_Set} = {this.m_Value} [{this.m_Minimum}, {this.m_Maximum}]";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 source = this.m_Value.Get(args);
            Vector3 value = new Vector3(
                Mathf.Clamp(source.x, this.m_Minimum.x, this.m_Maximum.x),
                Mathf.Clamp(source.y, this.m_Minimum.y, this.m_Maximum.y),
                Mathf.Clamp(source.z, this.m_Minimum.z, this.m_Maximum.z)
            );

            this.m_Set.Set(value, args);
            return DefaultResult;
        }
    }
}