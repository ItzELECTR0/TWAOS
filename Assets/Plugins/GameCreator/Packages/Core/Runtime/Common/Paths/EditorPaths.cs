namespace GameCreator.Runtime.Common
{
    public static class EditorPaths
    {
        public const string GAMECREATOR = "Assets/Plugins/GameCreator/";
        
        public const string PACKAGES = GAMECREATOR + "Packages/";
        public const string DATA = GAMECREATOR + "Data/";
        public const string DEPLOY_INSTALLS = GAMECREATOR + "Installs/";
        public const string DEPLOY_HUB = GAMECREATOR + "Hub/";

        public const string CORE = PACKAGES + "Core/";
        public const string EDITOR = CORE + "Editor/";

        public const string COMMON = EDITOR + "Common/";
        public const string CAMERAS = EDITOR + "Cameras/";
        public const string CHARACTERS = EDITOR + "Characters/";
        public const string VARIABLES = EDITOR + "Variables/";
        public const string VISUAL_SCRIPTING = EDITOR + "VisualScripting/";
        public const string INSTALLS = EDITOR + "Installs/";
        public const string HUB = EDITOR + "Hub/";
        public const string TOOLBAR = EDITOR + "Toolbar/";
    }
}