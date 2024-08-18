using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Hub
{
    public class HubSettingsWindow : EditorWindow
    {
        private const string MENU_TITLE = "Game Creator Hub - Settings";

        private const int MIN_WIDTH = 450;
        private const int MIN_HEIGHT = 200;

        private const string LABEL_INPUT_EMAIL = "Email:";
        private const string LABEL_INPUT_TOKEN = "Secret Token:";

        private const string USS_PATH = EditorPaths.HUB + "Windows/Stylesheets/HubSettingsWindow";

        private const string NAME_HEAD = "GC-Hub-Settings-Head";
        private const string NAME_BODY = "GC-Hub-Settings-Body";
        private const string NAME_FOOT = "GC-Hub-Settings-Foot";

        private const string NAME_FOOT_HELP = "GC-Hub-Settings-Foot-Button-Help";

        private static HubSettingsWindow WINDOW;

        // MEMBERS: -------------------------------------------------------------------------------

        private TextField m_InputEmail;
        private TextField m_InputToken;

        private VisualElement m_Head;
        private VisualElement m_Body;
        private VisualElement m_Foot;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        public static void OpenWindow()
        {
            if (WINDOW != null) WINDOW.Close();
            
            WINDOW = GetWindow<HubSettingsWindow>(true, MENU_TITLE, true);
            WINDOW.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
        }

        private void OnEnable()
        {
            StyleSheet[] styleSheetsCollection = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet styleSheet in styleSheetsCollection)
            {
                this.rootVisualElement.styleSheets.Add(styleSheet);
            }

            this.m_Head = new VisualElement { name = NAME_HEAD };
            this.m_Body = new VisualElement { name = NAME_BODY };
            this.m_Foot = new VisualElement { name = NAME_FOOT };

            Label labelTitle = new Label("Link your Game Creator Hub account");
            this.m_Head.Add(labelTitle);
            
            this.m_InputEmail = new TextField(LABEL_INPUT_EMAIL);
            this.m_InputToken = new TextField(LABEL_INPUT_TOKEN, 20, false, true, '*');

            this.m_Body.Add(this.m_InputEmail);
            this.m_Body.Add(this.m_InputToken);

            this.rootVisualElement.Add(this.m_Head);
            this.rootVisualElement.Add(this.m_Body);
            this.rootVisualElement.Add(this.m_Foot);

            this.OnAuthenticate(Auth.IsAuthenticated);
            Auth.EventAuthenticated += this.OnAuthenticate;
            Auth.EventAuthenticating += this.OnAuthenticate;
        }

        private void OnDisable()
        {
            Auth.EventAuthenticated -= this.OnAuthenticate;
            Auth.EventAuthenticating += this.OnAuthenticate;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnAuthenticate(bool authState)
        {
            this.m_Foot.Clear();
            
            if (Auth.IsAuthenticating)
            {
                Button buttonLoading = new Button { text = "Authenticating..." };
                buttonLoading.SetEnabled(false);
                this.m_Foot.Add(buttonLoading);
            }
            else
            {
                Action buttonAction = Auth.IsAuthenticated ? (Action) Auth.Logout : this.Login;
                Button buttonAuthenticate = new Button(buttonAction)
                {
                    text = Auth.IsAuthenticated ? "Logout" : "Link account"
                };

                this.m_Foot.Add(buttonAuthenticate);
            }

            VisualElement helpContainer = new VisualElement
            {
                name = NAME_FOOT_HELP
            };

            VisualElement helpSpacerL = new VisualElement();
            helpSpacerL.AddToClassList("gc-spacer");
            
            
            VisualElement helpSpacerR = new VisualElement();
            helpSpacerR.AddToClassList("gc-spacer");
            
            Button buttonHelp = new Button(Auth.GoToAccountHelp)
            {
                name = NAME_FOOT_HELP,
                text = "Learn why and how to connect your account"
            };
            
            helpContainer.Add(helpSpacerL);
            helpContainer.Add(buttonHelp);
            helpContainer.Add(helpSpacerR);
            
            this.m_Foot.Add(helpContainer);

            this.m_InputEmail.value = Auth.Username;
            this.m_InputToken.value = Auth.Passcode;
            
            this.m_InputEmail.SetEnabled(!Auth.IsAuthenticated);
            this.m_InputToken.SetEnabled(!Auth.IsAuthenticated);
        }
        
        private async void Login()
        {
            bool success = await Auth.Login(this.m_InputEmail.value, this.m_InputToken.value);
            if (!success) Auth.Logout();
        }
    }
}
