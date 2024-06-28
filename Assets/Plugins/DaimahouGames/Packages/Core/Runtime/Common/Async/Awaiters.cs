using System;
using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class Awaiters
    {
        private static readonly WaitForUpdate WaitForUpdate = new();
        private static readonly WaitForFixedUpdate WaitForFixedUpdate = new();
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new();

        public static WaitForUpdate NextFrame => WaitForUpdate;
        public static WaitForFixedUpdate FixedUpdate => WaitForFixedUpdate;
        public static WaitForEndOfFrame EndOfFrame => WaitForEndOfFrame;

        public static WaitForSeconds Seconds(float seconds) => new WaitForSeconds(seconds);
        public static WaitForSecondsRealtime SecondsRealtime(float seconds) => new WaitForSecondsRealtime(seconds);
        public static WaitUntil Until(Func<bool> predicate) => new WaitUntil(predicate);
        public static WaitWhile While(Func<bool> predicate) => new WaitWhile(predicate);
    }
}