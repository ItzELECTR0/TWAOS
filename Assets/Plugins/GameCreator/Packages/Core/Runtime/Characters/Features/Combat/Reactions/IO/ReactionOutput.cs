using System;

namespace GameCreator.Runtime.Characters
{
    public readonly struct ReactionOutput
    {
        public static readonly ReactionOutput None = new ReactionOutput();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        /// <summary>
        /// The source length in seconds of the animation clip played.
        /// </summary>
        [field: NonSerialized] public float Length { get; }
        
        /// <summary>
        /// The speed coefficient applied to the animation clip played.
        /// </summary>
        [field: NonSerialized] public float Speed { get; }
        
        /// <summary>
        /// The duration from which the reaction can be canceled.
        /// </summary>
        [field: NonSerialized] public float CancelTime { get; }
        
        /// <summary>
        /// The gravity influence of the reaction.
        /// </summary>
        [field: NonSerialized] public float Gravity { get; }
        
        /// <summary>
        /// The Reaction asset reference played.
        /// </summary>
        [field: NonSerialized] public Reaction Reaction { get; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ReactionOutput(float length, float speed, float cancelTime, float gravity, Reaction reaction)
        {
            this.Length = length;
            this.Speed = speed;
            this.CancelTime = cancelTime;
            this.Gravity = gravity;
            this.Reaction = reaction;
        }
    }
}