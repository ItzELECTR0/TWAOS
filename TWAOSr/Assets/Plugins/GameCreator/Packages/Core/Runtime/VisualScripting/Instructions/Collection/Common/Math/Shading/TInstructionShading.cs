using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Set", "Where the resulting Color value is set")]
    [Keywords("Shade", "Tint", "Hue", "Colour", "Color", "Paint", "Tone")]

    [Serializable]
    public abstract class TInstructionShading : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        protected PropertySetColor m_Set = SetColorGlobalName.Create;
    }
}