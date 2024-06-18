using GameCreator.Editor.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public sealed class DocumentationHub : VisualElement
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private readonly HitData m_Hit;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public DocumentationHub(HitData hit)
        {
            this.m_Hit = hit;
            if (this.m_Hit == null) return;
            
            StyleSheet[] styleSheetsSet = StyleSheetUtils.Load(DocumentationBaseElement.PATH_USS);
            foreach (StyleSheet styleSheet in styleSheetsSet) this.styleSheets.Add(styleSheet);

            this.IncludeTitle(this, false);
            this.IncludeRenderPipelines(this, true);
            this.IncludeDescription(this, true);
            this.IncludeVersion(this, true);
            this.IncludeCategory(this, true);

            this.IncludeParameters(this, true);
            this.IncludeExamples(this, true);
            this.IncludeDependencies(this, true);
            
            this.IncludeKeywords(this, true);
        }

        // BUILDER METHODS: -----------------------------------------------------------------------

        private void IncludeSeparator(VisualElement parent)
        {
            VisualElement separator = new VisualElement
            {
                name = DocumentationBaseElement.NAME_SEPARATOR_LARGE
            };
            
            parent.Add(separator);
        }

        private void IncludeTitle(VisualElement parent, bool prefixSeparator)
        {
            Label label = new Label
            {
                name = DocumentationBaseElement.NAME_TITLE,
                text = this.m_Hit.name
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        private void IncludeDescription(VisualElement parent, bool prefixSeparator)
        {
            if (string.IsNullOrEmpty(this.m_Hit.description)) return;
            
            Label label = new Label
            {
                name = DocumentationBaseElement.NAME_DESCRIPTION,
                text = this.m_Hit.description
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        private void IncludeCategory(VisualElement parent, bool prefixSeparator)
        {
            string category = this.m_Hit.category;
            if (string.IsNullOrEmpty(category)) return;
            
            Label label = new Label
            {
                name = DocumentationBaseElement.NAME_CATEGORY,
                text = category.Replace("/", DocumentationBaseElement.CATEGORY_SEPARATOR)
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        private void IncludeVersion(VisualElement parent, bool prefixSeparator)
        {
            string version = this.m_Hit.version.ToString();
            if (string.IsNullOrEmpty(version)) return;
            
            Label label = new Label
            {
                name = DocumentationBaseElement.NAME_VERSION,
                text = $"Latest version: {version}"
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        private void IncludeParameters(VisualElement parent, bool prefixSeparator)
        {
            if (this.m_Hit.parameters.Length == 0) return;
        
            VisualElement container = new VisualElement
            {
                name = DocumentationBaseElement.NAME_PARAMETERS
            };
        
            for (int i = 0; i < this.m_Hit.parameters.Length; ++i)
            {
                VisualElement content = new VisualElement
                {
                    name = DocumentationBaseElement.NAME_PARAMETER
                };
                
                content.AddToClassList(i % 2 == 0 
                    ? DocumentationBaseElement.ROW_EVEN 
                    : DocumentationBaseElement.ROW_ODD);
                
                content.Add(new Label
                {
                    name = DocumentationBaseElement.NAME_PARAMETER_TITLE,
                    text = this.m_Hit.parameters[i].name 
                });
                
                content.Add(new Label
                {
                    name = DocumentationBaseElement.NAME_PARAMETER_DESCR,
                    text = this.m_Hit.parameters[i].description
                });
                
                container.Add(content);
            }
        
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }

        private void IncludeExamples(VisualElement parent, bool prefixSeparator)
        {
            // if (this.m_Hit.examples.Length == 0) return;
            //
            // VisualElement container = new VisualElement
            // {
            //     name = DocumentationBaseElement.NAME_EXAMPLES
            // };
            //
            // for (int i = 0; i < this.m_Hit.examples.Length; i++)
            // {
            //     container.Add(new Label
            //     {
            //         name = DocumentationBaseElement.NAME_EXAMPLE_TITLE,
            //         text = $"Example {i + 1}"
            //     });
            //     
            //     container.Add(new Label
            //     {
            //         name = DocumentationBaseElement.NAME_EXAMPLE_CONTENT,
            //         text = this.m_Hit.examples[i]
            //     });
            // }
            //
            // if (prefixSeparator) this.IncludeSeparator(parent);
            // parent.Add(container);
        }

        private void IncludeDependencies(VisualElement parent, bool prefixSeparator)
        {
            if (this.m_Hit.dependencies.Length == 0) return;
        
            VisualElement container = new VisualElement
            {
                name = DocumentationBaseElement.NAME_DEPENDENCIES
            };
        
            for (int i = 0; i < this.m_Hit.dependencies.Length; ++i)
            {
                string id = this.m_Hit.dependencies[i].id;
                string version = string.Format(
                    "{0}.{1}.{2}",
                    this.m_Hit.dependencies[i].version.x,
                    this.m_Hit.dependencies[i].version.y,
                    this.m_Hit.dependencies[i].version.z
                );
                
                VisualElement content = new VisualElement
                {
                    name = DocumentationBaseElement.NAME_DEPENDENCY
                };
                content.AddToClassList(i % 2 == 0 
                    ? DocumentationBaseElement.ROW_EVEN 
                    : DocumentationBaseElement.ROW_ODD);
                
                content.Add(new Label
                {
                    name = DocumentationBaseElement.NAME_DEPENDENCY_ID,
                    text = id
                });
                
                content.Add(new Label
                {
                    name = DocumentationBaseElement.NAME_DEPENDENCY_VERSION,
                    text = version
                });
                
                container.Add(content);
            }
        
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }

        private void IncludeKeywords(VisualElement parent, bool prefixSeparator)
        {
            if (this.m_Hit.keywords.Length == 0) return;

            VisualElement container = new VisualElement
            {
                name = DocumentationBaseElement.NAME_KEYWORDS
            };
            
            foreach (string keyword in this.m_Hit.keywords)
            {
                container.Add(new Label
                {
                    name = DocumentationBaseElement.NAME_KEYWORD,
                    text = keyword
                });
            }
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }

        private void IncludeRenderPipelines(VisualElement parent, bool prefixSeparator)
        {
            RenderPipelines renderPipelines = this.m_Hit.renderPipelines;
            if (renderPipelines == null) return;
            if (!renderPipelines.builtin && !renderPipelines.urp && !renderPipelines.hdrp) return;

            VisualElement container = new VisualElement
            {
                name = DocumentationBaseElement.NAME_RENDERING_PIPELINES
            };

            container.Add(new Label
            {
                name = renderPipelines.builtin
                    ? DocumentationBaseElement.NAME_RENDERING_PIPELINE_ON
                    : DocumentationBaseElement.NAME_RENDERING_PIPELINE_OFF,
                text = "Built-in RP",
                tooltip = "Built-in Rendering Pipeline"
            });

            container.Add(new Label
            {
                name = renderPipelines.urp
                    ? DocumentationBaseElement.NAME_RENDERING_PIPELINE_ON
                    : DocumentationBaseElement.NAME_RENDERING_PIPELINE_OFF,
                text = "URP",
                tooltip = "Universal Rendering Pipeline"
            });
            
            container.Add(new Label
            {
                name = renderPipelines.hdrp
                    ? DocumentationBaseElement.NAME_RENDERING_PIPELINE_ON
                    : DocumentationBaseElement.NAME_RENDERING_PIPELINE_OFF,
                text = "HDRP",
                tooltip = "High Definition Rendering Pipeline"
            });
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }
    }
}