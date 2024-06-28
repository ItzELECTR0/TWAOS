using System.Reflection;

namespace DaimahouGames.Editor.Core
{
    public static class ReflectionUtils
    {
        public static T GetFieldValue<TSource, T>(this object obj, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = typeof(TSource).GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
    }
}