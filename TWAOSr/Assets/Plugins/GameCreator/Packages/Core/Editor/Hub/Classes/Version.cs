using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class Version
    {
        public int x;
        public int y;
        public int z;

        public Version()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Version(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Version Zero => new Version(0,0,0);

        public override string ToString() => $"{this.x}.{this.y}.{this.z}";
    }
}