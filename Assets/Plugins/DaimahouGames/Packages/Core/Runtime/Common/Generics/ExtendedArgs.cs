using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace DaimahouGames.Runtime.Core.Common
{
    public class ExtendedArgs : Args
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private readonly Dictionary<Type, object> m_Args = new();

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public new ExtendedArgs Clone
        {
            get
            {
                var clone = new ExtendedArgs(Self, Target);
                foreach (var (type, arg) in m_Args) clone.Set(type, arg);
                return clone;
            }
        }
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        
        public ExtendedArgs(Args args) : base(args?.Self, args?.Target) {}
        public ExtendedArgs(Component target) : base(target) {}
        public ExtendedArgs(GameObject target) : base(target) {}
        public ExtendedArgs(Component self, Component target) : base(self, target) {}
        public ExtendedArgs(GameObject self, GameObject target) : base(self, target) {}
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public static ExtendedArgs Upgrade(ref Args args)
        {
            if(args is not ExtendedArgs) args = new ExtendedArgs(args);
            return (ExtendedArgs) args;
        }

        public bool Has<T>() => m_Args.ContainsKey(typeof(T));
        
        public T Get<T>()
        {
            if(m_Args.TryGetValue(typeof(T), out var arg))
            {
                return (T) arg;
            }

            return default;
        }

        public bool GetBool(string name)
        {
            if (!m_Args.TryGetValue(typeof(Dictionary<string, bool>), out var arg)) return false;
            
            var argDic = arg as Dictionary<string, bool>;
            Assert.IsNotNull(argDic);

            return argDic.TryGetValue(name, out var result) && result;
        }

        public void SetBool(string name, bool value)
        {
            var argDic = Get<Dictionary<string, bool>>();

            if (argDic == null)
            {
                argDic = new Dictionary<string, bool>();
                m_Args[typeof(Dictionary<string, bool>)] = argDic;
            }
            
            argDic[name] = value;
        }

        public static void Set<T>(ref Args args, T value)
        {
            Upgrade(ref args).Set(value);
        }

        public void Set<T>(T arg)
        {
            if (arg is Target target)
            {
                ChangeTarget(target.GameObject);
            }
            m_Args[typeof(T)] = arg;
        }

        public void Set(Type type, object arg)
        {
            m_Args[type] = arg;
        }
        
        public void Clear() => m_Args.Clear();

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}