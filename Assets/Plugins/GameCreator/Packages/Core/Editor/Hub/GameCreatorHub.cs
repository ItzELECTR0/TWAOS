using System;
using System.Collections.Generic;

namespace GameCreator.Editor.Hub
{
    public static class GameCreatorHub
    {
        private const string URI_BASE = "https://gamecreator.io/";

        public const string URI_PACKAGE = URI_BASE + "hub/";
        public const string URI_CREATE_ACCOUNT = URI_BASE + "sign-in";
        public const string URI_ACCOUNT_SETTINGS = URI_BASE + "settings";
        public const string URI_ACCOUNT_HELP = URI_BASE + "learn/courses/how-to-upload-packages-to-the-hub";

        public const string URI_CF = "https://us-central1-catsoft-works-game-creator.cloudfunctions.net/";

        public const string DOWNLOAD_PROJECT_PATH = "Plugins/GameCreator/Hub";

        public static readonly string[] TYPES_SUPPORTED =
        {
            "Instruction",
            "Condition",
            "Event"
        };

        // PRIVATE STATIC MEMBERS: ----------------------------------------------------------------

        private static HitData[] DATA = Array.Empty<HitData>();

        // EVENTS: --------------------------------------------------------------------------------

        /// <summary>
        /// Fired every time the Data property changes its value
        /// </summary>
        public static event Action EventChangeData;

        // PROPERTIES: ----------------------------------------------------------------------------

        /// <summary>
        /// Returns whether there is a username registered
        /// </summary>
        public static bool HasUsername => Auth.HasUsername;

        /// <summary>
        /// Returns whether there is a passcode registered
        /// </summary>
        public static bool HasPasscode => Auth.HasPasscode;

        /// <summary>
        /// Returns the registered username. An empty string if no user is registered
        /// </summary>
        public static string Username => Auth.Username;

        /// <summary>
        /// Returns the registered passcode. An empty string if no pass is registered
        /// </summary>
        public static string Passcode => Auth.Passcode;

        /// <summary>
        /// Returns true while the system is authenticating with the Game Creator backend
        /// </summary>
        public static bool IsAuthenticating => Auth.IsAuthenticating;

        /// <summary>
        /// Returns true if the user is correctly authenticated with the Game Creator backend
        /// </summary>
        public static bool IsAuthenticated => Auth.IsAuthenticated;

        /// <summary>
        /// Returns the latest downloaded data from the Game Creator Hub server
        /// </summary>
        public static HitData[] Data
        {
            get => DATA;
            set
            {
                DATA = value;
                EventChangeData?.Invoke();
            }
        }
    }
}
