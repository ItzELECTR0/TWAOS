using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Remap Coordinates")]
    [Description("Changes each of the components of a Vector3 value")]

    [Category("Math/Geometry/Remap Coordinates")]

    [Parameter("Value", "The Vector3 value affected by the operation")]
    [Parameter("X", "Where the X coordinate component is remapped")]
    [Parameter("Y", "Where the Y coordinate component is remapped")]
    [Parameter("Z", "Where the Z coordinate component is remapped")]

    [Keywords("Change", "Vector3", "Vector2", "Component", "Towards", "Look", "Variable", "Axis")]
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryRemapCoordinates : Instruction
    {
        private enum Remap
        {
            X,
            Y,
            Z,
            Zero,
            One
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;

        [SerializeField] private Remap m_X = Remap.X;
        [SerializeField] private Remap m_Y = Remap.Y;
        [SerializeField] private Remap m_Z = Remap.Z;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Remap {this.m_Set} to ({this.m_X}, {this.m_Y}, {this.m_Z})";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 source = this.m_Set.Get(args);
            Vector3 target = new Vector3(
                this.DoRemap(source, this.m_X),
                this.DoRemap(source, this.m_Y),
                this.DoRemap(source, this.m_Z)
            ); 
            
            this.m_Set.Set(target, args);
            return DefaultResult;
        }

        private float DoRemap(Vector3 vector, Remap operation)
        {
            return operation switch
            {
                Remap.X => vector.x,
                Remap.Y => vector.y,
                Remap.Z => vector.z,
                Remap.Zero => 0f,
                Remap.One => 1f,
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }
    }
}