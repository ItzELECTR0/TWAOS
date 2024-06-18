using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class LabelProgress : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/LabelProgress";

        private const string NAME_BAR = "GC-LabelProgress-Bar";
        private const string NAME_PROGRESS = "GC-LabelProgress-Progress";
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private readonly Label m_Label;
        private float m_Value;
        
        private readonly VisualElement m_Bar;
        private readonly VisualElement m_Progress;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Label
        {
            get => this.m_Label.text;
            set => this.m_Label.text = value;
        }
        
        public float Value
        {
            get => this.m_Value;
            set
            {
                this.m_Value = Mathf.Clamp01(value);
                this.m_Progress.style.width = new Length(this.m_Value * 100f, LengthUnit.Percent);
            }
        }

        public Color Color
        {
            set => this.m_Progress.style.backgroundColor = value;
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public LabelProgress(string label, float value, Color color) : this(label, value, color, true)
        { }
        
        public LabelProgress(float value, Color color, string label) : this(label, value, color, false)
        { }

        private LabelProgress(string label, float value, Color color, bool labelFirst)
        {
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);

            this.m_Label = new Label { text = label };
            
            this.m_Bar = new VisualElement { name = NAME_BAR };
            this.m_Progress = new VisualElement { name = NAME_PROGRESS };
            
            this.m_Bar.Add(this.m_Progress);

            switch (labelFirst)
            {
                case true:
                    this.Add(this.m_Label);
                    this.Add(this.m_Bar);
                    break;
                
                case false: 
                    this.Add(this.m_Bar);
                    this.Add(this.m_Label);
                    break;
            }

            this.Value = value;
            this.Color = color;
        }
    }
}
