using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Transform to World Point")]
    [Description("Transform the Point from Local to World space")]

    [Category("Math/Geometry/Transform to World Point")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Transform", "The reference object to transform the coordinates")]
    [Parameter("Point", "The point that changes its space mode")]
    
    [Keywords("Location", "Position", "Local", "World", "Space", "Variable")]
    [Image(typeof(IconLocation), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionGeometryTransformPoint : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;

        [SerializeField]
        private PropertyGetGameObject m_Transform = GetGameObjectTransform.Create();

        [SerializeField] 
        private PropertyGetPosition m_Position = new PropertyGetPosition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => string.Format(
            "Set {0} = {2} to {1} World Space", 
            this.m_Set,
            this.m_Transform,
            this.m_Position
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Transform transform = this.m_Transform.Get<Transform>(args);
            if (transform == null) return DefaultResult;

            Vector3 value = transform.TransformPoint(this.m_Position.Get(args));
            this.m_Set.Set(value, args);
            
            return DefaultResult;
        }
    }
}