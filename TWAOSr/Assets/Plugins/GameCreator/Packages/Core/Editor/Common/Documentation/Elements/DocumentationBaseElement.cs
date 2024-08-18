using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Common
{
    public abstract class DocumentationBaseElement : VisualElement
    {
        public const string PATH_USS = EditorPaths.COMMON + "Documentation/Styles/Documentation";

        public const string NAME_SEPARATOR_SMALL = "GC-Documentation-Separator-Small";
        public const string NAME_SEPARATOR_LARGE = "GC-Documentation-Separator-Large";
        
        public const string NAME_HEADER = "GC-Documentation-Header";
        public const string NAME_PARAMETERS = "GC-Documentation-Parameters";
        public const string NAME_DEPENDENCIES = "GC-Documentation-Dependencies";
        public const string NAME_EXAMPLES = "GC-Documentation-Examples";
        public const string NAME_KEYWORDS = "GC-Documentation-Keywords";
        public const string NAME_RENDERING_PIPELINES = "GC-Documentation-RenderingPipelines";
        
        public const string NAME_ICON = "GC-Documentation-Icon";
        public const string NAME_TITLE = "GC-Documentation-Title";
        public const string NAME_DESCRIPTION = "GC-Documentation-Description";
        public const string NAME_CATEGORY = "GC-Documentation-Category";
        public const string NAME_VERSION = "GC-Documentation-Version";
        
        public const string NAME_PARAMETER = "GC-Documentation-Parameter";
        public const string NAME_PARAMETER_TITLE = "GC-Documentation-Parameter-Title";
        public const string NAME_PARAMETER_DESCR = "GC-Documentation-Parameter-Descr";
        
        public const string NAME_DEPENDENCY = "GC-Documentation-Dependency";
        public const string NAME_DEPENDENCY_ID = "GC-Documentation-Dependency-ID";
        public const string NAME_DEPENDENCY_VERSION = "GC-Documentation-Dependency-Version";
        
        public const string NAME_EXAMPLE_TITLE = "GC-Documentation-Example-Title";
        public const string NAME_EXAMPLE_CONTENT = "GC-Documentation-Example-Content";
        
        public const string NAME_KEYWORD = "GC-Documentation-Keyword";
        public const string NAME_RENDERING_PIPELINE_ON = "GC-Documentation-RenderingPipeline-On";
        public const string NAME_RENDERING_PIPELINE_OFF = "GC-Documentation-RenderingPipeline-Off";

        public const string ROW_EVEN = "gc-documentation-row-even";
        public const string ROW_ODD = "gc-documentation-row-odd";

        public const string CATEGORY_SEPARATOR = " › ";
        
        private static readonly TextInfo TEXT_INFO = new CultureInfo("en-US",false).TextInfo;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected Type Type { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected DocumentationBaseElement(Type type)
        {
            this.Type = type;

            StyleSheet[] styleSheets = StyleSheetUtils.Load(PATH_USS);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                this.styleSheets.Add(styleSheet);
            }
        }

        // BUILDER METHODS: -----------------------------------------------------------------------

        protected void IncludeSeparator(VisualElement parent)
        {
            VisualElement separator = new VisualElement { name = NAME_SEPARATOR_SMALL };
            parent.Add(separator);
        }
        
        protected virtual void IncludeHeader(VisualElement parent, bool prefixSeparator)
        {
            VisualElement header = new VisualElement { name = NAME_HEADER };
            
            this.IncludeIcon(header, false);
            this.IncludeTitle(header, false);
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(header);
        }
        
        protected virtual void IncludeIcon(VisualElement parent, bool prefixSeparator)
        {
            var attribute = this.Type.GetCustomAttributes<ImageAttribute>(true).FirstOrDefault();
            if (attribute == null) return;

            Image image = new Image
            {
                name = NAME_ICON,
                image = attribute.Image
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(image);
        }
        
        protected virtual void IncludeTitle(VisualElement parent, bool prefixSeparator)
        {
            Label label = new Label
            {
                name = NAME_TITLE,
                text = TypeUtils.GetTitleFromType(this.Type)
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        protected virtual void IncludeDescription(VisualElement parent, bool prefixSeparator)
        {
            var attribute = this.Type
                .GetCustomAttributes<DescriptionAttribute>(true)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(attribute?.Description)) return;
            
            Label label = new Label
            {
                name = NAME_DESCRIPTION,
                text = attribute.Description
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        protected virtual void IncludeCategory(VisualElement parent, bool prefixSeparator)
        {
            var attribute = this.Type
                .GetCustomAttributes<CategoryAttribute>(true)
                .FirstOrDefault();

            string category = attribute?.ToString(CATEGORY_SEPARATOR);
            if (string.IsNullOrEmpty(category)) return;
            
            Label label = new Label
            {
                name = NAME_CATEGORY,
                text = category
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        protected virtual void IncludeVersion(VisualElement parent, bool prefixSeparator)
        {
            var attribute = this.Type
                .GetCustomAttributes<VersionAttribute>(true)
                .FirstOrDefault();

            string version = attribute?.ToString();
            if (string.IsNullOrEmpty(version)) return;
            
            Label label = new Label
            {
                name = NAME_VERSION,
                text = version
            };
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(label);
        }

        protected virtual void IncludeParameters(VisualElement parent, bool prefixSeparator)
        {
            List<ParameterAttribute> parameters = new List<ParameterAttribute>(
                this.Type.GetCustomAttributes<ParameterAttribute>(true)
            );

            if (parameters.Count == 0) return;

            VisualElement container = new VisualElement { name = NAME_PARAMETERS };

            for (int i = 0; i < parameters.Count; ++i)
            {
                VisualElement content = new VisualElement { name = NAME_PARAMETER };
                content.AddToClassList(i % 2 == 0 ? ROW_EVEN : ROW_ODD);
                
                content.Add(new Label
                {
                    name = NAME_PARAMETER_TITLE,
                    text = parameters[i].Name 
                });
                
                content.Add(new Label
                {
                    name = NAME_PARAMETER_DESCR,
                    text = parameters[i].Description
                });
                
                container.Add(content);
            }

            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }

        protected virtual void IncludeExamples(VisualElement parent, bool prefixSeparator)
        {
            List<ExampleAttribute> examples = new List<ExampleAttribute>(
                this.Type.GetCustomAttributes<ExampleAttribute>(true)
            );

            if (examples.Count == 0) return;

            VisualElement container = new VisualElement { name = NAME_EXAMPLES };

            for (int i = 0; i < examples.Count; i++)
            {
                container.Add(new Label
                {
                    name = NAME_EXAMPLE_TITLE,
                    text = $"Example {i + 1}"
                });
                
                container.Add(new Label
                {
                    name = NAME_EXAMPLE_CONTENT,
                    text = examples[i].Content
                });
            }

            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }

        protected virtual void IncludeDependencies(VisualElement parent, bool prefixSeparator)
        {
            List<DependencyAttribute> dependencies = new List<DependencyAttribute>(
                this.Type.GetCustomAttributes<DependencyAttribute>(true)
            );

            if (dependencies.Count == 0) return;

            VisualElement container = new VisualElement { name = NAME_DEPENDENCIES };

            for (int i = 0; i < dependencies.Count; ++i)
            {
                string id = dependencies[i].ID;
                string version = string.Format(
                    "{0}.{1}.{2}",
                    dependencies[i].Version.Major,
                    dependencies[i].Version.Minor,
                    dependencies[i].Version.Build
                );
                
                VisualElement content = new VisualElement { name = NAME_DEPENDENCY };
                content.AddToClassList(i % 2 == 0 ? ROW_EVEN : ROW_ODD);
                
                content.Add(new Label
                {
                    name = NAME_DEPENDENCY_ID,
                    text = id
                });
                
                content.Add(new Label
                {
                    name = NAME_DEPENDENCY_VERSION,
                    text = version
                });
                
                container.Add(content);
            }

            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }

        protected virtual void IncludeKeywords(VisualElement parent, bool prefixSeparator)
        {
            List<KeywordsAttribute> keywords = new List<KeywordsAttribute>(
                this.Type.GetCustomAttributes<KeywordsAttribute>(true)
            );

            if (keywords.Count == 0) return;

            VisualElement container = new VisualElement { name = NAME_KEYWORDS };
            
            foreach (KeywordsAttribute keyword in keywords)
            {
                foreach (string word in keyword.Keywords)
                {
                    container.Add(new Label
                    {
                        name = NAME_KEYWORD,
                        text = word
                    });
                }
            }
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }
        
        protected virtual void IncludeRenderPipelines(VisualElement parent, bool prefixSeparator)
        {
            var attribute = this.Type
                .GetCustomAttributes<RenderPipelineAttribute>(true)
                .FirstOrDefault();
            
            if (attribute == null) return;
            VisualElement container = new VisualElement { name = NAME_RENDERING_PIPELINES };

            container.Add(new Label
            {
                name = attribute.Builtin
                    ? NAME_RENDERING_PIPELINE_ON
                    : NAME_RENDERING_PIPELINE_OFF,
                text = "Built-in RP",
                tooltip = "Built-in Rendering Pipeline"
            });

            container.Add(new Label
            {
                name = attribute.URP
                    ? NAME_RENDERING_PIPELINE_ON
                    : NAME_RENDERING_PIPELINE_OFF,
                text = "URP",
                tooltip = "Universal Rendering Pipeline"
            });
            
            container.Add(new Label
            {
                name = attribute.HDRP
                    ? NAME_RENDERING_PIPELINE_ON
                    : NAME_RENDERING_PIPELINE_OFF,
                text = "HDRP",
                tooltip = "High Definition Rendering Pipeline"
            });
            
            if (prefixSeparator) this.IncludeSeparator(parent);
            parent.Add(container);
        }
    }
}