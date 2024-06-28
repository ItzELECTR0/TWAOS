using System;
using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Core.Runtime.Common;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class NonSerializedListInspector : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|

        private const string USS_PATH = DaimahouPaths.COMMON + "StyleSheets/";
        private const string USS_HEAD_PATH = USS_PATH + "List-Head";
        private const string USS_BODY_PATH = USS_PATH + "List-Body";
        
        private const string CLASS_HEAD = "list-item-head";
        private const string CLASS_BODY = "list-body";
        
        private const string ELEMENT_NAME_HEAD = "List-Head";
        private const string ELEMENT_NAME_BODY = "List-Body";
        
        public const string CLASS_HEAD_INFO = "list-item-head-info";
        
        private static readonly IIcon ICON_ARR_D = new IconArrowDropDown(ColorTheme.Type.TextLight);
        private static readonly IIcon ICON_ARR_R = new IconArrowDropRight(ColorTheme.Type.TextLight);
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|

        private readonly Func<IEnumerable<IGenericItem>> m_FindItems;
        private readonly List<NonSerializedGenericInspector> m_Inspectors = new();
        
        protected readonly VisualElement m_Head;
        protected readonly VisualElement m_Body;

        protected Image m_HeadImage;
        protected Label m_HeadLabel;
        protected Button m_HeadButton;
        
        private bool m_Expanded = true;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|

        public NonSerializedListInspector(Func<IEnumerable<IGenericItem>> findItems)
        {
            m_FindItems = findItems;
            
            m_Head = new VisualElement { name = ELEMENT_NAME_HEAD };
            m_Body = new VisualElement { name = ELEMENT_NAME_BODY };
            
            m_Head.AddToClassList(CLASS_HEAD);
            m_Body.AddToClassList(CLASS_BODY);
            
            SetupHead();
            SetupBody();

            SetStyles();
            Refresh();
        }
        
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|

        public void Update()
        {
            if (m_FindItems().Count() != m_Inspectors.Count)
            {
                Refresh();
                return;
            }
            
            foreach (var inspector in m_Inspectors)
            {
                inspector.Refresh();
            }
        }
        
        public void Refresh()
        {
            m_Body.Clear();
            m_Inspectors.Clear();

            foreach (var inspector in m_FindItems().Select(MakeItemInspector))
            {
                m_Inspectors.Add(inspector);
                m_Body.Add(inspector);
            }
            
            m_Body.style.display = m_Expanded
                ? DisplayStyle.Flex
                : DisplayStyle.None;
                
            if(m_HeadImage != null) m_HeadImage.image = m_Expanded
                ? ICON_ARR_D.Texture
                : ICON_ARR_R.Texture;
        }
       
        public void SetTitle(string title)
        {
            if (title == null) return;
            
            m_Head.style.display = DisplayStyle.Flex;
            m_HeadLabel.text = title;
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|

        protected virtual NonSerializedGenericInspector MakeItemInspector(IGenericItem item)
        {
            return new NonSerializedGenericInspector(item);
        }
        
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|

        private void SetupHead()
        {
            hierarchy.Add(m_Head);
            
            m_HeadImage = new Image
            {
                image = m_Expanded
                    ? ICON_ARR_D.Texture
                    : ICON_ARR_R.Texture
            };
            
            m_HeadLabel = new Label("Undefined");
            
            m_HeadButton = new Button(() =>
            {
                m_Expanded = !m_Expanded;
                
                Refresh();
            });
            
            m_HeadButton.AddToClassList(CLASS_HEAD_INFO);

            m_HeadButton.Add(m_HeadImage);
            m_HeadButton.Add(m_HeadLabel);
            
            m_Head.Add(m_HeadButton);

            m_Head.style.display = DisplayStyle.None;
        }

        private void SetupBody() 
        {
            hierarchy.Add(m_Body);
        }
        
        private void SetStyles()
        {
            var styleSheetsPaths = new List<string>
            {
                USS_HEAD_PATH,
                USS_BODY_PATH,
            };

            var sheets = StyleSheetUtils.Load(styleSheetsPaths.ToArray());
            foreach (var sheet in sheets) styleSheets.Add(sheet);
        }
        
        //============================================================================================================||
        
    }
}