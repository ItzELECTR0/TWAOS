using System;
using System.Threading.Tasks;
using UnityEngine.Playables;

namespace GameCreator.Runtime.Characters.Animim
{
    public abstract class TAnimimOutput : PlayableBehaviour
    {
        protected static readonly Task TASK_COMPLETE = Task.FromResult(true);
        
        protected const float SAFE_TIME_EPSILON = 0.01f;
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] protected readonly AnimimGraph m_AnimimGraph;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] protected Playable ScriptPlayable { get; private set; }
        
        internal abstract float RootMotion { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TAnimimOutput(AnimimGraph animimGraph)
        {
            this.m_AnimimGraph = animimGraph;
        }
        
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        internal abstract void OnDeleteChild(TAnimimPlayableBehaviour playableBehaviour);

        // OVERRIDES: -----------------------------------------------------------------------------

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            this.ScriptPlayable = playable;
        }
    }
}