#if UNITY_2022_2_OR_NEWER
using System;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Sequences
{
    class DebouncedToolbarSearchField : ToolbarSearchField
    {
        Action m_DebounceOff;
        string m_NewValue;
        string m_PreviousValue;

        readonly string k_DebounceThresholdKeyName = "Search.DebounceThresholdMs";

        public DebouncedToolbarSearchField()
        {
            textInputField.RegisterValueChangedCallback(evt =>
            {
                // Intercept the ChangeEvent before other subscribers receive it.
                evt.StopImmediatePropagation();

                // Store values to set when we fire a new ChangeEvent.
                // The event object gets recycled by the event pool, so we can't save it to reuse later.
                m_NewValue = evt.newValue;
                m_PreviousValue = evt.previousValue;

                var delaySeconds = EditorPrefs.GetInt(k_DebounceThresholdKeyName, 250) / 1000D;

                // Cancel pending SendDebouncedChangeEvent invocation then schedule a new one.
                m_DebounceOff?.Invoke();
                m_DebounceOff = Dispatcher.CallDelayed(SendDebouncedChangeEvent, delaySeconds);
            });

            RegisterCallback<DetachFromPanelEvent>(DetachFromPanel);
        }

        void SendDebouncedChangeEvent()
        {
            using var evt = ChangeEvent<string>.GetPooled(m_PreviousValue, m_NewValue);
            evt.target = this;
            SendEvent(evt);
        }

        void DetachFromPanel(DetachFromPanelEvent evt)
        {
            // Cancel existing job
            m_DebounceOff?.Invoke();
        }
    }
}
#endif
