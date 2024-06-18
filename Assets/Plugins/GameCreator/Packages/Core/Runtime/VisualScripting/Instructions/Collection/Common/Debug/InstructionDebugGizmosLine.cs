using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting 
{
    [Version(0, 1, 1)]

    [Title("Gizmo Line")]
    [Category("Debug/Gizmos/Gizmo Line")]
    
    [Description("Displays a line between two points for a certain time")]

    [Keywords("Debug", "Gizmo", "Draw")]
    [Image(typeof(IconLineStartEnd), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionDebugGizmosLine : Instruction
    {
        [SerializeField] private PropertyGetPosition m_PointA = GetPositionSelf.Create();
        [SerializeField] private PropertyGetPosition m_PointB = GetPositionTarget.Create();

        [SerializeField] private PropertyGetColor m_Color = GetColorColorsGreen.Create;
        [SerializeField] private PropertyGetDecimal m_Duration = GetDecimalConstantZero.Create;
        
        public override string Title => $"Line [{this.m_PointA}, {this.m_PointB}]";

        protected override Task Run(Args args)
        {
            Vector3 pointA = this.m_PointA.Get(args);
            Vector3 pointB = this.m_PointB.Get(args);
            
            float duration = (float) this.m_Duration.Get(args);
            Color color = this.m_Color.Get(args);
            
            Debug.DrawLine(pointA, pointB, color, duration);
            return DefaultResult;
        }
    }
}