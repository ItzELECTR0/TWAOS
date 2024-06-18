using System;
using GameCreator.Runtime.Common;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public sealed class ContentBox : VisualElement
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/ContentBox";
        
        private const string CLASS_BODY_ACTIVE = "gc-content-box-active";
        
        private static readonly IIcon ICON_ARR_D = new IconArrowDropDown(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_ARR_R = new IconArrowDropRight(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly VisualElement m_Head;
        [NonSerialized] private readonly VisualElement m_Body;

        [NonSerialized] private readonly Image m_HeadIcon;
        [NonSerialized] private readonly Label m_HeadLabel;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public string Title
        {
            get => this.m_HeadLabel.text;
            set => this.m_HeadLabel.text = value;
        }
        
        public bool IsExpanded
        {
            get => this.m_Body.style.display == DisplayStyle.Flex;
            set
            {
                this.m_Body.style.display = value
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
                
                this.Refresh();
            }
        }

        public VisualElement Content => this.m_Body;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ContentBox(string title, bool isExpanded)
        {
            this.m_Head = new VisualElement { name = "GC-ContentBox-Head" };
            this.m_Body = new VisualElement { name = "GC-ContentBox-Body" };
            
            this.m_Body.AddToClassList(AlignLabel.CLASS_UNITY_MAIN_CONTAINER);
            this.m_Body.AddToClassList(AlignLabel.CLASS_UNITY_INSPECTOR_ELEMENT);
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.styleSheets.Add(sheet);
            
            this.Add(this.m_Head);
            this.Add(this.m_Body);

            this.m_HeadIcon = new Image();
            this.m_HeadLabel = new Label();

            this.m_Head.Add(this.m_HeadIcon);
            this.m_Head.Add(this.m_HeadLabel);
            
            this.m_Head.RegisterCallback<ClickEvent>(_ => this.IsExpanded = !this.IsExpanded);

            this.Title = title;
            this.IsExpanded = isExpanded;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Refresh()
        {
            if (this.IsExpanded)
            {
                this.m_HeadIcon.image = ICON_ARR_D.Texture;
                this.m_Body.style.display = DisplayStyle.Flex;
                this.m_Body.AddToClassList(CLASS_BODY_ACTIVE);
            }
            else
            {
                this.m_HeadIcon.image = ICON_ARR_R.Texture;
                this.m_Body.style.display = DisplayStyle.None;
                this.m_Body.RemoveFromClassList(CLASS_BODY_ACTIVE);
            }
        }
    }
}
