using System;

namespace GameCreator.Runtime.Common
{
    public interface ITweenInput
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public int Hash { get; }
        public float Duration { get; }
        
        public bool IsFinished { get; }
        public bool IsComplete { get; }
        public bool IsCanceled { get; }

        // EVENTS: --------------------------------------------------------------------------------
        
        event Action<bool> EventFinish;
        
        // METHODS: -------------------------------------------------------------------------------

        bool OnUpdate();

        void OnComplete();
        void OnCancel();
    }
}