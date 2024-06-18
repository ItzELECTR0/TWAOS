using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public abstract class TInputDropdown<T> : EditorWindow 
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/InputDropdown";

        private const string NAME_MODAL = "GC-InputDropdown";
        private const string NAME_CONTENT = "GC-InputDropdown-Content";
        private const string NAME_FIELD = "GC-InputDropdown-Field";
        
        protected static TInputDropdown<T> Window;

        // MEMBERS: -------------------------------------------------------------------------------

        private string m_LabelText;
        private Action<T> m_Callback;

        private VisualElement m_Root;
        protected VisualElement m_Content;
        
        private Button m_Button;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected abstract T Value { get; }
        protected abstract VisualElement Field { get; }

        // WINDOW: --------------------------------------------------------------------------------

        private void OnDisable()
        {
            this.UnregisterFieldChange();
        }

        private void CreateGUI()
        {
            this.m_Root = new VisualElement { name = NAME_MODAL };
            this.rootVisualElement.Add(this.m_Root);
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheets)
            {
                this.m_Root.styleSheets.Add(styleSheet);
            }

            this.m_Content = new VisualElement { name = NAME_CONTENT };

            this.CreateField();
            this.RegisterFieldChange();

            this.Field.name = NAME_FIELD;
            this.Field.Focus();
            this.m_Button = new Button(this.Submit) { text = "Save" };
            
            this.m_Content.Add(this.m_Button);
            
            this.m_Root.Add(new LabelTitle(this.m_LabelText));
            this.m_Root.Add(this.m_Content);
            
            this.rootVisualElement.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                this.Field.Focus();
            });
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        protected abstract void CreateField();

        protected abstract void RegisterFieldChange();
        protected abstract void UnregisterFieldChange();

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected void OnChangeInputField()
        {
            this.Submit();
        }

        private void Submit()
        {
            this.m_Callback?.Invoke(this.Value);
            this.Close();
        }
        
        // PROTECTED STATIC METHODS: --------------------------------------------------------------

        protected static void SetupWindow(string text, VisualElement parent, Action<T> callback)
        {
            Window.minSize = new Vector2(200, 62);

            Window.m_LabelText = text;
            Window.m_Callback = callback;
            
            Rect rectActivator = new Rect(
                focusedWindow.position.x + parent.worldBound.x,
                focusedWindow.position.y + parent.worldBound.y,
                parent.worldBound.width,
                parent.worldBound.height
            );
            
            Vector2 windowSize = new Vector2(
                Math.Max(parent.resolvedStyle.width, Window.minSize.x),
                Window.minSize.y
            );
            
            Window.ShowAsDropDown(rectActivator, windowSize);
        }
    }
}