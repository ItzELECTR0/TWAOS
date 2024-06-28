using System.Collections.Generic;
using DaimahouGames.Core.Runtime.Common;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    public class FoldoutInspector : VisualElement
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string USS_PATH = DaimahouPaths.COMMON + "StyleSheets/";
        private const string USS_HEAD_PATH = USS_PATH + "List-Head";
        private const string USS_BODY_PATH = USS_PATH + "List-Body";
        private const string USS_FOOT_PATH = USS_PATH + "List-Foot";

        public const string CLASS_HEAD = "list-item-head";
        public const string CLASS_BODY = "list-item-body";
        
        public const string CLASS_DROP_ABOVE = "gc-list-item-drop-above";
        public const string CLASS_DROP_BELOW = "gc-list-item-drop-below";
        
        public const string CLASS_HEAD_INFO = "list-item-head-info";
        public const string CLASS_HEAD_INFO_EXPANDED = "list-item-head-info--expanded"; 
        
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|

        protected readonly VisualElement m_Head = new();
        protected readonly VisualElement m_Body = new();

        public readonly VisualElement m_DropAbove = new();
        public readonly VisualElement m_DropBelow = new();
        
        protected Image m_HeadImage;
        protected Label m_HeadLabel;
        protected Button m_HeadButton;

        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        
        public virtual bool IsExpanded { get; set; }
        protected virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
        
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※

        public FoldoutInspector() => BuildInspector();
        
        public FoldoutInspector(VisualElement e)
        {
            BuildInspector();
            AddBodyElement(e);
        }
        
        public FoldoutInspector(SerializedObject serializedObject, string[] elements)
        {
            BuildInspector();
            AddBodyElements(serializedObject, elements);
        }

        private void BuildInspector()
        {
            m_Head.AddToClassList(CLASS_HEAD);
            m_Body.AddToClassList(CLASS_BODY);
            
            m_DropAbove.AddToClassList(CLASS_DROP_ABOVE);
            m_DropBelow.AddToClassList(CLASS_DROP_BELOW);
            
            SetStyles();
            
            Add(m_Head);
            Add(m_Body);
            
            Initialize();
        }

        ~FoldoutInspector()
        {
            OnDisable();
        }
        
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※

        private void Initialize()
        {
            SetupHead();
            SetupBody();
            
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
            EditorApplication.playModeStateChanged += OnChangePlayMode;
            
            OnChangePlayMode(EditorApplication.isPlaying
                ? PlayModeStateChange.EnteredPlayMode
                : PlayModeStateChange.EnteredEditMode
            );
        }
        
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
        }
        
        // ※  Public Methods: --------------------------------------------------------------------------------------|※

        public void Refresh()
        {
            UpdateHead();
            UpdateBody();
        }
        
        public void SetIcon(IIcon icon)
        {
            m_HeadImage.image = icon != null ? icon.Texture : Texture2D.whiteTexture;
            m_HeadImage.style.display = icon != null ? DisplayStyle.Flex : DisplayStyle.None;

            if(icon == null) m_HeadLabel.style.marginLeft = 8;
        }

        public void SetTile(string title)
        {
            m_HeadLabel.text = title;
        }

        public void AddBodyElement(VisualElement e)
        {
            m_Body.Add(e);
        }
        
        public void AddBodyElements(SerializedObject serializedObject, params string[] elements)
        {
            foreach (var element in elements)
            {
                var prop = serializedObject.FindProperty(element);
                var field = new PropertyField(prop);

                AddBodyElement(field);
            }
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        
        protected virtual void OnChangePlayMode(PlayModeStateChange state) { }

        protected virtual void SetupHead()
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
        
        protected virtual void SetupBody()
        {
            UpdateBody();
        }

        protected virtual void UpdateHead()
        {
            if (IsExpanded)
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

            m_HeadLabel.style.color = Color;
        }

        protected virtual void UpdateBody()
        {
            m_Body.style.display = IsExpanded
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
        
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        
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