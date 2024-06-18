using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Scale")]

    [Serializable]
    public abstract class PropertyTypeGetScale : TPropertyTypeGet<Vector3>
    {
        protected enum ScaleSpace
        {
            Local,
            Global
        }
    }
}