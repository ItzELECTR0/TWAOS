using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Characters
{
    [Title("Player")]
    
    public interface IUnitPlayer : IUnitCommon
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        bool IsControllable { get; set; }

        Vector3 InputDirection { get; }
    }
}