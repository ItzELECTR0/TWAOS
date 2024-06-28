using UnityEngine;
using Object = UnityEngine.Object;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class ValidationExtensions
    {
        public static T Required<T>(this T obj, string msg = null)
        {
            return obj.RequiredOn(null, msg);
        }
        
        public static T RequiredOn<T>(this T obj, Object context, string msg = null)
        {
            if (obj != null) return obj;

            var err = string.Format("<color=red>MISSING {0}</color>- {1}{2}", 
                typeof(T).Name, 
                context ? $" on {context.name}" : "",
                string.IsNullOrEmpty(msg) ? "" : $" - {msg}"
            );
            
            Debug.LogError(err, context);

            var mb = context as MonoBehaviour;
            if (mb) mb.enabled = false;
            
            return default;
        }
    }
}