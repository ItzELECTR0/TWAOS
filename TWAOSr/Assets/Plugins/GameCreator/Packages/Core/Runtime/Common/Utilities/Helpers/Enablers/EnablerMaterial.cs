using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerMaterial : TEnablerValue<Material>
    {
        public EnablerMaterial()
        { }

        public EnablerMaterial(Material value) : base(false, value)
        { }

        public EnablerMaterial(bool isEnabled, Material value) : base(isEnabled, value)
        { }
    }
}