using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class RenderPipelineAttribute : Attribute
    {
        public bool Builtin { get; }
        public bool URP { get; }
        public bool HDRP { get; }

        public RenderPipelineAttribute(bool builtin, bool universal, bool highDefinition)
        {
            this.Builtin = builtin;
            this.URP = universal;
            this.HDRP = highDefinition;
        }

        public override string ToString()
        {
            List<string> support = new List<string>();
            
            if (this.Builtin) support.Add("Built-in");
            if (this.URP) support.Add("URP");
            if (this.HDRP) support.Add("HDRP");
            
            return string.Join(",", support.Select(renderingPipeline => renderingPipeline));
        }
    }
}
