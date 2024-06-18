using System;
using UnityEngine;

namespace GameCreator.Editor.Installs
{
    [Serializable]
    public struct Version
    {
        public int major;
        public int minor;
        public int patch;

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public Version(int major, int minor, int patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public Version(string version)
        {
            major = 0;
            minor = 0;
            patch = 0;
            
            string[] split = version.Split('.');
            if (split.Length == 3)
            {
                this.major = int.Parse(split[0]);
                this.minor = int.Parse(split[1]);
                this.patch = int.Parse(split[2]);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public static Version Zero => new Version(0,0,0);
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => $"{this.major}.{this.minor}.{this.patch}";

        // COMPARERS: -----------------------------------------------------------------------------
        
        public bool IsLower(Version version)
        {
            if (this.major < version.major) return true;
            if (this.major == version.major)
            {
                if (this.minor < version.minor) return true;
                if (this.minor == version.minor)
                {
                    if (this.patch < version.patch) return true;
                }
            }

            return false;
        }
        
        public bool IsHigher(Version version)
        {
            if (this.major > version.major) return true;
            if (this.major == version.major)
            {
                if (this.minor > version.minor) return true;
                if (this.minor == version.minor)
                {
                    if (this.patch > version.patch) return true;
                }
            }

            return false;
        }
        
        public bool IsEqual(Version version)
        {
            if (this.major != version.major) return false;
            if (this.minor != version.minor) return false;
            if (this.patch != version.patch) return false;
            
            return true;
        }

        public bool IsHigherOrEqual(Version version)
        {
            return this.IsEqual(version) || this.IsHigher(version);
        }
        
        public bool IsLowerOrEqual(Version version)
        {
            return this.IsEqual(version) || this.IsLower(version);
        }
    }
}