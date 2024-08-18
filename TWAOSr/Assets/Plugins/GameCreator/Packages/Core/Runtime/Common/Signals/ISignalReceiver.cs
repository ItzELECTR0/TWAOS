using UnityEngine;

namespace GameCreator.Runtime.Common
{
    /// <summary>
    /// Implement this interface to subscribe objects to a specific signal using
    /// <code>Signals.Instance.Subscribe(..)</code> and unsubscribe
    /// using <code>Signals.Instance.Unsubscribe(..)</code>
    /// </summary>
    public interface ISignalReceiver
    {
        void OnReceiveSignal(SignalArgs args);
    }
}