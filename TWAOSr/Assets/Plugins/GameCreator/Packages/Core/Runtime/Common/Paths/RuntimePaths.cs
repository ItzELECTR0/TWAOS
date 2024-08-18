namespace GameCreator.Runtime.Common
{
    public static class RuntimePaths
    {
        public const string GAMECREATOR = "Assets/Plugins/GameCreator/";
        
        public const string PACKAGES = GAMECREATOR + "Packages/";
        public const string DATA = GAMECREATOR + "Data/";
        public const string DEPLOY_INSTALLS = GAMECREATOR + "Installs/";
        public const string DEPLOY_HUB = GAMECREATOR + "Hub/";

        public const string CORE = PACKAGES + "Core/";
        public const string RUNTIME = CORE + "Runtime/";
        public const string EDITOR = CORE + "Editor/";

        public const string COMMON = RUNTIME + "Common/";
        public const string CAMERAS = RUNTIME + "Cameras/";
        public const string CHARACTERS = RUNTIME + "Characters/";
        public const string VARIABLES = RUNTIME + "Variables/";
        public const string VISUAL_SCRIPTING = RUNTIME + "VisualScripting/";
        public const string HUB = RUNTIME + "Hub/";

        public const string GIZMOS = EDITOR + "Gizmos/";
    }
}