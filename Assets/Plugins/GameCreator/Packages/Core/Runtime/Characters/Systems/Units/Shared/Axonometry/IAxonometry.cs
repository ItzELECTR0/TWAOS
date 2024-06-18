using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Axonometry Type")]
    
    public interface IAxonometry : ICloneable
    {
        /// <summary>
        /// Driver unit. Executed whenever the movement speed and direction vectors are calculated and
        /// are ready to be fed into the desired controller.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="movement"></param>
        /// <returns></returns>
        Vector3 ProcessTranslation(TUnitDriver driver, Vector3 movement);

        /// <summary>
        /// Driver unit. Processes the character position after all movement is performed
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="position"></param>
        void ProcessPosition(TUnitDriver driver, Vector3 position);

        /// <summary>
        /// Facing unit. Processes the character rotation/direction after all rotations are performed
        /// </summary>
        /// <param name="facing"></param>
        /// <param name="direction"></param>
        Vector3 ProcessRotation(TUnitFacing facing, Vector3 direction);
    }
}