using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Rotation")]

    [Serializable]
    public abstract class PropertyTypeGetRotation : TPropertyTypeGet<Quaternion>
    {
        protected enum RotationSpace
        {
            Local,
            Global
        }
    }
}