using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    public class WaitForUpdate : CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
}