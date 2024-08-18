using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Hub
{
    internal static class Auth
    {
        [Serializable]
        private struct OutLogin
        {
            public string username;
            public string passcode;
        }

        // CONSTANTS: -----------------------------------------------------------------------------

        private const string KEY_USERNAME = "gchub:username";
        private const string KEY_PASSCODE = "gchub:passcode";

        private const string CF_LOGIN = "editorLogin";

        // PROPERTIES: ----------------------------------------------------------------------------

        public static bool IsAuthenticated => HasUsername && HasPasscode;
        public static bool IsAuthenticating { get; private set; }

        public static string Username
        {
            get => EditorPrefs.GetString(KEY_USERNAME);
            private set => EditorPrefs.SetString(KEY_USERNAME, value);
        }

        public static string Passcode
        {
            get => EditorPrefs.GetString(KEY_PASSCODE);
            private set => EditorPrefs.SetString(KEY_PASSCODE, value);
        }

        public static bool HasUsername => !string.IsNullOrEmpty(Username);
        public static bool HasPasscode => !string.IsNullOrEmpty(Passcode);

        // EVENTS: --------------------------------------------------------------------------------

        public static event Action<bool> EventAuthenticating;
        public static event Action<bool> EventAuthenticated;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static async Task<bool> Login(string username, string passcode)
        {
            if (IsAuthenticating) return false;

            IsAuthenticating = true;
            EventAuthenticating?.Invoke(true);

            OutLogin data = new OutLogin
            {
                username = username,
                passcode = passcode
            };
            
            Http.ReceiveData response = await Http.Send(CF_LOGIN, data);
            bool successful;
            string messageTitle;
            string messageContent;

            if (response.error)
            {
                successful = false;
                messageTitle = "Error while authenticating";
                messageContent = response.data;

                Username = string.Empty;
                Passcode = string.Empty;
            }
            else
            {
                successful = true;
                messageTitle = "Authentication successful";
                messageContent = $"User {username} has been successfully authenticated";

                Username = username;
                Passcode = passcode;
            }

            EditorUtility.DisplayDialog(
                messageTitle,
                messageContent,
                "Accept"
            );

            IsAuthenticating = false;
            EventAuthenticating?.Invoke(false);
            EventAuthenticated?.Invoke(successful);
            
            return successful;
        }

        public static void GoToLoginAccount()
        {
            Application.OpenURL(GameCreatorHub.URI_CREATE_ACCOUNT);
        }
        
        public static void GoToMyAccount()
        {
            Application.OpenURL(GameCreatorHub.URI_ACCOUNT_SETTINGS);
        }

        public static void GoToAccountHelp()
        {
            Application.OpenURL(GameCreatorHub.URI_ACCOUNT_HELP);
        }

        public static void Logout()
        {
            Username = string.Empty;
            Passcode = string.Empty;
            EventAuthenticated?.Invoke(false);
        }
    }
}