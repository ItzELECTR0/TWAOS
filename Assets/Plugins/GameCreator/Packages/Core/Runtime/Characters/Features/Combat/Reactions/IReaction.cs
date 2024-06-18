using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    public interface IReaction
    {
        /// <summary>
        /// Returns null if the reaction cannot be played. Otherwise returns the reaction entry.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        ReactionItem CanRun(Character character, Args args, ReactionInput input);
        
        /// <summary>
        /// Plays the reaction based on the ReactionItem entry provided.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <param name="reaction"></param>
        /// <returns></returns>
        ReactionOutput Run(Character character, Args args, ReactionInput input, ReactionItem reaction);
        
        /// <summary>
        /// Calculates and plays the reaction based on the raw input.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        ReactionOutput Run(Character character, Args args, ReactionInput input);
    }
}