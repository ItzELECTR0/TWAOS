using System;
using System.IO;
using System.Text.RegularExpressions;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Installs
{
    public static class InstallManager
    {
        public const string NAME = "Install";
        
        private const string RX_INSTALL_PATH = @"^{0}@(\d+.\d+.\d+)";
        private const string PACKAGE_FILE = "Package.unitypackage";
        
        // EVENTS: --------------------------------------------------------------------------------

        public static event Action<string> EventChange; 

        // GETTER PUBLIC METHODS: -----------------------------------------------------------------

        /// <summary>
        /// Returns true if any version of the install asset ID is currently installed
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public static bool IsInstalled(string assetID)
        {
            string deployInstalls = PathUtils.PathForOS(EditorPaths.DEPLOY_INSTALLS);
            if (!Directory.Exists(deployInstalls)) return false;
            
            string[] installedPaths = Directory.GetDirectories(deployInstalls);
            string regex = string.Format(RX_INSTALL_PATH, GetInstallPath(assetID));
            
            Regex match = new Regex(regex);

            foreach (string installedPath in installedPaths)
            {
                Match result = match.Match(PathUtils.PathToUnix(installedPath));
                if (result.Success) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the installed version of the install asset ID. If none is installed it returns
        /// the default version number 0.0.0
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public static Version GetInstalledVersion(string assetID)
        {
            if (!IsInstalled(assetID)) return Version.Zero;

            string deployInstalls = PathUtils.PathForOS(EditorPaths.DEPLOY_INSTALLS);
            if (!Directory.Exists(deployInstalls)) return Version.Zero;
            
            string targetPath = GetInstallPath(assetID);
            string[] installedPaths = Directory.GetDirectories(deployInstalls);
            
            string regexPath = string.Format(RX_INSTALL_PATH, targetPath);
            Regex match = new Regex(regexPath);

            foreach (string installedPath in installedPaths)
            {
                Match result = match.Match(PathUtils.PathToUnix(installedPath)); 
                if (result.Success && result.Groups.Count == 2)
                {
                    return new Version(result.Groups[1].ToString());
                }
            }

            return Version.Zero;
        }
        
        /// <summary>
        /// Returns the installation path of a package without the version extension
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public static string GetInstallPath(string assetID)
        {
            return PathUtils.Combine(EditorPaths.DEPLOY_INSTALLS, assetID);
        }
        
        /// <summary>
        /// Returns the installation path of the package with the indicated version 
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string GetInstallPath(string assetID, Version version)
        {
            string path = GetInstallPath(assetID);
            return $"{path}@{version.ToString()}";
        }
        
        /// <summary>
        /// Returns the Install Asset object with the provided ID. Returns null if it cannot
        /// be found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Installer GetInstallAsset(string id)
        {
            string[] installAssetsGUIDs = AssetDatabase.FindAssets($"t:{nameof(Installer)}");
            foreach (string installAssetGUID in installAssetsGUIDs)
            {
                string installAssetPath = AssetDatabase.GUIDToAssetPath(installAssetGUID);
                Installer asset = AssetDatabase.LoadAssetAtPath<Installer>(installAssetPath);
                
                if (asset != null && asset.Data.ID == id) return asset;
            }

            return null;
        }
        
        // LOGIC PUBLIC METHODS: ------------------------------------------------------------------ 
        
        /// <summary>
        /// Installs an Install Asset along with its dependencies. Returns true if the processes
        /// is successful. False otherwise
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool Install(Installer asset)
        {
            if (asset == null) return false;
            if (IsInstalled(asset.Data.ID))
            {
                Version installedVersion = GetInstalledVersion(asset.Data.ID);
                
                if (installedVersion.IsEqual(asset.Data.Version))
                {
                    EditorUtility.DisplayDialog(
                        $"Version {asset.Data.Version} of '{asset.Data.ID}' is already installed", 
                        "If you want to reinstall this asset, remove it first and install it again", 
                        "Ok"
                    );
                    
                    return false;
                }
                
                if (installedVersion.IsHigher(asset.Data.Version))
                {
                    EditorUtility.DisplayDialog(
                        $"Trying to install {asset.Data.Version} of '{asset.Data.ID}' but a newer one is present", 
                        $"To downgrade from {installedVersion} to {asset.Data.Version}, you need to manually remove it first", 
                        "Ok"
                    );
                    
                    return false;
                }
            }

            if (InstallAsset(asset))
            {
                EventChange?.Invoke(asset.Data.ID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes an installed install asset. Note that this will not delete its dependencies
        /// </summary>
        /// <param name="assetID"></param>
        public static void Delete(string assetID)
        {
            Version version = GetInstalledVersion(assetID);
            string installedPath = GetInstallPath(assetID, version);
            
            AssetDatabase.DeleteAsset(installedPath);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
            
            EventChange?.Invoke(assetID);
        }

        /// <summary>
        /// Creates an install folder at the installation location to start building from
        /// </summary>
        /// <param name="asset"></param>
        public static void Create(Installer asset)
        {
            if (asset == null) return;
            if (IsInstalled(asset.Data.ID)) return;

            string path = GetInstallPath(asset.Data.ID, asset.Data.Version);
            
            DirectoryUtils.RequirePath(path);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
            
            EventChange?.Invoke(asset.Data.ID);
        }

        /// <summary>
        /// Changes the version of the installed package to the one indicated by the install asset
        /// and creates a .unitypackage file next to the asset based on the data of the installed
        /// version
        /// </summary>
        /// <param name="asset"></param>
        public static void Build(Installer asset)
        {
            if (!IsInstalled(asset.Data.ID))
            {
                EditorUtility.DisplayDialog(
                    $"Install '{asset.Data.ID}' is not installed",
                    "It is not possible to build from an install that does not exist",
                    "Ok"
                );
                
                return;
            }

            string sourcePath = GetInstallPath(asset.Data.ID, GetInstalledVersion(asset.Data.ID));
            string targetPath = GetInstallPath(asset.Data.ID, asset.Data.Version);

            if (!Directory.Exists(PathUtils.PathForOS(sourcePath)))
            {
                EditorUtility.DisplayDialog(
                    $"Directory at '{sourcePath}' does not exist",
                    "This Install cannot be built from an empty directory",
                    "Ok"
                );
                
                return;
            }

            if (sourcePath != targetPath)
            {
                AssetDatabase.MoveAsset(sourcePath, targetPath);
                AssetDatabase.DeleteAsset(sourcePath);
                AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);   
            }

            string packagePath = GetPackagePath(asset);

            AssetDatabase.ExportPackage(targetPath, packagePath, ExportPackageOptions.Recurse);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
            
            EventChange?.Invoke(asset.Data.ID);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static bool InstallAsset(Installer asset)
        {
            if (asset == null) return false;
            if (!InstallDependencies(asset)) return false;

            if (IsInstalled(asset.Data.ID))
            {
                Version installedVersion = GetInstalledVersion(asset.Data.ID);
                if (installedVersion.IsHigherOrEqual(asset.Data.Version)) return true;
                Delete(asset.Data.ID);
            }

            string packagePath = GetPackagePath(asset);
            if (File.Exists(PathUtils.PathForOS(packagePath)))
            {
                AssetDatabase.ImportPackage(packagePath, false);
                AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
                return true;
            }

            return false;
        }
        
        private static bool InstallDependencies(Installer asset)
        {
            Dependency[] dependencies = asset.Data.Dependencies;
            foreach (Dependency dependency in dependencies)
            {
                if (IsInstalled(dependency.ID))
                {
                    Version installedVersion = GetInstalledVersion(dependency.ID);
                    if (dependency.MinVersion.IsHigherOrEqual(installedVersion)) continue;
                }

                Installer dependencyAsset = GetInstallAsset(dependency.ID);
                if (dependencyAsset == null)
                {
                    EditorUtility.DisplayDialog(
                        $"Unable to find dependency '{dependency.ID}' package",
                        "Make sure the install you are trying to install is present",
                        "Ok"
                    );
                    
                    return false;
                }

                if (dependencyAsset.Data.Version.IsLower(dependency.MinVersion))
                {
                    EditorUtility.DisplayDialog(
                        $"Cannot install '{dependency.ID}' with version {dependency.MinVersion}",
                        $"The highest version present is {dependencyAsset.Data.Version}",
                        "Ok"
                    );

                    return false;
                }

                if (!InstallAsset(dependencyAsset)) return false;
            }

            return true;
        }

        private static string GetPackagePath(Installer asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string path = Path.GetDirectoryName(assetPath);
            
            return PathUtils.Combine(path ?? string.Empty, PACKAGE_FILE);
        }
    }
}
