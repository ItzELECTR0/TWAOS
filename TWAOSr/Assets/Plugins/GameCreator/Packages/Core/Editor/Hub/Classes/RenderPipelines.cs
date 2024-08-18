using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class RenderPipelines
    {
        public bool builtin;
        public bool urp;
        public bool hdrp;
        
        public RenderPipelines()
        {
            this.builtin = false;
            this.urp = false;
            this.hdrp = false;
        }

        public RenderPipelines(bool builtin, bool urp, bool hdrp)
        {
            this.builtin = builtin;
            this.urp = urp;
            this.hdrp = hdrp;
        }
    }
}