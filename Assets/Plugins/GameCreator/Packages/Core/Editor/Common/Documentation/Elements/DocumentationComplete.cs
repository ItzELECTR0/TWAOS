using System;

namespace GameCreator.Editor.Common
{
    public class DocumentationComplete : DocumentationBaseElement
    {
        public DocumentationComplete(Type type) : base(type)
        {
            this.IncludeHeader(this, false);
            this.IncludeRenderPipelines(this, true);
            this.IncludeDescription(this, true);
            this.IncludeCategory(this, true);
            this.IncludeVersion(this, true);

            this.IncludeParameters(this, true);
            this.IncludeExamples(this, true);
            this.IncludeDependencies(this, true);

            this.IncludeKeywords(this, true);
        }
    }
}