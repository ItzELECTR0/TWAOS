using System;

namespace GameCreator.Editor.Hub
{
    [Serializable]
    public class Date
    {
        public long _seconds;
        public long _nanoseconds;

        public override string ToString()
        {
            return this.ToString("d");
        }

        public string ToString(string format)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(this._seconds).ToString(format);
        }
    }
}