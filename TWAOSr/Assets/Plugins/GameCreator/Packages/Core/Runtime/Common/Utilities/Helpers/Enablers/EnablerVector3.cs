using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerVector3 : TEnablerValue<Vector3>
    {
        public EnablerVector3()
        { }

        public EnablerVector3(Vector3 value) : base(false, value)
        { }

        public EnablerVector3(bool isEnabled, Vector3 value) : base(isEnabled, value)
        { }
    }
}