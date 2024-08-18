using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class Signals
    {
        [NonSerialized]
        private static Dictionary<PropertyName, List<ISignalReceiver>> SIGNALS = 
            new Dictionary<PropertyName, List<ISignalReceiver>>();
        
        // INITIALIZERS: --------------------------------------------------------------------------

        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode()
        {
            SIGNALS = new Dictionary<PropertyName, List<ISignalReceiver>>();
        }

        #endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        /// <summary>
        /// Emits an event with the specified Signal <paramref name="args"/>. If any receivers
        /// are listening, these will be invoked in order. 
        /// </summary>
        public static void Emit(SignalArgs args)
        {
            if (ApplicationManager.IsExiting) return;
            if (!SIGNALS.TryGetValue(args.signal, out List<ISignalReceiver> receivers))
            {
                return;
            }
            
            foreach (ISignalReceiver receiver in receivers)
            {
                receiver.OnReceiveSignal(args);
            }
        }

        // SUBSCRIPTION METHODS: ------------------------------------------------------------------

        /// <summary>
        /// Starts listening for the specific <paramref name="signal"/>. When that signal is raised
        /// it invokes the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The implementing object that listens to the signal</param>
        /// <param name="signal">The signal to listen</param>
        public static void Subscribe(ISignalReceiver source, PropertyName signal)
        {
            if (ApplicationManager.IsExiting) return;
            if (!SIGNALS.TryGetValue(signal, out List<ISignalReceiver> receivers))
            {
                receivers = new List<ISignalReceiver>();
                SIGNALS.Add(signal, receivers);
            }

            foreach (ISignalReceiver receiver in receivers)
            {
                if (receiver == source) return;
            }
            
            receivers.Add(source);
        }

        /// <summary>
        /// Stops the object <paramref name="source"/> implementing the interface from listening
        /// the <paramref name="signal"/> 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="signal"></param>
        public static void Unsubscribe(ISignalReceiver source, PropertyName signal)
        {
            if (ApplicationManager.IsExiting) return;
            if (!SIGNALS.TryGetValue(signal, out List<ISignalReceiver> receivers)) return;
            receivers.Remove(source);
                
            if (receivers.Count > 0) return;
            SIGNALS.Remove(signal);
        }
    }
}