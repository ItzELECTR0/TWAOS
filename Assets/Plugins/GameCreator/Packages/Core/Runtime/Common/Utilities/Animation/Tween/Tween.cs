using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class Tween
    {
        /// <summary>
        /// Creates a tweening transition, cancelling any other transitions affecting the targeted
        /// member.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="input"></param>
        public static void To(GameObject gameObject, ITweenInput input)
        {
            TweenRunner tween = gameObject.Get<TweenRunner>();
            if (tween == null) tween = gameObject.Add<TweenRunner>();
            
            tween.To(input);
        }
        
        /// <summary>
        /// Cancels a particular tweening transition with a specific hash
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="hash"></param>
        public static void Cancel(GameObject gameObject, int hash)
        {
            TweenRunner tween = gameObject.Get<TweenRunner>();
            if (tween == null) return;
            
            tween.Cancel(hash);
        }
        
        /// <summary>
        /// Cancels all ongoing tweening transition on the game object
        /// </summary>
        /// <param name="gameObject"></param>
        public static void CancelAll(GameObject gameObject)
        {
            TweenRunner tween = gameObject.Get<TweenRunner>();
            if (tween == null) return;
            
            tween.CancelAll();
        }

        /// <summary>
        /// Calculates the hash of the member
        /// </summary>
        /// <param name="type"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static int GetHash(Type type, string member)
        {
            if (type == null) return 0;
            
            string typeName = type.Name.ToLowerInvariant();
            string memberName = member.ToLowerInvariant();
            
            return $"{typeName}.{memberName}".GetHashCode();
        }

        /// <summary>
        /// Calculates the hash of the member
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static int GetHash(Type type, Component instance, string member)
        {
            if (type == null) return 0;
            if (instance == null) return 0;
            
            string typeName = type.Name.ToLowerInvariant();
            string memberName = member.ToLowerInvariant();
            
            return $"{typeName}.{instance.GetInstanceID()}.{memberName}".GetHashCode();
        }
    }
}