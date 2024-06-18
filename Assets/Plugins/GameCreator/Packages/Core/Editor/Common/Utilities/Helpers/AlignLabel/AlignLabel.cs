using System;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class AlignLabel
    {
        // PUBLIC CONSTANTS: ----------------------------------------------------------------------
        
        public const string CLASS_UNITY_ALIGN_LABEL = "unity-base-field__aligned";
        
        public const string CLASS_UNITY_INSPECTOR_ELEMENT = "unity-inspector-element";
        public const string CLASS_UNITY_MAIN_CONTAINER = "unity-inspector-main-container";
        
        // PRIVATE CONSTANTS: ---------------------------------------------------------------------
        
        private const float EPSILON = 0.001f;
        
        private static readonly CustomStyleProperty<float> LabelWidthRatioProperty = new CustomStyleProperty<float>("--unity-property-field-label-width-ratio");
        private static readonly CustomStyleProperty<float> LabelExtraPaddingProperty = new CustomStyleProperty<float>("--unity-property-field-label-extra-padding");
        private static readonly CustomStyleProperty<float> LabelBaseMinWidthProperty = new CustomStyleProperty<float>("--unity-property-field-label-base-min-width");
        private static readonly CustomStyleProperty<float> LabelExtraContextWidthProperty = new CustomStyleProperty<float>("--unity-base-field-extra-context-width");

        private const float LABEL_WIDTH_RATION = 0.45f;
        private const float LABEL_EXTRA_PADDING = 37f;
        private const float LABEL_BASE_MIN_WIDTH = 123f;
        private const float LABEL_EXTRA_CONTEXT_WIDTH = 1f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly VisualElement m_Field;
        [NonSerialized] private readonly Label m_Label;
        
        [NonSerialized] private float m_LabelWidthRatio;
        [NonSerialized] private float m_LabelExtraPadding;
        [NonSerialized] private float m_LabelBaseMinWidth;
        [NonSerialized] private float m_LabelExtraContextWidth;

        [NonSerialized] private VisualElement m_CachedContextWidthElement;
        [NonSerialized] private VisualElement m_CachedInspectorElement;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private AlignLabel(VisualElement field)
        {
            this.m_Field = field;
            this.m_Label = field.Q<Label>();

            this.m_Field.RegisterCallback<AttachToPanelEvent>(this.OnAttachToPanel);
            this.m_Field.RegisterCallback<DetachFromPanelEvent>(this.OnDetachFromPanel);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static void On(VisualElement field)
        {
            _ = new AlignLabel(field);
        }

        // CALLBACKS: -----------------------------------------------------------------------------

        private void OnAttachToPanel(AttachToPanelEvent eventAttach)
        {
            if (eventAttach.destinationPanel == null) return;
            if (eventAttach.destinationPanel.contextType == ContextType.Player) return;

            VisualElement currentElement = this.m_Field.parent;
            while (currentElement != null)
            {
                if (currentElement.ClassListContains(CLASS_UNITY_INSPECTOR_ELEMENT))
                {
                    this.m_CachedInspectorElement = currentElement;
                }

                if (currentElement.ClassListContains(CLASS_UNITY_MAIN_CONTAINER))
                {
                    this.m_CachedContextWidthElement = currentElement;
                    break;
                }

                currentElement = currentElement.parent;
            }

            if (this.m_CachedInspectorElement == null)
            {
                return;
            }

            this.m_LabelWidthRatio = LABEL_WIDTH_RATION;
            this.m_LabelExtraPadding = LABEL_EXTRA_PADDING;
            this.m_LabelBaseMinWidth = LABEL_BASE_MIN_WIDTH;
            this.m_LabelExtraContextWidth = LABEL_EXTRA_CONTEXT_WIDTH;

            this.m_Field.RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            this.m_Field.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent eventDetach)
        {
            this.m_Field.UnregisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            this.m_Field.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        private void OnCustomStyleResolved(CustomStyleResolvedEvent eventResolution)
        {
            if (eventResolution.customStyle.TryGetValue(LabelWidthRatioProperty, out float labelWidthRatio))
            {
                this.m_LabelWidthRatio = labelWidthRatio;
            }

            if (eventResolution.customStyle.TryGetValue(LabelExtraPaddingProperty, out float labelExtraPadding))
            {
                this.m_LabelExtraPadding = labelExtraPadding;
            }

            if (eventResolution.customStyle.TryGetValue(LabelBaseMinWidthProperty, out float labelBaseMinWidth))
            {
                this.m_LabelBaseMinWidth = labelBaseMinWidth;
            }

            if (eventResolution.customStyle.TryGetValue(LabelExtraContextWidthProperty, out float labelExtraContextWidth))
            {
                this.m_LabelExtraContextWidth = labelExtraContextWidth;
            }

            this.ChangeWidth();   
        }

        private void OnGeometryChanged(GeometryChangedEvent eventGeometry)
        {
            this.ChangeWidth();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void ChangeWidth()
        {
            float totalPadding = this.m_LabelExtraPadding;
            float spacing = this.m_Field.worldBound.x - this.m_CachedInspectorElement.worldBound.x - this.m_CachedInspectorElement.resolvedStyle.paddingLeft;

            totalPadding += spacing;
            totalPadding += this.m_Field.resolvedStyle.paddingLeft;

            float minWidth = this.m_LabelBaseMinWidth - spacing - this.m_Field.resolvedStyle.paddingLeft;
            VisualElement contextWidthElement = this.m_CachedContextWidthElement ?? this.m_CachedInspectorElement;

            this.m_Label.style.minWidth = Math.Max(minWidth, 0);
            
            float newWidth = (contextWidthElement.resolvedStyle.width + this.m_LabelExtraContextWidth) * this.m_LabelWidthRatio - totalPadding;
            if (Math.Abs(this.m_Label.resolvedStyle.width - newWidth) > EPSILON)
            {
                this.m_Label.style.width = Math.Max(0f, newWidth);
            }
        }
    }
}