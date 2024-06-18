using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public interface IInteractive : ISpatialHash
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        /// <summary>
        /// The scene object that this interface belongs to
        /// </summary>
        GameObject Instance { get; }
        
        /// <summary>
        /// Returns the scene object instance id that this interface belongs to
        /// </summary>
        int InstanceID { get; }
        
        /// <summary>
        /// Whether this Interactive object is being interacted. Useful to hide any interaction
        /// tooltips while it is running
        /// </summary>
        bool IsInteracting { get; }

        // METHODS: -------------------------------------------------------------------------------
        
        /// <summary>
        /// Executed when a character attempts to interact with this interface
        /// </summary>
        /// <param name="character"></param>
        void Interact(Character character);

        /// <summary>
        /// Executed when the interaction finishes
        /// </summary>
        void Stop();
    }
}