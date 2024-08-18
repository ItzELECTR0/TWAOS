using System;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace GameCreator.Editor.Common.Versions
{
    [Serializable]
    internal class AssetVersion
    {
        public static readonly AssetVersion None = new AssetVersion(); 
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private int major;
        [SerializeField] private int minor;
        [SerializeField] private int patch;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Major => this.major;
        public int Minor => this.minor;
        public int Patch => this.patch;
        
        public bool Empty => this.major == default &&
                             this.minor == default && 
                             this.patch == default;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public AssetVersion()
        { }

        public AssetVersion(string version) : this()
        {
            string[] versions = version.Split('.');
            if (versions.Length != 3) return;

            bool majorSuccess = int.TryParse(versions[0], out int majorValue);
            bool minorSuccess = int.TryParse(versions[1], out int minorValue);
            bool patchSuccess = int.TryParse(versions[2], out int patchValue);

            if (!majorSuccess || !minorSuccess || !patchSuccess) return;
            
            this.major = majorValue;
            this.minor = minorValue;
            this.patch = patchValue;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsOlderThan(AssetVersion otherVersion)
        {
            if (this.Major > otherVersion.Major) return false;
            if (this.Major != otherVersion.Major) return true;
            
            if (this.Minor > otherVersion.Minor) return false;
            if (this.Minor != otherVersion.Minor) return true;
            
            return this.Patch < otherVersion.Patch;
        }
        
        public bool IsNewerThan(AssetVersion otherVersion)
        {
            if (this.Major > otherVersion.Major) return true;
            if (this.Major < otherVersion.Major) return false;
            
            if (this.Minor > otherVersion.Minor) return true;
            if (this.Minor < otherVersion.Minor) return false;
            
            return this.Patch > otherVersion.Patch;
        }

        // TO STRING: -----------------------------------------------------------------------------

        public override string ToString() => $"{this.Major}.{this.Minor}.{this.Patch}";
    }
}