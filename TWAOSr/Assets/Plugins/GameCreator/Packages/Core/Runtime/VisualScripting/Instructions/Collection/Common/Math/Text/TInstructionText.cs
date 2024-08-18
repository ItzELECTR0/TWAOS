using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Set", "Where the resulting value is set")]

    [Keywords("String", "Text", "Character")]
    [Serializable]
    public abstract class TInstructionText : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected PropertySetString m_Set = SetStringNone.Create;
    }
}