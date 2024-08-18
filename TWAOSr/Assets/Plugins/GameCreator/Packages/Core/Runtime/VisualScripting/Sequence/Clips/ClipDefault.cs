using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class ClipDefault : Clip
    {
        public const string NAME_INSTRUCTIONS = nameof(m_Instructions);
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private RunInstructionsList m_Instructions = new RunInstructionsList();
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ClipDefault() : base(default)
        { }
        
        public ClipDefault(float time) : base(time)
        { }

        public ClipDefault(InstructionList instructions, float time) : base(time)
        {
            this.m_Instructions = new RunInstructionsList(instructions);
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void OnStart(ITrack track, Args args)
        {
            base.OnStart(track, args);
            this.Run(args);
        }

        // METHODS: -------------------------------------------------------------------------------
        
        private void Run(Args args)
        {
            _ = this.m_Instructions.Run(
                args.Clone,
                new RunnerConfig
                {
                    Name = "On Clip Run",
                    Location = new RunnerLocationLocation(
                        args.Self != null ? args.Self.transform.position : Vector3.zero,
                        args.Self != null ? args.Self.transform.rotation : Quaternion.identity
                    )
                }
            );
        }
    }
}