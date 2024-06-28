using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class InputDropdown : EditorWindow 
    {
        private const string USS_PATH = EditorPaths.COMMON + "UI/StyleSheets/InputDropdown";

        private const string NAME_MODAL = "GC-InputDropdown";
        private const string NAME_CONTENT = "GC-InputDropdown-Content";
        
        private static readonly Vector2 MIN_SIZE = new Vector2(100, 62);
        private static InputDropdown Window;

        // MEMBERS: -------------------------------------------------------------------------------

        private string m_LabelText;
        private Action<string> m_Callback;

        private VisualElement m_Root;
        private VisualElement m_Content;
        
        private TextField m_TextField;
        private Button m_Button;

        // WINDOW: --------------------------------------------------------------------------------

        public static void Open(string text, VisualElement parent, Action<string> callback)
        {
            if (Window != null)
            {
                Window.Close();
                return;
            }

            Window = CreateInstance<InputDropdown>();
            Window.minSize = MIN_SIZE;

            Window.m_LabelText = text;
            Window.m_Callback = callback;
            
            Rect rectActivator = new Rect(
                focusedWindow.position.x + parent.worldBound.x,
                focusedWindow.position.y + parent.worldBound.y,
                parent.worldBound.width,
                parent.worldBound.height
            );
            
            Vector2 windowSize = new Vector2(
                Math.Max(parent.resolvedStyle.width, MIN_SIZE.x),
                MIN_SIZE.y
            );
            
            Window.ShowAsDropDown(rectActivator, windowSize);
        }

        private void OnDisable()
        {
            this.m_TextField?.UnregisterValueChangedCallback(this.OnChangeTextField);
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

            this.m_TextField = new TextField { isDelayed = true };
            this.m_TextField.RegisterValueChangedCallback(this.OnChangeTextField);

            this.m_Button = new Button(this.Submit) { text = "Save" };

            this.m_Content.Add(this.m_TextField);
            this.m_Content.Add(this.m_Button);
            
            this.m_Root.Add(new LabelTitle(this.m_LabelText));
            this.m_Root.Add(this.m_Content);
            
            this.m_TextField.Focus();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnChangeTextField(ChangeEvent<string> changeEvent)
        {
            this.Submit();
        }

        private void Submit()
        {
            this.m_Callback?.Invoke(this.m_TextField.value);
            this.Close();
        }
    }
}