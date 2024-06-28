using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaimahouGames.Core.Runtime.Common;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class NonSerializedGenericInspector : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string USS_PATH = DaimahouPaths.COMMON + "StyleSheets/";
        private const string USS_HEAD_PATH = USS_PATH + "List-Head";
        private const string USS_BODY_PATH = USS_PATH + "List-Body";
        private const string USS_FOOT_PATH = USS_PATH + "List-Foot";
        
        public const string CLASS_HEAD = "list-item-head";
        public const string CLASS_BODY = "list-item-body";
        
        public const string CLASS_HEAD_INFO = "list-item-head-info";
        public const string CLASS_HEAD_INFO_EXPANDED = "list-item-head-info--expanded"; 
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|

        private readonly IGenericItem m_Item;

        protected readonly VisualElement m_Head = new();
        protected readonly VisualElement m_Body = new();
        
        protected Image m_HeadImage;
        protected Label m_HeadLabel;
        protected Button m_HeadButton;

        private InfoMessage m_InfoLabel;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        private bool IsExpanded { get; set; }
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public NonSerializedGenericInspector(IGenericItem item)
        {
            m_Item = item;
            IsExpanded = m_Item.IsExpanded;
            
            m_Head.AddToClassList(CLASS_HEAD);
            m_Body.AddToClassList(CLASS_BODY);
            
            SetStyles();
            
            Add(m_Head);
            Add(m_Body);
            
            SetupHead();
            SetupBody();
        }
        
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public void Refresh()
        {
            UpdateHead();
            UpdateBody();
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        
        private void SetupHead()
        {
            m_HeadImage = new Image();
            m_HeadLabel = new Label("Undefined");

            m_HeadButton = new Button(() =>
            {
                IsExpanded = !IsExpanded;
                
                UpdateHead();
                UpdateBody();
            });

            m_HeadButton.AddToClassList(CLASS_HEAD_INFO);

            m_HeadButton.Add(m_HeadImage);
            m_HeadButton.Add(m_HeadLabel);
            m_Head.Add(m_HeadButton);

            UpdateHead();
        }
        
        private void SetupBody()
        {
            m_InfoLabel = new InfoMessage("");
            m_Body.Add(m_InfoLabel);

            UpdateBody();
        }
        
        private void UpdateHead()
        {
            if (IsExpanded || m_Item.IsExpanded)
            {
                if (!m_HeadButton.ClassListContains(CLASS_HEAD_INFO_EXPANDED))
                {
                    m_HeadButton.AddToClassList(CLASS_HEAD_INFO_EXPANDED);
                }
            }
            else
            {
                m_HeadButton.RemoveFromClassList(CLASS_HEAD_INFO_EXPANDED);
            }

            m_HeadLabel.text = m_Item.Title;
            m_HeadLabel.style.color = m_Item.Color;
            
            SetIcon();
        }

        protected virtual void UpdateBody()
        {
            m_InfoLabel.Text = string.Join("\n", m_Item.Info);
            m_Body.style.display = IsExpanded || m_Item.IsExpanded
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
        
        private void SetIcon()
        {
            var iconAttrs = m_Item.GetType().GetCustomAttributes<ImageAttribute>();
            var icon = iconAttrs.FirstOrDefault()?.Image;

            m_HeadImage.image = icon != null ? icon : Texture2D.whiteTexture;
            m_HeadImage.style.display = icon != null ? DisplayStyle.Flex : DisplayStyle.None;

            if(icon == null) m_HeadLabel.style.marginLeft = 8;
        }
        
        private void SetStyles()
        {
            var styleSheetsPaths = new List<string>
            {
                USS_HEAD_PATH,
                USS_BODY_PATH,
                USS_FOOT_PATH
            };

            var sheets = StyleSheetUtils.Load(styleSheetsPaths.ToArray());
            foreach (var sheet in sheets) styleSheets.Add(sheet);
        }
        
        //============================================================================================================||
    }
}